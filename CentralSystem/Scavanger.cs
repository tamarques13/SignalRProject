using CentralSystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace CentralSystem
{
    public class Scavenger : BackgroundService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public static ConcurrentDictionary<Guid, Message> LoggedMessages = new();

        public Scavenger(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var task1 = SendNotificationsToClients(stoppingToken);
            var task2 = CheckLoggedMessages(stoppingToken);

            Task.WaitAll(task1, task2);
        }

        private async Task SendNotificationsToClients(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                User.GetUsers().ToList().ForEach(u =>
                {
                    string message = $"Hi {u.Name}! New Post was added!";

                    _hubContext.Clients.Client(u.ConnectionId).SendAsync("ReceiveMessage", "Server", message);
                    LoggedMessages.TryAdd(Guid.NewGuid(), new Message { Name = u.Name, Description = message });

                });

                await Task.Delay(1500);
            }
        }

        private async Task CheckLoggedMessages(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                User.GetUsers().ToList().ForEach(u =>
                {
                    string message = $"Hi {u.Name}! New Post was added!";

                    Console.WriteLine($"Number os Messages for {u.Name}:  {LoggedMessages.Count(lm => lm.Value.Name == u.Name && lm.Value.Description == message)}");
                });

                await Task.Delay(5000);
            }
        }
    }
}