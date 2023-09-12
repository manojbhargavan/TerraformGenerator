namespace TerraformGenerator.Business
{
    internal interface ITerraformHelper
    {
        (bool promptGenerated, List<string> promptList) GenerateTerraformPrompt(string runId);
    }
}