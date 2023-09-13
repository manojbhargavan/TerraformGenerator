using System.Collections;

namespace TerraformGenerator.Business.Azure
{
    public interface IAzureInputProvider
    {
        List<AzureResourceDescriptor> GetAzureResourceDescriptor();
    }
}