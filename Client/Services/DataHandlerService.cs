using Client.Services.Interfaces;

namespace Client.Services
{
    internal class DataHandlerService(IMessageQueueService messageQueue) : IDataHandlerService
    {
        private readonly IMessageQueueService _messageQueue = messageQueue;
        private const string path = @"C:\Dev\TrainToDev\SignalRProject\data.txt";

        public void HandleDataOnStartUp()
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

        public string GetDataPath()
        {
            return path;
        }

        private void LoadDataFileIntoQueue()
        {
            var content = File.ReadAllLines(path);

            foreach (string line in content)
            {
                _messageQueue.GetQueue().Enqueue(line);
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
