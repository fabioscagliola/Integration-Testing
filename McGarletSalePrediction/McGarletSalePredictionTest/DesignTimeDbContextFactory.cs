using com.fabioscagliola.IntegrationTesting.McGarletSalePrediction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace com.fabioscagliola.IntegrationTesting.McGarletSalePredictionTest
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<McGarletSalePredictionDbContext>
    {
        public McGarletSalePredictionDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder<McGarletSalePredictionDbContext>();
            dbContextOptionsBuilder.UseSqlite(Settings.Instance.SqliteConnectionString, sqliteOptionsAction => sqliteOptionsAction.MigrationsAssembly("McGarletSalePredictionTest"));
            return new(dbContextOptionsBuilder.Options);
        }
    }
}
