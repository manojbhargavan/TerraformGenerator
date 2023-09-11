using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.Interfaces;

namespace TerraformGenerator
{
    internal class Sample
    {
        private readonly IOpenAIService openAIService;

        public Sample(IOpenAIService openAIService)
        {
            this.openAIService = openAIService;
        }

        public async Task GetResponse()
        {
            var completionResult = await openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                            {
                                ChatMessage.FromSystem("You are a helpful assistant."),
                                ChatMessage.FromUser("Who won the world series in 2020?"),
                                ChatMessage.FromAssistant("The Los Angeles Dodgers won the World Series in 2020."),
                                ChatMessage.FromUser("Where was it played?")
                            },
                Model = "gpt-4",
                MaxTokens = 50//optional
            });
            if (completionResult.Successful)
            {
                Console.WriteLine(completionResult.Choices.First().Message.Content);
            }
        }
    }
}
