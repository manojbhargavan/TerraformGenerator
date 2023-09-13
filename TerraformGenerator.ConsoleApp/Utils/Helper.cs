namespace TerraformGenerator.Utils
{
    public static class Helper
    {
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
    }
}
