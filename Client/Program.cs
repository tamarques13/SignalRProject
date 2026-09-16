using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input;
            string groupInput;
            List<string> users = new();

            Console.WriteLine("Type Group Name:");
            groupInput = Console.ReadLine() ?? "";

            do
            {
                Console.WriteLine("Type Client Name:");
                input = Console.ReadLine() ?? "";

                if (input != "x")
                    users.Add(input);

            } while (input.ToLower() != "x");

            users.ForEach(user =>
            {
                _ = ConnectClientToCentral(user, groupInput);
            });

            Console.ReadKey();
        }

        private static async Task ConnectClientToCentral(string user, string groupName)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chatHub")
                .Build();

            connection.On("ReceiveMessage", (string userName, string message) =>
            {
                Console.WriteLine(userName + ':' + message.Replace("<name>", user));
            });

            await connection.StartAsync();
            await connection.InvokeAsync("OnConnectedAsync", user, groupName);
            Console.WriteLine("SignalR Connected");

            await connection.InvokeAsync("JoinGroup", groupName, user);

            //Console.ReadKey();
            //await connection.InvokeAsync("OnDisconnectedAsync", userName);
        }

    }
}