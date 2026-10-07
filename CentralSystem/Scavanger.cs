using CentralSystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Globalization;

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
            var task1 = SendNotificationsToClients(1000, stoppingToken);
            var task2 = CheckLoggedMessages(stoppingToken);

            Task.WaitAll(task1, task2);
        }

        private async Task SendNotificationsToClients(int ms, CancellationToken stoppingToken)
        {
            int i = 0;
            Random random = new Random();
            RegionInfo region = new RegionInfo("US");

            while (!stoppingToken.IsCancellationRequested)
            {
                var users = User.GetUsers().ToList();

                foreach (var user in users)
                {
                    if (!User.IsItEmpty())
                        return;

                    i++;

                    string Message = $"Hi {user.Name}! Transaction #{i} of {region.CurrencySymbol}{random.Next(0, 1000)}.{random.Next(10, 99)} has been sent to your account.";
                    await _hubContext.Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", user.Group, Message);

                    LoggedMessages.TryAdd(Guid.NewGuid(), new Message { Name = user.Name, Description = Message });

                };

                await Task.Delay(ms);
            }
        }

        private async Task CheckLoggedMessages(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (User user in User.GetUsers())
                {
                    Console.WriteLine($"[{user.Group}] Number os Messages for {user.Name}: {LoggedMessages.Count(lm => lm.Value.Name == user.Name)}");
                };

                await Task.Delay(10000);
            }
        }
    }
}