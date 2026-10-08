using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

using static Client.ShutDownHandler;
using static Client.MessageQueue;
using static Client.SignalRConnections;
using static Client.CollectInput;
using static Client.DataHandler;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            SetConsoleCtrlHandler(Handler, true);

            Console.WriteLine("SignalR w/ Queue Training Project");
            handleDataOnStartUp();

            groupInput = CollectString("Group");
            string clientInput = CollectString("Client");

            await ConnectClientToCentral(clientInput, groupInput);
            var task = ProcessMessageQueue(1000, token);

            CancelProcessMessageQueue(task);

            await DisconnectClientsFromCentral(groupInput);
        }
    }
}