using com.fabioscagliola.IntegrationTesting.McGarletSalePrediction.ML;

namespace com.fabioscagliola.IntegrationTesting.McGarletSalePrediction
{
    public class ForecastedSale : ActualData
    {
        public int Id { get; set; }

        public float ForecastedQuantity { get; set; }

        public float LowerBoundQuantity { get; set; }

        public float UpperBoundQuantity { get; set; }

        public ForecastedSale() { }

        public ForecastedSale(ActualData actualSale) : this()
        {
            Date = actualSale.Date;
            ProdCode = actualSale.ProdCode;
            ProdName = actualSale.ProdName;
            Quantity = actualSale.Quantity;
        }
    }
}
