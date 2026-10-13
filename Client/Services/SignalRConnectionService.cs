using Microsoft.AspNetCore.SignalR.Client;
using Client.Services.Interfaces;

namespace Client.Services
{
    internal class SignalRConnectionService(IMessageQueueService messageQueue) : ISignalRConnectionService
    {
        private readonly IMessageQueueService _messageQueueService = messageQueue;
        private static List<HubConnection> connections = new();

        public async Task ConnectClientToCentral(string clientInput, string groupName)
        {
            Console.Clear();

            var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/chatHub")
            .Build();

            connection.On("ReceiveMessage", (string userName, string message) =>
            {
                var updatedMessage = $"[{userName}]" + ": " + message.Replace("<name>", clientInput);

                _messageQueueService.GetQueue().Enqueue(updatedMessage);
            });

            connections.Add(connection);

            await connection.StartAsync();
            await connection.InvokeAsync("OnConnectedAsync", clientInput, groupName);

            await connection.InvokeAsync("JoinGroup", groupName, clientInput);
        }

        public async Task DisconnectClientsFromCentral(string groupName)
        {
            foreach (var connection in connections)
            {
                await connection.InvokeAsync("LeaveGroup", groupName);
                await connection.InvokeAsync("OnDisconnectedAsync");
            }
            ;

            connections.Clear();
            Console.WriteLine("Press 'Any Key' to exit");
            Console.ReadKey();
        }
    }
}
