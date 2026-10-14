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

            try
            {
                Console.WriteLine($"Type {type} Name:");

                result = Console.ReadLine() ?? "";

                if (!CheckStringNotNull(result))
                {
                    Environment.Exit(0);
                }

                return result;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Unable to collect user input");
                throw;
            }
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
