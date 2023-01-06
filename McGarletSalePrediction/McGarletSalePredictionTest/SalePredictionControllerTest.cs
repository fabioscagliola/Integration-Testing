using com.fabioscagliola.IntegrationTesting.McGarletSalePrediction;
using com.fabioscagliola.IntegrationTesting.McGarletSalePrediction.Controllers;
using NUnit.Framework;
using System.Net.Http.Json;

namespace com.fabioscagliola.IntegrationTesting.McGarletSalePredictionTest
{
    public class SalePredictionControllerTest : BaseTest
    {
        [Test]
        public async Task SalePrediction_PredictSales_Succeeds()
        {
            PredictSalesData data = new() { ProdCode = 127, Days = 7 };
            HttpClient httpClient = McGarletSalePredictionTestWebApplicationFactory.CreateClient();
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.McGarletSalePredictionUrl}/SalePrediction/PredictSales", JsonContent.Create(data));
            httpResponseMessage.EnsureSuccessStatusCode();
            IEnumerable<ForecastedSale>? forecastedSaleList = await httpResponseMessage.Content.ReadFromJsonAsync(typeof(IEnumerable<ForecastedSale>)) as IEnumerable<ForecastedSale>;
            Assert.That(forecastedSaleList, Is.Not.Null);
            //TODO: Make assertions 
        }

        [Test]
        public async Task SalePrediction_PredictSalesAsText_Succeeds()
        {
            PredictSalesData data = new() { ProdCode = 127, Days = 7 };
            HttpClient httpClient = McGarletSalePredictionTestWebApplicationFactory.CreateClient();
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.McGarletSalePredictionUrl}/SalePrediction/PredictSalesAsText", JsonContent.Create(data));
            httpResponseMessage.EnsureSuccessStatusCode();
            //TODO: Make assertions 
        }
    }
}
