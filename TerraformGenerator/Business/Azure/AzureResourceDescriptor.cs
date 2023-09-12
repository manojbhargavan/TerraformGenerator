using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraformGenerator.Business.Azure
{
    internal class AzureResourceDescriptor : IResourceDescriptor
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
