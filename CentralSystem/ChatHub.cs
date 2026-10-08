using Microsoft.AspNetCore.SignalR;

namespace CentralSystem.Hubs
{
    public class ChatHub : Hub
    {
        public async Task OnConnectedAsync(string username, string groupName)
        {
            string message = $"User {username} has connected.";

            User.AddUser(Context.ConnectionId, username, groupName);

            // Disable Message to Client
            //await Clients.Caller.SendAsync("ReceiveMessage", "Server", message);

            Console.WriteLine(message);
        }

        public async Task OnDisconnectedAsync()
        {
            string user = User.GetUsers()
                              .ToList()
                              .Find(user => user.ConnectionId == Context.ConnectionId)?
                              .Name ?? throw new InvalidOperationException("User not found.");

            string message = $"User {user} has disconnected.";

            Console.WriteLine(message);
            User.RemoveUser(Context.ConnectionId);

            await Clients.Caller.SendAsync("ReceiveMessage", "Server", message);
        }

        public async Task JoinGroup(string groupName, string name)
        {
            string message = $"{name} has joined the group {groupName}.";

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // Disable Message to Client
            //await Clients.Caller.SendAsync("ReceiveMessage", "Server", message);

            Group.AddGroup(groupName);

            Console.WriteLine(message);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}