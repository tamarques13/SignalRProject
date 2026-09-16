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
                foreach (var g in Group.GetGroups())
                {
                    string groupMessage = $"[{g}] New Post was added!" ;
                    await _hubContext.Clients.Group(g).SendAsync("ReceiveMessage", "Server", groupMessage);

                    foreach (var u in User.GetUsers().Where(u => u.Group == g))
                    {
                         string message = $"Hi {u.Name}! Check the new post added to {g}!";

                        await _hubContext.Clients.Client(u.ConnectionId).SendAsync("ReceiveMessage", "Server", message);
                         LoggedMessages.TryAdd(Guid.NewGuid(), new Message { Name = u.Name, Description = message });

                     };
                 };

                await Task.Delay(3000);
            }
        }

        private async Task CheckLoggedMessages(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                User.GetUsers().ToList().ForEach(u =>
                {
                    string message = $"Hi {u.Name}! Check the new post added to {u.Group}!";

                    Console.WriteLine($"Number os Messages for {u.Name}:  {LoggedMessages.Count(lm => lm.Value.Name == u.Name && lm.Value.Description == message)}");
                });

                await Task.Delay(10000);
            }
        }
    }
}