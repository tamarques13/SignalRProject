using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chatHub")
                .Build();

            connection.On("ReceiveMessage", (string userName, string message) =>
            {
                Console.WriteLine(userName + ':' + message);
            });

            Console.WriteLine("Type UserName:");

            string userName = Console.ReadLine() ?? "";

            Console.WriteLine("Type GroupName:");

            string groupName = Console.ReadLine() ?? "";

            connection.StartAsync();
            connection.InvokeAsync("OnConnectedAsync", userName, groupName);
            Console.WriteLine("SignalR Connected");

            connection.InvokeAsync("JoinGroup", groupName, userName);


            //while (true)
            //{
            //    string message = Console.ReadLine();

            //    connection.InvokeCoreAsync("SendToEveryone", args: [userName, message]);

            //}

            Console.ReadKey();
            connection.InvokeAsync("OnDisconnectedAsync", userName);

            Console.ReadKey();
        }
    }
}