using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace CentralSystem.Hubs
{
    public class ChatHub : Hub
    {
        public static ConcurrentDictionary<string, User> Users = new();

        public async Task OnConnectedAsync(string username)
        {
            User.AddUser(Context.ConnectionId, username);

            await Clients.Others.SendAsync("ReceiveMessage", "Server" ,$"User {username} has connected.");
            Console.WriteLine($"User: {username} has been Connected.");
        }

        public async Task SendToEveryone(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            Console.WriteLine("[{0}] Sent: {1}\n", user, message);
        }
    }
}