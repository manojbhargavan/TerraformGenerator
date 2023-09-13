namespace TerraformGenerator.Business.Azure
{
    public class AzureResourceDescriptor : IResourceDescriptor
    {
        public int Order { get; set; }
        public bool ResourceExists { get; set; }
        public string ResourceType { get; set; }
        public string Name { get; set; }
        public string ResourceGroup { get; set; }
        public List<string> Tags { get; set; }
        public string Location { get; set; }
        public List<KeyValuePair<string, string>> Properties { get; set; }
    }
}
