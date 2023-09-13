using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraformGenerator.Lib
{
    public interface ITerraformHelper
    {
        (bool promptGenerated, List<string> promptList) GenerateTerraformPrompt(string runId);
    }
}
