namespace Client
{
    internal class CollectInput
    {
        public static string? groupInput;

        public static string CollectString(string type)
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
