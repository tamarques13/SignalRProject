using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        private static List<HubConnection> connections = new();
        private static List<string> users = new();
        private static string groupInput;
        private static string clientInput;

        static async Task Main(string[] args)
        {
            Console.WriteLine("SignalR Training Project");

            groupInput = CollectGroup();

            if (!CheckStringNotNull(groupInput))
                return;

            clientInput = CollectClient();

            if (!CheckStringNotNull(clientInput))
                return;

            await ConnectClientToCentral(clientInput, groupInput);

            Console.ReadKey();
            Console.Clear();

            await DisconnectClientsFromCentral(groupInput);

            Console.WriteLine();
            Console.WriteLine("Press 'Any Key' to exit");
            Console.ReadKey();
        }

        private static string CollectGroup()
        {
            Console.WriteLine();
            Console.WriteLine("Type Group Name:");
            return Console.ReadLine() ?? "";
        }

        private static string CollectClient()
        {
            Console.WriteLine();
            Console.WriteLine("Type Client Name:");
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
                Console.WriteLine(userName + ':' + message.Replace("<name>", clientInput));
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
    }
}