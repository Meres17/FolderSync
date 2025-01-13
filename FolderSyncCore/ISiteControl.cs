namespace FolderSyncCore
{
    public interface ISiteControl
    {
        void OpenSite(string destDir);

        void CloseSite(string destDir);
    }
}
