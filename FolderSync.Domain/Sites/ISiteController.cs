namespace FolderSync.Domain.Sites
{
    /// <summary>
    /// 控制 .NET 站台上下線（透過 App_offline.htm）。
    /// </summary>
    public interface ISiteController
    {
        void TakeOffline(string siteDir);

        void BringOnline(string siteDir);
    }
}
