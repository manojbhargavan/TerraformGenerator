namespace TerraformGenerator.Lib.Providers.Azure
{
    public interface IAzureInputProvider
    {
        List<AzureResourceDescriptor> GetAzureResourceDescriptor();
    }
}
