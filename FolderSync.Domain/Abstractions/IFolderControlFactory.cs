namespace FolderSync.Domain
{
    public interface IFolderControlFactory
    {
        string[] GetNames();
        IFolderControl CreateControl(string name);
        IAsyncFolderControl CreateAsyncControl(string name);
    }
}
