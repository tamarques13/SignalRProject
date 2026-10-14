using Client.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Client.Services
{
    internal class CollectInputService(ILogger<CollectInputService> logger) : ICollectInputService
    {
        private readonly ILogger<CollectInputService> _logger = logger;
        private static string? groupInput;

        public string CollectString(string type)
        {

            Console.Clear();
            string result;


            Console.WriteLine($"Type {type} Name:");

            result = Console.ReadLine() ?? "";

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
            return groupInput ?? "";
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
