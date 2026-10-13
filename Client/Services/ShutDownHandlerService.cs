using Client.Services.Interfaces;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace Client.Services
{
    internal class ShutDownHandlerService(
        IMessageQueueService messageQueue,
        ICollectInputService collectInput,
        IDataHandlerService dataHandler,
        ISignalRConnectionService signalRConnections
        ) : IShutDownHandlerService
    {
        private readonly IMessageQueueService _messageQueue = messageQueue;
        private readonly ICollectInputService _collectInput = collectInput;
        private readonly IDataHandlerService _dataHandler = dataHandler;
        private readonly ISignalRConnectionService _signalRConnections = signalRConnections;
        
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
                    Console.WriteLine("Saving Data to File (data.txt)");

                    _ = _signalRConnections.DisconnectClientsFromCentral(_collectInput.GetGroupInput());
                    var content = _messageQueue.GetQueue().ToArray();

                    File.WriteAllLines(_dataHandler.GetDataPath(), content);
                    Environment.Exit(0);

                    return false;

                default:
                    return false;
            }
        }
    }
}
