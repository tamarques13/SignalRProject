using Microsoft.AspNetCore.Connections.Features;
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

            connection.StartAsync().Wait();

            connection.On("ReceiveMessage", (string userName, string message) =>
            {
                Console.WriteLine(userName + ':' + message);

            });


            // This does not follow the requirements, the server should send the notifications to each user
            // Purely for testing
            //while (true)
            //{
            //    connection.InvokeCoreAsync("SendToEveryone", args: ["Tiago", "A new post has been added!"]);

            //    Thread.Sleep(3000);
            //}

        Console.ReadKey();
        }
    }
}
