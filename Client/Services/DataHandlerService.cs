using Client.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Client.Services
{
    internal class DataHandlerService(IMessageQueueService messageQueue, ILogger<DataHandlerService> logger ) : IDataHandlerService
    {
        private readonly IMessageQueueService _messageQueue = messageQueue;
        private readonly ILogger<DataHandlerService> _logger = logger;
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
            try
            {
                var content = File.ReadAllLines(path);

                foreach (string line in content)
                {
                    _messageQueue.GetQueue().Enqueue(line);
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, $"File not found: {path}");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, $"Failed to read file: {path}");
            }
        }

        private void DeleteDataFile()
        {
            try
            {
                if (!File.Exists(path))
                {
                    _logger.LogWarning($"File does not exist: {path}");
                }

                File.Delete(path);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, $"Failed to delete file: {path}");
            }
        }
    }
}
