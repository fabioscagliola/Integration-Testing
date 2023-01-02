using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace com.fabioscagliola.IntegrationTesting.McGarletSalePrediction
{
    public class Program
    {
        static void Main(string[] args)
        {
            WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

            webApplicationBuilder.Services.AddControllers();
            webApplicationBuilder.Services.AddEndpointsApiExplorer();

            webApplicationBuilder.Services.AddSwaggerGen(setupAction =>
            {
                setupAction.EnableAnnotations();
                setupAction.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

            webApplicationBuilder.Services.AddDbContext<McGarletSalePredictionDbContext>(optionsAction =>
            {
                optionsAction.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("ConnectionString"));
            });

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
