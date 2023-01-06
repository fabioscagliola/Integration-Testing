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
        readonly string dataPath;
        readonly string machineLearningModelPath = @"C:\Data\thesoftwaretailors\McGarlet-Sale-Prediction.NET\McGarletSalePrediction\McGarletSalePrediction\ML\MachineLearningModel.zip";  // TODO: Hardcoded 

        public MachineLearningModel(string dataPath)
        {
            ml = new MLContext();
            this.dataPath = dataPath;
        }

        public IDataView CreateDataView(int prodCode)
        {
            TextLoader textLoader = ml.Data.CreateTextLoader<ActualData>(
                separatorChar: ';',
                hasHeader: true,
                trimWhitespace: true);

            IDataView dataView = textLoader.Load(dataPath);

            return ml.Data.FilterByCustomPredicate<ActualData>(dataView, actualData => actualData.ProdCode != prodCode);
        }

        public TimeSeriesPredictionEngine<ActualData, ForecastedData> CreatePredictionEngine(IDataView dataView, bool trainModel = false)
        {
            ITransformer machineLearningModel;

            //int period = ml.AnomalyDetection.DetectSeasonality(dataView, nameof(ActualData.Quantity));
            //Trace.WriteLine($"The seasonality period is {period}");

            if (trainModel)
            {
                IEstimator<ITransformer> forecastingEstimator = ml.Forecasting.ForecastBySsa(
                    outputColumnName: nameof(ForecastedData.ForecastedQuantity),
                    inputColumnName: nameof(ActualData.Quantity),
                    windowSize: 7,
                    seriesLength: 30,
                    trainSize: 365,  // File.ReadLines(dataPath).Count() - 1,
                    horizon: 7,
                    confidenceLowerBoundColumn: nameof(ForecastedData.LowerBoundQuantity),
                    confidenceUpperBoundColumn: nameof(ForecastedData.UpperBoundQuantity),
                    confidenceLevel: .95f);

                forecastingEstimator.Append(ml.Transforms.NormalizeMeanVariance(nameof(ActualData.Quantity)));

                forecastingEstimator.AppendCacheCheckpoint(ml);

                machineLearningModel = forecastingEstimator.Fit(dataView);

                //Evaluate(machineLearningModel, dataView);
            }
            else
                machineLearningModel = ml.Model.Load(machineLearningModelPath, out _);

            TimeSeriesPredictionEngine<ActualData, ForecastedData> predictionEngine = machineLearningModel.CreateTimeSeriesEngine<ActualData, ForecastedData>(ml);

            if (trainModel)
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

        void Evaluate(ITransformer machineLearningModel, IDataView dataView)
        {
            IDataView forecast = machineLearningModel.Transform(dataView);

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
