using System.Collections.Concurrent;

namespace Client
{
    internal class MessageQueue
    {
        public static ConcurrentQueue<string> queue = new();

        public static CancellationTokenSource source = new CancellationTokenSource();
        public static CancellationToken token = source.Token;

        public static async Task ProcessMessageQueue(int ms, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                queue.TryDequeue(out string? message);

                Console.WriteLine(message);

                await Task.Delay(ms);
            }
        }

        public static void CancelProcessMessageQueue(Task task)
        {
            Console.ReadKey();
            source.Cancel();
            task.Wait();
            Console.Clear();
        }
    }
}
