using Client.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Client.Services
{
    internal class MessageQueueService(ILogger<MessageQueueService> logger) : IMessageQueueService
    {
        private readonly ILogger<MessageQueueService> _logger = logger;
        private static ConcurrentQueue<string> queue = new();
        private static CancellationTokenSource source = new CancellationTokenSource();

        public async Task ProcessMessageQueue(int ms, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    queue.TryDequeue(out string? message);

                    Console.WriteLine(message);

                    await Task.Delay(ms);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Unable to process queue message");
                }

            }
        }

        public void CancelProcessMessageQueue(Task task)
        {
            try
            {
                Console.ReadKey();
                source.Cancel();
                task.Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while stopping message queue processing.");
            }
            finally
            {
                Console.Clear();
            }
        }

        public ConcurrentQueue<string> GetQueue()
        {
            return queue;
        }

        public CancellationTokenSource GetCancellationTokenSource()
        {
            return source;
        }
    }
}
