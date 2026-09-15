using CentralSystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace CentralSystem
{
    public class Scavenger : BackgroundService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public static ConcurrentDictionary<int, Message> LoggedMessages = new();
        public int i = 0;

        public Scavenger(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                User.GetUsers().ToList().ForEach(u =>
                {
                    _hubContext.Clients.Client(u.ConnectionId).SendAsync("ReceiveMessage", "Server", $"Hi {u.Name}! New Post was added!");
                    LoggedMessages.TryAdd(i++, new Message { Name = u.Name, Description = $"Hi {u.Name}! New Post was added!" });
                });

                if ((i % 5) == 0)
                    Console.WriteLine($"Number os Messages: {LoggedMessages.Count}");

                await Task.Delay(1000);
            }
        }
    }
}