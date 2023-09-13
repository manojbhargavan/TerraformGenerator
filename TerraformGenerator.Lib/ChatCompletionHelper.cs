using TerraformGenerator.Lib.Providers;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using Microsoft.Extensions.DependencyInjection;

namespace TerraformGenerator.Lib
{
    public class ChatCompletionHelper
    {
        private readonly IOpenAIService openAIService;
        private readonly ITerraformHelper terraformHelper;

        public CloudProvider CloudProvider { get; private set; }

        public ChatCompletionHelper(IServiceProvider serviceProvider)
        {
            this.openAIService = serviceProvider.GetRequiredService<IOpenAIService>();
            this.terraformHelper = serviceProvider.GetRequiredService<ITerraformHelper>();
        }

        public async Task<string> StartUserPrompting(string runId)
        {

            var prompt = terraformHelper.GenerateTerraformPrompt(runId);
            if (!prompt.promptGenerated)
                Console.WriteLine("Nothing to do.. Please retry");
            else
                Console.WriteLine($"{Environment.NewLine}--Prompt--{Environment.NewLine}" +
                    $"{string.Join(Environment.NewLine, prompt.promptList)}{Environment.NewLine}" +
                    $"--Prompt--{Environment.NewLine}Please wait...{Environment.NewLine}");

            var completionResult = await openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = prompt.promptList.Select(p => ChatMessage.FromUser(p)).ToList(),
                Model = Models.Gpt_4,
                Temperature = 0,
                N = 1
            });
            if (completionResult.Successful)
            {
                string result = completionResult.Choices.First().Message.Content;
                Console.WriteLine($"--Response--\n{result}\n--Response--");
                return result;
            }
            else
            {
                Console.WriteLine($"Error: {completionResult.Error?.Message}");
                return "";
            }

        }
    }
}
