using Microsoft.EntityFrameworkCore;

namespace com.fabioscagliola.IntegrationTesting.WebApi
{
    public class Program
    {
        static void Main(string[] args)
        {
            WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

            webApplicationBuilder.Services.AddControllers();
            webApplicationBuilder.Services.AddEndpointsApiExplorer();
            webApplicationBuilder.Services.AddSwaggerGen();

            webApplicationBuilder.Services.AddDbContext<WebApiDbContext>(optionsAction =>
            {
                optionsAction.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("ConnectionString"));
            });

            //new() { Id = 1, FName = "Fabio", LName = "Scagliola" },
            //new() { Id = 2, FName = "Laura", LName = "Bernasconi" },

            WebApplication webApplication = webApplicationBuilder.Build();

            if (webApplication.Environment.IsDevelopment())
            {
                webApplication.UseSwagger();
                webApplication.UseSwaggerUI();
            }

            webApplication.UseAuthorization();

            webApplication.UseHttpsRedirection();

            webApplication.MapControllers();

            webApplication.Run();
        }
    }
}
