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
        readonly IWebHostEnvironment webHostEnvironment;

        public SalePredictionController(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Predicts the sales of a product over a given number of days.
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
        /// Predicts the sales of a product over a given number of days and returns the result as a comma-separated values file.
        /// </summary>
        [HttpPost]
        [Route("[action]")]
        [SwaggerResponse(200, Type = typeof(string))]
        [SwaggerResponse(400)]
        public IActionResult PredictSalesAsText(PredictSalesData data)
        {
            try
            {
                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine("Date;ProdCode;ProdName;Quantity");
                foreach (ForecastedSale forecastedSale in DoForecastSaleList(data))
                    stringBuilder.AppendLine($"{forecastedSale.Date:yyyy-MM-dd};{forecastedSale.ProdCode};{forecastedSale.ProdName};{forecastedSale.ForecastedQuantity}");
                return base.Ok(stringBuilder.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        IEnumerable<ForecastedSale> DoForecastSaleList(PredictSalesData data)
        {
            MachineLearningModel machineLearningModel = new(Path.Combine(webHostEnvironment.ContentRootPath, "Data", "Data.csv"));  // TODO: Protect data  
            IDataView dataView = machineLearningModel.CreateDataView(data.ProdCode);
            TimeSeriesPredictionEngine<ActualData, ForecastedData> predictionEngine = machineLearningModel.CreatePredictionEngine(dataView, trainModel: true);  // TODO: Train model 
            return machineLearningModel.Forecast(predictionEngine, dataView, data.Days);
        }
    }
}
