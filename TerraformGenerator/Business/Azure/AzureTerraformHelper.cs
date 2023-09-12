using LoremNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraformGenerator.Utils;

namespace TerraformGenerator.Business.Azure
{
    internal class AzureTerraformHelper : ITerraformHelper
    {
        public List<AzureResourceDescriptor> ResourcesToCreate { get; private set; }
        public string OutputFolder { get; set; }
        public bool Initialized { get; private set; } = false;
        private bool _resourceGroupExists = false;
        private readonly List<string> _azureResourceType = new() { "Storage Account", "Virtual Machine",
            "Virtual Network", "Subnet", "Firewall", "Others" };

        private void Initialize()
        {
            var outputFolder = "Output Folder".GetUserInputString(mandatory: true);
            if (string.IsNullOrWhiteSpace(outputFolder)) return;

            OutputFolder = outputFolder;

            // Resources
            PrepareResourceDescriptors();
        }

        public (bool, List<string>) GenerateTerraformPrompt()
        {
            List<string> gptPrompt = new();
            Initialize();
            if (!Initialized) return (Initialized, gptPrompt);

            // Create Prompt
            gptPrompt.Add("You are a code generator, you will reply with brief to-the-point " +
                "code easily parsable to split to different file with split indicator '~FN~filename' no styling");
            gptPrompt.Add("Create a terraform script using the latest Azure provider with the following resources: ");
            foreach (var curRes in ResourcesToCreate.OrderBy(r => r.Order))
            {
                StringBuilder currentPrompt = new("");
                currentPrompt.Append($"{curRes.Order + 1}. Create a {curRes.ResourceType} ");
                currentPrompt.Append($"with default name {curRes.Name} in the default location {curRes.Location}. ");
                if(!curRes.ResourceType.Equals("Resource Group", StringComparison.InvariantCultureIgnoreCase))
                {
                    currentPrompt.Append($" In the resource group {curRes.ResourceGroup}. ");
                }
                currentPrompt.AppendLine($" With tags {string.Join(",", curRes.Tags)}.");

                gptPrompt.Add(currentPrompt.ToString());
            }

            gptPrompt.Add("Generate response in three section for provider, variables and main file");
            return (Initialized, gptPrompt);
        }

        public void PrepareResourceDescriptors()
        {
            int order = 0;
            ResourcesToCreate = new();

            // Tags
            List<string> tags = new();
            bool addMoreTags = "Add Tags".GetUserInputBoolean();
            while (addMoreTags)
            {
                var tag = "Enter tag name and value (name|value), press enter to skip".GetUserInputString();
                Console.WriteLine($"Value: '{tag}'");
                if (!string.IsNullOrWhiteSpace(tag))
                    tags.Add(tag);
                else
                    addMoreTags = false;
            }

            // Location
            var location = "Azure Location".GetUserInputString() ?? "eastus";

            // Resource Group            
            var rgNameFunc = () =>
            {
                var rgTemp = "Resource Group Name".GetUserInputString();
                string rgName = "";
                if (string.IsNullOrWhiteSpace(rgTemp))
                {
                    _resourceGroupExists = false;
                    rgName = "-rg".GetRandomName(10);
                }
                else
                {
                    rgName = rgTemp;
                    _resourceGroupExists = "Resource Group Exists".GetUserInputBoolean();
                }

                return rgName;
            };
            var rgName = rgNameFunc();

            ResourcesToCreate.Add(new AzureResourceDescriptor()
            {
                Order = order++,
                ResourceType = "Resource Group",
                Location = location,
                Name = rgName,
                Tags = tags
            });



            // Lastly
            Initialized = true;
        }
    }
}
