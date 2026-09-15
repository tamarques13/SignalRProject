using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace CentralSystem.Hubs
{
    public class ChatHub : Hub
    {
        public async Task OnConnectedAsync(string username)
        {
            string message = $"User {username} has connected.";

            User.AddUser(Context.ConnectionId, username);

            await Clients.Others.SendAsync("ReceiveMessage", "Server" , message);
            Console.WriteLine(message);
        }

        public async Task OnDisconnectedAsync(string username)
        {
            string message = $"User {username} has disconnected.";

            User.RemoveUser(Context.ConnectionId);

            await Clients.Others.SendAsync("ReceiveMessage", "Server", message);
            Console.WriteLine(message);
        }

        public async Task SendToEveryone(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            Console.WriteLine("[{0}] Sent: {1}\n", user, message);
        }
    }
}