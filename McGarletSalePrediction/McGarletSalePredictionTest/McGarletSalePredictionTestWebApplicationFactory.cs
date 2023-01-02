using com.fabioscagliola.IntegrationTesting.McGarletSalePrediction;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace com.fabioscagliola.IntegrationTesting.McGarletSalePredictionTest
{
    public class McGarletSalePredictionTestWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(configureServices =>
            {
                configureServices.Remove(configureServices.Single(d => d.ServiceType == typeof(DbContextOptions<McGarletSalePredictionDbContext>)));

                configureServices.AddSingleton((Func<IServiceProvider, DbConnection>)(implementationFactory =>
                {
                    SqliteConnection sqliteConnection = new(Settings.Instance.SqliteConnectionString);
                    sqliteConnection.Open();
                    return sqliteConnection;
                }));

                configureServices.AddDbContext<McGarletSalePredictionDbContext>((serviceProvider, dbContextOptionBuilder) =>
                {
                    DbConnection dbConnection = serviceProvider.GetRequiredService<DbConnection>();
                    dbContextOptionBuilder.UseSqlite(dbConnection);
                });

                ServiceProvider serviceProvider = configureServices.BuildServiceProvider();
                IServiceScope serviceScope = serviceProvider.CreateScope();
                McGarletSalePredictionDbContext webApiDbContext = serviceScope.ServiceProvider.GetRequiredService<McGarletSalePredictionDbContext>();
                webApiDbContext.Database.EnsureCreated();
            });
        }
    }
}
