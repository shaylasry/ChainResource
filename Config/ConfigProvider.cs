namespace ChainResource.Config {

using Microsoft.Extensions.Configuration;

    public static class ConfigProvider
    {
        public static readonly IConfiguration Configuration;

        static ConfigProvider()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public static string GetApiKey() =>
            Configuration["OpenExchangeRates:ApiKey"] ?? throw new Exception("API Key not found");
    }
}