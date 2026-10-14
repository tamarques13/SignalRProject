using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CentralSystem.Hubs
{
    public class ChatHub(ILogger<ChatHub> logger) : Hub
    {
        private readonly ILogger<ChatHub> _logger = logger;

        public async Task OnConnectedAsync(string username, string groupName)
        {
            try
            {
            string message = $"User {username} has connected.";

            User.AddUser(Context.ConnectionId, username, groupName);

            Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to add user");
            }
        }

        public async Task OnDisconnectedAsync()
        {
            try
            {
                string user = User.GetUsers()
                                   .ToList()
                                   .Find(user => user.ConnectionId == Context.ConnectionId)?
                                   .Name ?? throw new InvalidOperationException("User not found.");

                string message = $"User {user} has disconnected.";
                User.RemoveUser(Context.ConnectionId);
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Something happened while disconnecting user");
            }
        }

        public async Task JoinGroup(string groupName, string name)
        {
            try
            {
                string message = $"{name} has joined the group {groupName}.";

                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                Group.AddGroup(groupName);

                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to Add user to group");
            }
        }

        public async Task LeaveGroup(string groupName)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to Remove user from group");
            }
        }
    }
}