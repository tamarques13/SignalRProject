using Microsoft.AspNetCore.SignalR;

namespace CentralSystem.Hubs
{
    public class ChatHub : Hub
    {
        public async Task OnConnectedAsync(string username, string groupName)
        {
            string message = $"User {username} has connected.";

            User.AddUser(Context.ConnectionId, username, groupName);
  
            //await Clients.Others.SendAsync("ReceiveMessage", "Server" , message);
            Console.WriteLine(message);
        }

        // Note:
        // Not being used
        public async Task OnDisconnectedAsync(string username)
        {
            string message = $"User {username} has disconnected.";

            User.RemoveUser(Context.ConnectionId);

            await Clients.Others.SendAsync("ReceiveMessage", "Server", message);
            Console.WriteLine(message);
        }

        public async Task JoinGroup(string groupName, string name)
        {
            string message = $"<name> has joined the group {groupName}.";
            
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            //await Clients.Group(groupName).SendAsync("ReceiveMessage", "Server", message);
            Group.AddGroup(groupName);

            // Doubt:
            // Is it better to do a find in User.GetUsers() with ConnectionId
            // Or receive directly through params

            Console.WriteLine(message.Replace("<name>", name));
        }

        // Note:
        // Not being used
        public async Task SendToEveryone(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            Console.WriteLine("[{0}] Sent: {1}\n", user, message);
        }
    }
}