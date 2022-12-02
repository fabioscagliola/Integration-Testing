using com.fabioscagliola.IntegrationTesting.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace com.fabioscagliola.IntegrationTesting.WebApiTest
{
    public class Tests
    {
        protected WebApplicationFactory<Program> webApplicationFactory;

        [SetUp]
        public void Setup()
        {
            webApplicationFactory = new WebApplicationFactory<Program>();
        }

        [TearDown]
        public void TearDown()
        {
            webApplicationFactory.Dispose();
        }

        [Test]
        public async Task Test1()
        {
            HttpClient httpClient = webApplicationFactory.CreateClient();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("https://localhost:7108/Person/List");
            httpResponseMessage.EnsureSuccessStatusCode();

        }
    }
}
