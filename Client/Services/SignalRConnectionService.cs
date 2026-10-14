using Microsoft.AspNetCore.SignalR.Client;
using Client.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Client.Services
{
    internal class SignalRConnectionService(IMessageQueueService messageQueue, ILogger<SignalRConnectionService> logger) : ISignalRConnectionService
    {
        private readonly IMessageQueueService _messageQueueService = messageQueue;
        private readonly ILogger<SignalRConnectionService> _logger = logger;
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

            try
            {
                connections.Add(connection);

                await connection.StartAsync();
                await connection.InvokeAsync("OnConnectedAsync", clientInput, groupName);

                await connection.InvokeAsync("JoinGroup", groupName, clientInput);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to connect client {clientInput}");
            }
        }

        public async Task DisconnectClientsFromCentral(string groupName)
        {
            foreach (var connection in connections)
            {
                try
                {
                    await connection.InvokeAsync("LeaveGroup", groupName);
                    await connection.InvokeAsync("OnDisconnectedAsync");
                    await connection.StopAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to disconnect client connection");
                }
                finally
                {
                    await connection.DisposeAsync();
                    connections.Remove(connection);
                }
            };

            Console.WriteLine("Press 'Any Key' to exit");
            Console.ReadKey();
        }
    }
}