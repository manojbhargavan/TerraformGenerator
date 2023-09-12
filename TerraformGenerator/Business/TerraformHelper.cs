using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraformGenerator.Business.Azure;

namespace TerraformGenerator.Business
{
    internal class TerraformHelper
    {
        private readonly IOpenAIService openAIService;
        public CloudProvider CloudProvider { get; private set; }

        public TerraformHelper(IOpenAIService openAIService)
        {
            this.openAIService = openAIService;
        }

        public async Task StartUserPrompting()
        {
            // Take from user
            CloudProvider provider = CloudProvider.Azure;

            ITerraformHelper terraformHelper;

            switch (provider)
            {
                case CloudProvider.Azure:
                default:
                    terraformHelper = new AzureTerraformHelper();
                    break;
            }

            var prompt = terraformHelper.GenerateTerraformPrompt();
            if (!prompt.promptGenerated)
                Console.WriteLine("Nothing to do.. Please retry");

            var completionResult = await openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = prompt.promptList.Select(p => ChatMessage.FromUser(p)).ToList(),
                Model = Models.Gpt_4,
                Temperature = 0,
                N = 1
            });
            if (completionResult.Successful)
            {
                Console.WriteLine(completionResult.Choices.First().Message.Content);
            }

        }
    }
}
