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
        public bool Initialized { get; private set; } = false;

        public (bool, List<string>) GenerateTerraformPrompt(string runId)
        {
            List<string> gptPrompt = new();
            PrepareResourceDescriptors();
            if (!Initialized) return (Initialized, gptPrompt);

            // Create Prompt
            gptPrompt.Add(runId.GetCommonPrompt());
            foreach (var curRes in ResourcesToCreate.OrderBy(r => r.Order))
            {
                StringBuilder currentPrompt = new("");
                if(curRes.ResourceExists)
                {
                    currentPrompt.Append($"{curRes.Order + 1}.Add a Data Source for {curRes.ResourceType} ");
                }
                else
                {
                    currentPrompt.Append($"{curRes.Order + 1}.Create a {curRes.ResourceType} ");
                }
                
                currentPrompt.Append($"with default name {curRes.Name} in the default location {curRes.Location}.");
                if(!curRes.ResourceType.Equals("Resource Group", StringComparison.InvariantCultureIgnoreCase))
                {
                    
                    currentPrompt.Append($"In the resource group {curRes.ResourceGroup}.");
                }

                if(curRes.Properties?.Count() > 0)
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
            return (Initialized, gptPrompt);
        }

        public void PrepareResourceDescriptors()
        {
            int order = 0;
            ResourcesToCreate = new();

            // Tags
            List<string> tags = new();
            bool addMoreTags = "Add Resource Tags".GetUserInputBoolean();
            while (addMoreTags)
            {
                var tag = "tag name and value (name|value), press enter to skip".GetUserInputString();
                if (!string.IsNullOrWhiteSpace(tag))
                    tags.Add(tag);
                else
                    addMoreTags = false;
            }

            // Location
            var location = "Azure Location".GetUserInputString() ?? "eastus";

            // Resource Group
            bool _resourceGroupExists = false;
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
                Tags = tags,
                ResourceExists = _resourceGroupExists
            });

            // Add Other Resources
            bool createNewResourceScript = "Want to add another resource?".GetUserInputBoolean();
            while (createNewResourceScript)
            {
                bool resourceExists = "Resource Exists".GetUserInputBoolean();
                string resourceType = "Resource Type (like Storage Account, Web App, Function App), default to Storage Account".GetUserInputString(true)! ?? "Storage Account";
                string resourceName = $"{resourceType} Name".GetUserInputString()!;
                if (string.IsNullOrEmpty(resourceName)) resourceName = $"-{resourceType.GetResourceTypeShort()}".GetRandomName(10);
                string resourceLocation = $"{resourceType} Location".GetUserInputString() ?? "eastus";
                List<KeyValuePair<string, string>> props = new();
                bool addAdditionalProperties = $"Want to add additional settings for {resourceName} {resourceType}?".GetUserInputBoolean();
                while(addAdditionalProperties)
                {
                    string description = "Description of the Property".GetUserInputString()!;
                    if (string.IsNullOrEmpty(description)) break;
                    string defaultValue = $"Default Value for {description}".GetUserInputString()! ?? "".GetRandomName(5);
                    props.Add(new KeyValuePair<string, string>(description, defaultValue));
                    addAdditionalProperties = $"Want to add additional settings for {resourceName} {resourceType}?".GetUserInputBoolean();
                }

                ResourcesToCreate.Add(new AzureResourceDescriptor()
                {
                    Order = order++,
                    ResourceType = resourceType,
                    Location = resourceLocation,
                    Name = resourceName,
                    Tags = tags,
                    ResourceExists = resourceExists,
                    ResourceGroup = rgName,
                    Properties = props
                });

                createNewResourceScript = "Want to add another resource?".GetUserInputBoolean();
            } 


            // Lastly
            Initialized = true;
        }
    }
}
