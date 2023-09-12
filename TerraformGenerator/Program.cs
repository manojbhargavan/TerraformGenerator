using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Extensions;
using OpenAI.Managers;
using System.Security;
using System.Security.Authentication.ExtendedProtection;
using TerraformGenerator.Business;

namespace TerraformGenerator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Init Dependency Injection Container
            var service = new ServiceCollection();
            service.ConfigureServices();

            // Start User Prompting
            var serviceProvider = service.BuildServiceProvider();
            serviceProvider.GetRequiredService<TerraformHelper>().StartUserPrompting().Wait();   
        }
    }
}