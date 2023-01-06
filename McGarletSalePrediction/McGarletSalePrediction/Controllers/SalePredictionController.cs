using com.fabioscagliola.IntegrationTesting.McGarletSalePrediction.ML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;

namespace com.fabioscagliola.IntegrationTesting.McGarletSalePrediction.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalePredictionController : ControllerBase
    {
        /// <summary>
        /// Predicts the sale of a product.
        /// </summary>
        [HttpPost]
        [Route("[action]")]
        [SwaggerResponse(200, Type = typeof(IEnumerable<ForecastedSale>))]
        [SwaggerResponse(400)]
        public IActionResult PredictSales(PredictSalesData data)
        {
            try
            {
                return base.Ok(DoForecastSaleList(data));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Predicts the sale of a product.
        /// </summary>
        [HttpPost]
        [Route("[action]")]
        [SwaggerResponse(200, Type = typeof(string))]
        [SwaggerResponse(400)]
        public IActionResult PredictSalesAsText(PredictSalesData data)
        {
            try
            {
                StringBuilder stringBuilder= new();
                stringBuilder.AppendLine("Date;ProdCode;ProdName;Quantity");
                foreach (ForecastedSale forecastedSale in DoForecastSaleList(data))
                    stringBuilder.AppendLine($"{forecastedSale.Date:yyyy-MM-dd};{forecastedSale.ProdCode};{forecastedSale.ProdName};{forecastedSale.Quantity}");
                return base.Ok(stringBuilder.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        static IEnumerable<ForecastedSale> DoForecastSaleList(PredictSalesData data)
        {
            MachineLearningModel machineLearningModel = new(@"C:\Data\thesoftwaretailors\McGarlet-Sale-Prediction.NET\Data\Data.csv");  // TODO: Hardcoded 
            IDataView dataView = machineLearningModel.CreateDataView(data.ProdCode);
            TimeSeriesPredictionEngine<ActualData, ForecastedData> predictionEngine = machineLearningModel.CreatePredictionEngine(dataView, trainModel: true);  // TODO: Train model 
            return machineLearningModel.Forecast(predictionEngine, dataView, data.Days);
        }
    }
}
