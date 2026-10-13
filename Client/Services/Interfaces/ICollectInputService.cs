namespace Client.Services.Interfaces
{
    public interface ICollectInputService
    {
        string CollectString(string type);
        void SetGroupInput(string value);
        string GetGroupInput();
    }
}
