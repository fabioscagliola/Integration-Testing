using com.fabioscagliola.IntegrationTesting.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace com.fabioscagliola.IntegrationTesting.WebApiTest
{
    public class WebApiTestWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(configureServices =>
            {
                configureServices.Remove(configureServices.Single(d => d.ServiceType == typeof(DbContextOptions<WebApiDbContext>)));

                configureServices.AddDbContext<WebApiDbContext>(optionsAction =>
                {
                    SqliteConnection sqliteConnection = new("DataSource=:memory:");
                    sqliteConnection.Open();
                    optionsAction.UseSqlite(sqliteConnection);
                });
            });
        }
    }
}
