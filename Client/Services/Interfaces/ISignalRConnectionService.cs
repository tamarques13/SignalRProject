using Microsoft.AspNetCore.SignalR.Client;

namespace Client.Services.Interfaces
{
    public interface ISignalRConnectionService
    {
        Task ConnectClientToCentral(string clientInput, string groupName);
        Task DisconnectClientsFromCentral(string groupName);
    }
}
