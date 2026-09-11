using Microsoft.AspNetCore.SignalR;

namespace CentralSystem.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendToEveryone(string user, string message)
        {
            Console.WriteLine("[{0}] Sent: {1}\n", user, message);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}