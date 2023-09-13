using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TerraformGenerator.Lib;
using TerraformGenerator.Lib.Providers;
using TerraformGenerator.Lib.Providers.Azure;
using TerraformGenerator.Utils;
using Helper = TerraformGenerator.Lib.Utils.Helper;

namespace TerraformGenerator
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // Init Dependency Injection Container
            var service = new ServiceCollection();
            service.ConfigureServices();

            // Start User Prompting
            var serviceProvider = service.BuildServiceProvider();

            // Get Configuration
            var config = serviceProvider.GetService<IConfiguration>()!;


            string runId = Guid.NewGuid().ToString("N").Substring(0, 15);

            // Get From User
            CloudProvider provider = CloudProvider.Azure;

            switch (provider)
            {
                case CloudProvider.AWS:
                    break;
                case CloudProvider.GCP:
                    break;
                case CloudProvider.Azure:
                default:
                    ITerraformHelper terraformHelper = new AzureTerraformHelper(serviceProvider.GetRequiredService<IAzureInputProvider>());
                    break;
            }


            var response = await serviceProvider.GetRequiredService<ChatCompletionHelper>().StartUserPrompting(runId);

            // Parse Response and Write to Files
            var outputFolder = "Output Folder".GetUserInputString();
            if (string.IsNullOrWhiteSpace(outputFolder)) outputFolder = Path.Join(config["Defaults:OutputFolder"], runId);
            else outputFolder = Path.Join(outputFolder, runId);

            var filesGenerated = Helper.ParseResponseAndWrite(response, runId, outputFolder);
            Console.WriteLine($"Files Generated: \n{string.Join(Environment.NewLine, filesGenerated)}");
        }
    }
}