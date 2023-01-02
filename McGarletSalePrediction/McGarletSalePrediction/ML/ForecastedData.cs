#nullable disable

namespace com.fabioscagliola.IntegrationTesting.McGarletSalePrediction.ML
{
    public class ForecastedData
    {
        public float[] ForecastedQuantity { get; set; }

        public float[] LowerBoundQuantity { get; set; }

        public float[] UpperBoundQuantity { get; set; }
    }
}
