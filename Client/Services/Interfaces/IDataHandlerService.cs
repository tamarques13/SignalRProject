namespace Client.Services.Interfaces
{
    public interface IDataHandlerService
    {
        void HandleDataOnStartUp();
        string GetDataPath();
    }
}