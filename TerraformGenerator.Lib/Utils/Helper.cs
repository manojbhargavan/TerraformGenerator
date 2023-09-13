using LoremNET;

namespace TerraformGenerator.Lib.Utils
{
    public static class Helper
    {
        static readonly string GptPromptFileNameSep = "~";
        static readonly IEnumerable<string> GptResponseJunk = new List<string> { "```hcl", "```terraform", "```" };

        public static string GetRandomName(this string postfix, int lengthLimit)
        {
            var name = $"{Lorem.Words(1, false, false)}{Guid.NewGuid().ToString().Replace("-", "")}{postfix}";
            return name.Substring(name.Length - lengthLimit);
        }

        public static string GetCommonPrompt(this string runId)
        {
            return "You are a code gen, you will reply with brief to-the-point " +
                $"code easily parsable to split to different files with file start indicator '{GetFileNameIndicator(runId)}'" +
                $"{Environment.NewLine}Create a terraform script using the latest Azure provider with the following resources:";
        }

        public static string GetFileNameIndicator(string runId)
        {
            return $"{runId}{GptPromptFileNameSep}filename{GptPromptFileNameSep}";
        }

        public static IEnumerable<FileInfo> ParseResponseAndWrite(this string response, string runId, string outputDirectory)
        {

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            List<FileInfo> result = new();
            var fileContent = response.Split(runId).ToList();
            fileContent = fileContent.Where(f => !string.IsNullOrWhiteSpace(f)).ToList();
            foreach (var file in fileContent)
            {
                using (var reader = new StringReader(file))
                {
                    string fileName = reader.ReadLine()?.Replace(GptPromptFileNameSep, "")!;
                    if (fileName == null) { continue; }

                    string currentFileContent = reader.ReadToEnd();
                    currentFileContent = currentFileContent.CleanUpGptResponseJunk();
                    string fullFileName = Path.Join(outputDirectory, fileName);
                    File.WriteAllText(fullFileName, currentFileContent);
                    result.Add(new FileInfo(fullFileName));
                }
            }

            return result;
        }

        public static string CleanUpGptResponseJunk(this string response)
        {
            foreach (var junk in GptResponseJunk)
            {
                response = response.Replace(junk, "");
            }
            return response;
        }

        public static string GetResourceTypeShort(this string type)
        {
            string result = string.Empty;
            string[] types = type.Split(' ');
            foreach (var t in types)
            {
                result += t[0];
            }
            return result.ToLower();
        }
    }
}
