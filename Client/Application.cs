using Client.Services.Interfaces;

namespace Client
{
    public class Application(
        IDataHandlerService dataHandler,
        IMessageQueueService messageQueue,
        ICollectInputService collectInput,
        ISignalRConnectionService signalRConnections,
        IShutDownHandlerService shutDownHandler
        )
    {
        private readonly IDataHandlerService _dataHandler = dataHandler;
        private readonly IMessageQueueService _messageQueue = messageQueue;
        private readonly ICollectInputService _collectInput = collectInput;
        private readonly ISignalRConnectionService _signalRConnections = signalRConnections;
        private readonly IShutDownHandlerService _shutDownHandler = shutDownHandler;

        public async Task ConsoleApp()
        {
            _shutDownHandler.SetShutDownProcess();

            Console.WriteLine("SignalR w/ Queue Training Project");
            _dataHandler.HandleDataOnStartUp();

            _collectInput.SetGroupInput(_collectInput.CollectString("Group"));
            string clientInput = _collectInput.CollectString("Client");

            await _signalRConnections.ConnectClientToCentral(clientInput, _collectInput.GetGroupInput());
            var task = _messageQueue.ProcessMessageQueue(1000, _messageQueue.GetCancellationTokenSource().Token);

            _messageQueue.CancelProcessMessageQueue(task);

            await _signalRConnections.DisconnectClientsFromCentral(_collectInput.GetGroupInput());
        }
    }
}
