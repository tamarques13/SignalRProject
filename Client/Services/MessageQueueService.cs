using Client.Services.Interfaces;
using System.Collections.Concurrent;

namespace Client.Services
{
    internal class MessageQueueService : IMessageQueueService
    {
        private static ConcurrentQueue<string> queue = new();

        private static CancellationTokenSource source = new CancellationTokenSource();

        public async Task ProcessMessageQueue(int ms, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                queue.TryDequeue(out string? message);

                Console.WriteLine(message);

                await Task.Delay(ms);
            }
        }

        public void CancelProcessMessageQueue(Task task)
        {
            Console.ReadKey();
            source.Cancel();
            task.Wait();
            Console.Clear();
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
