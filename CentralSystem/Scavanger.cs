using CentralSystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace CentralSystem
{
    public class Scavenger : BackgroundService
    {
        private readonly IHubContext<ChatHub> _hubContext;
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
                });

                await Task.Delay(3000);
            }
        }
    }
}