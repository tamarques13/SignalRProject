using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;

namespace Client
{
    internal class Program
    {
        private static List<HubConnection> connections = new();
        private static ConcurrentQueue<string> queue = new();
        static CancellationTokenSource source = new CancellationTokenSource();
        static CancellationToken token = source.Token;

        static async Task Main(string[] args)
        {
            Console.WriteLine("SignalR w/ Queue Training Project");

            string groupInput = CollectString("Group");
            string clientInput = CollectString("Client");

            await ConnectClientToCentral(clientInput, groupInput);
            var task = ProcessMessageQueue(1000, token);

            CancelProcessMessageQueue(task);
            
            await DisconnectClientsFromCentral(groupInput);
        }

        private static async Task ConnectClientToCentral(string clientInput, string groupName)
        {
            Console.Clear();

            var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/chatHub")
            .Build();

            connection.On("ReceiveMessage", (string userName, string message) =>
            {
                var updatedMessage = $"[{userName}]" + ": " + message.Replace("<name>", clientInput);

                queue.Enqueue(updatedMessage);
            });

            connections.Add(connection);

            await connection.StartAsync();
            await connection.InvokeAsync("OnConnectedAsync", clientInput, groupName);

            await connection.InvokeAsync("JoinGroup", groupName, clientInput);
        }

        private static async Task DisconnectClientsFromCentral(string groupName)
        {
            foreach (var connection in connections)
            {
                await connection.InvokeAsync("LeaveGroup", groupName);
                await connection.InvokeAsync("OnDisconnectedAsync");
            };

            connections.Clear();
            Console.WriteLine("Press 'Any Key' to exit");
            Console.ReadKey();
        }

        private static async Task ProcessMessageQueue(int ms, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                queue.TryDequeue(out string? message);

                Console.WriteLine(message);

                await Task.Delay(ms);
            }
        }

        private static void CancelProcessMessageQueue(Task task)
        {
            Console.ReadKey();
            source.Cancel();
            task.Wait();
            Console.Clear();
        }

        private static string CollectString(string type)
        {

            Console.WriteLine();
            Console.WriteLine($"Type {type} Name:");

            string result = Console.ReadLine() ?? "";

            if (!CheckStringNotNull(result))
                throw new ArgumentNullException("Value is Null");

            return result;
        }

        private static bool CheckStringNotNull(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Console.Clear();
                Console.WriteLine("Warning: Must provide one value!");
                Console.WriteLine();
                Console.WriteLine("Press 'Any Key' to exit");
                Console.ReadKey();
                return false;
            }

            return true;
        }
    }
}