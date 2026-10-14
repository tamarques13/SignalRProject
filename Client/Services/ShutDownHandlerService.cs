using Client.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Client.Services
{
    internal class ShutDownHandlerService(
        IMessageQueueService messageQueue,
        ICollectInputService collectInput,
        IDataHandlerService dataHandler,
        ISignalRConnectionService signalRConnections,
        ILogger<ShutDownHandlerService> logger
        ) : IShutDownHandlerService
    {
        private readonly IMessageQueueService _messageQueue = messageQueue;
        private readonly ICollectInputService _collectInput = collectInput;
        private readonly IDataHandlerService _dataHandler = dataHandler;
        private readonly ISignalRConnectionService _signalRConnections = signalRConnections;
        private readonly ILogger<ShutDownHandlerService> _logger = logger;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);
        private delegate bool SetConsoleCtrlEventHandler(CtrlType sig);

        public void SetShutDownProcess()
        {
            SetConsoleCtrlHandler(Handler, true);
        }

        private bool Handler(CtrlType signal)
        {
            switch (signal)
            {
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    try
                    {
                        SaveDataToFile();
                        return false;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during application shutdown.");
                        return false;
                    }

                default:
                    return false;
            }
        }

        private void SaveDataToFile()
        {
            Console.WriteLine("Saving Data to File (data.txt)");

            // Bug at line 45/46  
            _signalRConnections
                .DisconnectClientsFromCentral(_collectInput.GetGroupInput())
                .GetAwaiter()
                .GetResult();

            var content = _messageQueue.GetQueue().ToArray();

            File.WriteAllLines(_dataHandler.GetDataPath(), content);
            //Environment.Exit(0);
        }
    }
}
