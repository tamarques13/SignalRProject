using Microsoft.AspNetCore.SignalR;

namespace CentralSystem.Hubs
{
    public class ChatHub : Hub
    {
        public async Task OnConnectedAsync(string username)
        {
            User user = new User(username);

            await Clients.All.SendAsync("ReceiveMessage", $"Connection {username} has been added");
            Console.WriteLine($"User: {username} has been Connected.");
        }

        public async Task SendToEveryone(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            Console.WriteLine("[{0}] Sent: {1}\n", user, message);
        }
    }
}