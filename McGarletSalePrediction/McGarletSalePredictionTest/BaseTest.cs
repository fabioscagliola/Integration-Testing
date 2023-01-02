using com.fabioscagliola.IntegrationTesting.McGarletSalePrediction;
using NUnit.Framework;

namespace com.fabioscagliola.IntegrationTesting.McGarletSalePredictionTest
{
    public abstract class BaseTest
    {
        protected McGarletSalePredictionTestWebApplicationFactory<Program> McGarletSalePredictionTestWebApplicationFactory;

        [SetUp]
        public void Setup()
        {
            McGarletSalePredictionTestWebApplicationFactory = new McGarletSalePredictionTestWebApplicationFactory<Program>();
        }

        [TearDown]
        public void TearDown()
        {
            McGarletSalePredictionTestWebApplicationFactory.Dispose();
        }
    }
}
