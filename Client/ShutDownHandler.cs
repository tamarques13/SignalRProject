using System.Runtime.InteropServices;

using static Client.MessageQueue;
using static Client.SignalRConnections;
using static Client.CollectInput;
using static Client.DataHandler;

namespace Client
{
    internal class ShutDownHandler
    {
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);
        public delegate bool SetConsoleCtrlEventHandler(CtrlType sig);
        public enum CtrlType { CTRL_C_EVENT = 0, CTRL_BREAK_EVENT = 1, CTRL_CLOSE_EVENT = 2, CTRL_LOGOFF_EVENT = 5, CTRL_SHUTDOWN_EVENT = 6 }

        public static bool Handler(CtrlType signal)
        {
            switch (signal)
            {
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    Console.WriteLine("Saving Data to File (data.txt)");

                    var content = queue.ToArray();
                    _ = DisconnectClientsFromCentral(groupInput);

                    File.WriteAllLines(path, content);
                    Environment.Exit(0);

                    return false;

                default:
                    return false;
            }
        }
    }
}
