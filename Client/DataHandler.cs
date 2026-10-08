using static Client.MessageQueue;

namespace Client
{
    internal class DataHandler
    {
        public const string path = @"C:\Dev\TrainToDev\SignalRProject\data.txt";

        public static void handleDataOnStartUp()
        {
            Console.WriteLine("");
            Console.WriteLine("How would like to Proceed?");
            Console.WriteLine("Press 'D' to start a new run");
            Console.WriteLine("Press 'C' to continue a existing run");

            var input = Console.ReadKey().Key;

            switch (input)
            {
                case ConsoleKey.D:
                    DeleteDataFile();

                    break;

                case ConsoleKey.C:
                    LoadDataFileIntoQueue();

                    break;

                default:
                    Console.WriteLine("Key not supported");
                    Console.WriteLine("Exiting...");
                    Console.ReadKey();
                    Environment.Exit(0);

                    break;
            }

        }

        private static void LoadDataFileIntoQueue()
        {
            var content = File.ReadAllLines(path);

            foreach (string line in content)
            {
                queue.Enqueue(line);
            }
        }

        private static void DeleteDataFile()
        {
            File.Delete(path);

            if (File.Exists(path))
            {
                Console.WriteLine("File has not been deleted");
                Console.WriteLine("Exiting...");
                Console.ReadKey();
                Environment.Exit(0);
            }

        }
    }
}
