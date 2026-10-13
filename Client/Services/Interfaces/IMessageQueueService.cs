using System.Collections.Concurrent;

namespace Client.Services.Interfaces
{
    public interface IMessageQueueService
    {
        Task ProcessMessageQueue(int ms, CancellationToken stoppingToken);
        void CancelProcessMessageQueue(Task task);
        ConcurrentQueue<string> GetQueue();
        CancellationTokenSource GetCancellationTokenSource();
    }
}
