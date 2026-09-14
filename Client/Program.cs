using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string userName = "";

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chatHub")
                .Build();

            connection.On("ReceiveMessage", (string userName, string message) =>
            {
                Console.WriteLine(userName + ':' + message);
            });

            try
            {
                Console.WriteLine("Type UserName:");

                do
                {
                    userName = Console.ReadLine() ?? "";
                }
                while (userName.Length == 0);

                connection.StartAsync();
                connection.InvokeAsync("OnConnectedAsync", userName);
                Console.WriteLine("SignalR Connected");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            };

            //while (true)
            //{
            //    string message = Console.ReadLine();

            //    connection.InvokeCoreAsync("SendToEveryone", args: [userName, message]);

            //}

            Console.ReadKey();
        }
    }
}
