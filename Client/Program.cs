using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        private static List<HubConnection> connections = new();
        private static List<string> queue = new();
        private static List<string> users = new();
        private static string groupInput;
        private static string clientInput;
        static CancellationTokenSource source = new CancellationTokenSource();
        static CancellationToken token = source.Token;

        static async Task Main(string[] args)
        {
            Console.WriteLine("SignalR w/ Queue Training Project");

            groupInput = CollectString("Group");

            if (!CheckStringNotNull(groupInput))
                return;

            clientInput = CollectString("Client");

            if (!CheckStringNotNull(clientInput))
                return;

            await ConnectClientToCentral(clientInput, groupInput);

            var task = ProcessMessageQueue(1000, token);

            // Can I wait for ChatHub response so i can send this
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            source.Cancel();
            task.Wait();

            Console.Clear();

            await DisconnectClientsFromCentral(groupInput);

            Console.WriteLine("Press 'Any Key' to exit");
            Console.ReadKey();
        }

        private static string CollectString(string type)
        {
            Console.WriteLine();
            Console.WriteLine($"Type {type} Name:");

 return Console.ReadLine() ?? "";
        }

        private static bool CheckStringNotNull(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                InvalidValuePrint();
                return false;
            }

            return true;
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

                queue.Add(updatedMessage);
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
        }

        private static void InvalidValuePrint()
        {
            Console.Clear();
            Console.WriteLine("Warning: Must provide one value!");
            Console.WriteLine();
            Console.WriteLine("Press 'Any Key' to exit");
            Console.ReadKey();
        }

        private static async Task ProcessMessageQueue(int ms, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = queue.First();
                queue.Remove(message);

                Console.WriteLine(message);

                await Task.Delay(ms);
            }
        }
    }
}