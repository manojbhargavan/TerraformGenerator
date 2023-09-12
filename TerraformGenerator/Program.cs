using LoremNET;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Extensions;
using OpenAI.Managers;
using System.Security;
using System.Security.Authentication.ExtendedProtection;
using TerraformGenerator.Business;
using TerraformGenerator.Utils;

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
            var response = await serviceProvider.GetRequiredService<TerraformHelper>().StartUserPrompting(runId);

            // Parse Response and Write to Files
            var outputFolder = "Output Folder".GetUserInputString();
            if (string.IsNullOrWhiteSpace(outputFolder)) outputFolder = $"{config["Defaults:OutputFolder"]}{runId}";
            else outputFolder = Path.Join(outputFolder, runId);

            var filesGenerated = Helper.ParseResponseAndWrite(response, runId, outputFolder);
            Console.WriteLine($"Files Generated: \n{string.Join(Environment.NewLine, filesGenerated)}");
        }
    }
}