using com.fabioscagliola.IntegrationTesting.WebApi;
using NUnit.Framework;

namespace com.fabioscagliola.IntegrationTesting.WebApiTest;

public abstract class BaseTest
{
    protected WebApiTestWebApplicationFactory<Program> WebApiTestWebApplicationFactory;

    [SetUp]
    public void Setup()
    {
        WebApiTestWebApplicationFactory = new WebApiTestWebApplicationFactory<Program>();
    }

    [TearDown]
    public void TearDown()
    {
        WebApiTestWebApplicationFactory.Dispose();
    }
}
