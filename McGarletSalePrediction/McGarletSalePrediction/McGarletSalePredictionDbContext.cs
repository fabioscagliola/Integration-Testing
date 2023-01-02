using Microsoft.EntityFrameworkCore;

namespace com.fabioscagliola.IntegrationTesting.McGarletSalePrediction
{
    public class McGarletSalePredictionDbContext : DbContext
    {
        public McGarletSalePredictionDbContext(DbContextOptions options) : base(options) { }

        public DbSet<ForecastedSale> ForecastedSale { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ForecastedSale>().ToTable(nameof(ForecastedSale));
        }
    }
}
