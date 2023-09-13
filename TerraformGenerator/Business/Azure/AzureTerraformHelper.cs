using System.Text;
using TerraformGenerator.Utils;

namespace TerraformGenerator.Business.Azure
{
    internal class AzureTerraformHelper : ITerraformHelper
    {
        private IAzureInputProvider _azureInputProvider;
        public AzureTerraformHelper(IAzureInputProvider azureInputProvider)
        {
            _azureInputProvider = azureInputProvider;
        }

        public (bool, List<string>) GenerateTerraformPrompt(string runId)
        {
            try
            {
                List<string> gptPrompt = new();

                List<AzureResourceDescriptor> resourcesToCreate = _azureInputProvider.GetAzureResourceDescriptor()!;

                // Nothing to Prompt for
                if (resourcesToCreate?.Count == 0) return (false, gptPrompt);

                // Create Prompt
                gptPrompt.Add(runId.GetCommonPrompt());
                foreach (var curRes in resourcesToCreate!.OrderBy(r => r.Order))
                {
                    StringBuilder currentPrompt = new("");
                    if (curRes.ResourceExists)
                    {
                        currentPrompt.Append($"{curRes.Order + 1}.Add a Data Source for {curRes.ResourceType} ");
                    }
                    else
                    {
                        currentPrompt.Append($"{curRes.Order + 1}.Create a {curRes.ResourceType} ");
                    }

                    currentPrompt.Append($"with default name {curRes.Name} in the default location {curRes.Location}.");
                    if (!curRes.ResourceType.Equals("Resource Group", StringComparison.InvariantCultureIgnoreCase))
                    {

                        currentPrompt.Append($"In the resource group {curRes.ResourceGroup}.");
                    }

                    if (curRes.Properties?.Count() > 0)
                    {
                        foreach (var prop in curRes.Properties)
                        {
                            currentPrompt.Append($"With property '{prop.Key}' defaulted to '{prop.Value}'.");
                        }
                    }

                    currentPrompt.Append($" With tags {string.Join(",", curRes.Tags)}.");
                    gptPrompt.Add(currentPrompt.ToString());
                }

                gptPrompt.Add("Generate response in three files provider,variables and main file");

                

                return (true, gptPrompt);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
