using NUnit.Framework;

namespace com.fabioscagliola.IntegrationTesting.WebApiTest
{
    public class PersonControllerTest : BaseTest
    {
        [Test]
        public async Task Person_List_Succeeds()
        {
            HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("https://localhost:65535/Person/List");
            httpResponseMessage.EnsureSuccessStatusCode();
        }
    }
}
