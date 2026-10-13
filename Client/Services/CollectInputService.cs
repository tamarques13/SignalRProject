using Client.Services.Interfaces;

namespace Client.Services
{
    internal class CollectInputService : ICollectInputService
    {
        private static string? groupInput;

        public string CollectString(string type)
        {

            Console.Clear();
            Console.WriteLine($"Type {type} Name:");

            string result = Console.ReadLine() ?? "";

            if (!CheckStringNotNull(result))
            {
                Environment.Exit(0);
            }

            return result;
        }

        public void SetGroupInput(string value)
        {
            groupInput = value;
        }

        public string GetGroupInput()
        {
            return groupInput ?? throw new NullReferenceException();
        }

        private static bool CheckStringNotNull(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Console.Clear();
                Console.WriteLine("Warning: Must provide one value!");
                Console.WriteLine();
                Console.WriteLine("Press 'Any Key' to exit");
                Console.ReadKey();
                return false;
            }

            return true;
        }
    }
}
