using CentralSystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
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
                foreach (var group in Group.GetGroups())
                {
                    string groupMessage = "Hi <name>! Check the new post added!";
                    await _hubContext.Clients.Group(group).SendAsync("ReceiveMessage", group, groupMessage);

                    foreach (var u in User.GetUsers().Where(u => u.Group == group))
                    {
                        LoggedMessages.TryAdd(Guid.NewGuid(), new Message { Name = u.Name, Description = groupMessage.Replace("<name>", u.Name)});
                    };
                 };

                await Task.Delay(3000);
            }
        }

        private async Task CheckLoggedMessages(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach(User user in User.GetUsers())
                { 
                    Console.WriteLine($"[{user.Group}] Number os Messages for {user.Name}: {LoggedMessages.Count(lm => lm.Value.Name == user.Name)}");
                };

                await Task.Delay(10000);
            }
        }
    }
}