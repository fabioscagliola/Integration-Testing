using Microsoft.Extensions.Configuration;

namespace com.fabioscagliola.IntegrationTesting.WebApiTest
{
    class Settings
    {
        static Settings? instance;

        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
                    instance = configuration.GetSection("Settings").Get<Settings>();
                    if (instance == null)
                        throw new ApplicationException("Something went wrong while initializing the settings.");
                }

                return instance;
            }
        }

        public string WebApiUrl { get; set; }

        public string SqliteConnectionString { get; set; }
    }
}
