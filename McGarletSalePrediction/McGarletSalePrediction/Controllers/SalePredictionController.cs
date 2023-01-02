using com.fabioscagliola.IntegrationTesting.McGarletSalePrediction.ML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using Swashbuckle.AspNetCore.Annotations;

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
                MachineLearningModel machineLearningModel = new(@"C:\Data\thesoftwaretailors\McGarlet-Sale-Prediction.NET\Data\Data.csv");  // TODO: Hardcoded 
                IDataView dataView = machineLearningModel.CreateDataView(data.ProdCode);
                TimeSeriesPredictionEngine<ActualData, ForecastedData> predictionEngine = machineLearningModel.CreatePredictionEngine(dataView, trainModel: true);  // TODO: Train model 
                IEnumerable<ForecastedSale> forecastedSaleList = machineLearningModel.Forecast(predictionEngine, dataView, data.Days);
                return base.Ok(forecastedSaleList);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
