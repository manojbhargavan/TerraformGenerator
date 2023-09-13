using TerraformGenerator.Utils;

namespace TerraformGenerator.Business.Azure
{
    public class AzureConsoleInputProvider : IAzureInputProvider
    {
        public List<AzureResourceDescriptor> GetAzureResourceDescriptor()
        {
            int order = 0;
            List<AzureResourceDescriptor> resourcesToCreate = new();

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

            resourcesToCreate.Add(new AzureResourceDescriptor()
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
                if (string.IsNullOrEmpty(resourceName)) resourceName = $"{resourceType.GetResourceTypeShort()}".GetRandomName(10);
                resourceName = resourceName.Replace("-", "");
                
                string resourceLocation = $"{resourceType} Location".GetUserInputString()!;
                if (string.IsNullOrEmpty(resourceLocation)) resourceLocation = "eastus";

                List<KeyValuePair<string, string>> props = new();
                bool addAdditionalProperties = $"Want to add additional settings for {resourceName} {resourceType}?".GetUserInputBoolean();
                while (addAdditionalProperties)
                {
                    string description = "Description of the Property".GetUserInputString()!;
                    if (string.IsNullOrEmpty(description)) break;
                    string defaultValue = $"Default Value for {description}".GetUserInputString()! ?? "".GetRandomName(5);
                    props.Add(new KeyValuePair<string, string>(description, defaultValue));
                    addAdditionalProperties = $"Want to add additional settings for {resourceName} {resourceType}?".GetUserInputBoolean();
                }

                // Add to resources to create
                resourcesToCreate.Add(new AzureResourceDescriptor()
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

            return resourcesToCreate;
        }
    }
}