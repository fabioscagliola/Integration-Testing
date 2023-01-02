#nullable disable

using Microsoft.ML.Data;

namespace com.fabioscagliola.IntegrationTesting.McGarletSalePrediction.ML
{
    public class ActualData
    {
        [LoadColumn(0)]
        public DateTime Date { get; set; }

        [LoadColumn(1)]
        public int ProdCode { get; set; }

        [LoadColumn(2)]
        public string ProdName { get; set; }

        [LoadColumn(3)]
        public float Quantity { get; set; }
    }
}
