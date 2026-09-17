using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        private static List<HubConnection> connections = new();
        private static List<string> users = new();
        private static string groupInput;

        static async Task Main(string[] args)
        {
            Console.WriteLine("SignalR Training Project");

            groupInput = CollectGroup();

            if (!CheckStringNotNull(groupInput))
                return;

            CollectClients();

            if (!CheckListHasValues(users)) 
                return;

            await ConnectClientToCentral(groupInput);

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

        private static void CollectClients()
        {
            string input;

            do
            {
                Console.Clear();
                Console.WriteLine("Type 'x' to Contine...");
                Console.WriteLine();

                Console.WriteLine($"Clients added: {users.Count}");
                Console.WriteLine("Type Client Name:");

                input = Console.ReadLine() ?? "Unknown";

                if (!input.Equals("x")) 
                    users.Add(input);

            } while (!input.Equals("x"));
        }

        private static bool CheckListHasValues(List<string> values)
        {
            if (values.Count < 1)
            {
                InvalidValuePrint();
                return false;
            }

            return true;
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

        private static async Task ConnectClientToCentral(string groupName)
        {
            Console.Clear();

            foreach (var user in users)
            {
                var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chatHub")
                .Build();

                connection.On("ReceiveMessage", (string userName, string message) =>
                {
                    Console.WriteLine(userName + ':' + message.Replace("<name>", user));
                });

                connections.Add(connection);

                await connection.StartAsync();
                await connection.InvokeAsync("OnConnectedAsync", user, groupName);

                await connection.InvokeAsync("JoinGroup", groupName, user);
            };
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