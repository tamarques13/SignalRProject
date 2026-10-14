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
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during application shutdown.");
                    }

                    return true;
                default:
                    return true;
            }
        }

        private void SaveDataToFile()
        {
            try
            {
                _signalRConnections
                    .DisconnectClientsFromCentral(_collectInput.GetGroupInput())
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to disconnect SignalR clients.");
            }

            Console.WriteLine("Saving Data to File (data.txt)");

            try
            {
                string path = _dataHandler.GetDataPath();

                if (string.IsNullOrWhiteSpace(path))
                {
                    _logger.LogWarning("Data file path is invalid.");
                    return;
                }

                var content = _messageQueue.GetQueue().ToArray();
                int index = 1;

                List<string> lines = [];

                foreach (var item in content)
                {
                    lines.Add($"{index}. {item}");
                    index++;
                }

                File.WriteAllLines(path, lines);
            }
            catch (IOException ex)
            {
                _logger.LogWarning(ex, $"Problem occured while saving queue");
            }
            finally
            {
                Environment.Exit(0);
            }
        }
    }
}
