using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System.Diagnostics;

namespace com.fabioscagliola.IntegrationTesting.McGarletSalePrediction.ML
{
    public class MachineLearningModel
    {
        readonly MLContext ml;

        readonly string machineLearningModelPath = @"C:\Data\thesoftwaretailors\McGarlet-Sale-Prediction.NET\McGarletSalePrediction\McGarletSalePrediction\ML\MachineLearningModel.zip";  // TODO: Hardcoded 

        public MachineLearningModel()
        {
            ml = new MLContext();
        }

        public IDataView CreateDataView(int prodCode)
        {
            TextLoader textLoader = ml.Data.CreateTextLoader<ActualData>(
                separatorChar: ';',
                hasHeader: true,
                trimWhitespace: true);

            IDataView temp = textLoader.Load(@"C:\Data\thesoftwaretailors\McGarlet-Sale-Prediction.NET\Data\Data.csv");  // TODO: Hardcoded 

            return ml.Data.FilterByCustomPredicate<ActualData>(temp, actualData => actualData.ProdCode == prodCode);
        }

        public TimeSeriesPredictionEngine<ActualData, ForecastedData> CreatePredictionEngine(IDataView dataView)
        {
            SsaForecastingEstimator forecastingEstimator = ml.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedQuantity",
                inputColumnName: "Quantity",
                windowSize: 2,
                seriesLength: 10,
                trainSize: 2067559,  // TODO: Hardcoded 
                horizon: 10,
                confidenceLowerBoundColumn: "LowerBoundQuantity",
                confidenceUpperBoundColumn: "UpperBoundQuantity",
                confidenceLevel: .95f);

            ITransformer forecastingTransformer = forecastingEstimator.Fit(dataView);

            //Evaluate(forecastingTransformer, dataView);

            TimeSeriesPredictionEngine<ActualData, ForecastedData> predictionEngine = forecastingTransformer.CreateTimeSeriesEngine<ActualData, ForecastedData>(ml);

            predictionEngine.CheckPoint(ml, machineLearningModelPath);

            return predictionEngine;
        }

        public IEnumerable<ForecastedSale> Forecast(TimeSeriesPredictionEngine<ActualData, ForecastedData> predictionEngine, IDataView dataView, int days)
        {
            ForecastedData forecastedData = predictionEngine.Predict();

            return ml.Data.CreateEnumerable<ActualData>(dataView, false).Take(days).Select((actualData, i) => new ForecastedSale(actualData)
            {
                ForecastedQuantity = forecastedData.ForecastedQuantity[i],
                LowerBoundQuantity = Math.Max(0, forecastedData.LowerBoundQuantity[i]),
                UpperBoundQuantity = forecastedData.UpperBoundQuantity[i],
            });
        }

        void Evaluate(ITransformer forecastingTransformer, IDataView dataView)
        {
            IDataView forecast = forecastingTransformer.Transform(dataView);

            IEnumerable<float> actualQuantities = ml.Data.CreateEnumerable<ActualData>(dataView, true).Select(sale => sale.Quantity);

            IEnumerable<float> forecastedQuantities = ml.Data.CreateEnumerable<ForecastedData>(dataView, true).Select(sale => sale.ForecastedQuantity[0]);  // TODO: Throws an exception 

            IEnumerable<float> metricList = actualQuantities.Zip(forecastedQuantities, (actualQuantity, forecastedQuantity) => actualQuantity - forecastedQuantity);

            float meanAbsError = metricList.Average(x => Math.Abs(x));
            float rootMeanSquaredDeviation = (float)Math.Sqrt(metricList.Average(x => Math.Pow(x, 2)));

            Trace.WriteLine($"The mean absolute error is {meanAbsError:d3}");
            Trace.WriteLine($"The root-mean-square deviation is {rootMeanSquaredDeviation:d3}");
        }
    }
}
