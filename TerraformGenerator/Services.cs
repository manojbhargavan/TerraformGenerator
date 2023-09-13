using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Extensions;
using TerraformGenerator.Business.Azure;
using TerraformGenerator.Lib;
using TerraformGenerator.Lib.Providers.Azure;

namespace TerraformGenerator
{
    internal static class Services
    {
        internal static void ConfigureServices(this IServiceCollection services)
        {
            // Add Configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            services.AddSingleton(configuration);

            // Add GPT 
            services.AddOpenAIService(settings =>
            {
                string organization = configuration["CHAT_GPT_API_ORG"]?.ToString() ?? string.Empty;
                string apiKey = configuration["CHAT_GPT_API_KEY"]?.ToString() ?? string.Empty;

                if (string.IsNullOrEmpty(organization))
                {
                    throw new Exception("Chat GPT Organization cannot be blank. Please make sure CHAT_GPT_API_ORG environment variable is set");
                }

                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new Exception("Chat GPT API Key cannot be blank. Please make sure CHAT_GPT_API_KEY environment variable is set");
                }

                settings.Organization = organization;
                settings.ApiKey = apiKey;
            });

            // Inject TerraformHelper

            // Inject Input Providers
            // Azure
            var iAzureInputProvider = new AzureConsoleInputProvider();
            services.AddSingleton<IAzureInputProvider>(iAzureInputProvider);
            services.AddSingleton<ITerraformHelper>(new AzureTerraformHelper(iAzureInputProvider));
            services.AddSingleton<ChatCompletionHelper>();
        }
    }
}
