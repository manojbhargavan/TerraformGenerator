using LoremNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraformGenerator.Business.Azure;

namespace TerraformGenerator.Utils
{
    internal static class Helper
    {
        static readonly string GptPromptFileNameSep = "~";
        static readonly IEnumerable<string> GptResponseJunk = new List<string> { "```hcl", "```terraform", "```" };

        public static string? GetUserInputString(this string inputPrompt, bool mandatory = false)
        {
            string? userInput;
            Console.Write($"Enter the {inputPrompt}: ");
            userInput = Console.ReadLine()!;
            if (!mandatory)
            {
                return userInput;
            }

            int tries = 0;
            while (string.IsNullOrEmpty(userInput))
            {
                Console.WriteLine($"Enter the {inputPrompt}, please: ");
                userInput = Console.ReadLine()!;
                tries++;
                if (tries == 3)
                {
                    Console.WriteLine("I give up...");
                    return null;
                }
            }
            return userInput;
        }

        public static bool GetUserInputBoolean(this string message)
        {
            bool result = false;
            Console.Write($"{message} (enter y for yes): ");
            string userInput = Console.ReadLine()!;

            if (!string.IsNullOrEmpty(userInput) && userInput.Equals("y", StringComparison.InvariantCultureIgnoreCase))
            {
                result = true;
            }

            return result;
        }

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
                    if(fileName == null) { continue; }

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
