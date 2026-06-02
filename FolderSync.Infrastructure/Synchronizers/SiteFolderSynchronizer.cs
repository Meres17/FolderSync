
namespace FolderSync.Infrastructure.Synchronizers
{
    internal class SiteFolderSynchronizer : IFolderSynchronizer
    {
        private readonly IFolderSynchronizer _inner;
        private readonly ISiteController _site;
        private readonly AppSettings _settings;

        public SiteFolderSynchronizer(IFolderSynchronizer inner, ISiteController site, AppSettings settings)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _site = site ?? throw new ArgumentNullException(nameof(site));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public Task ApplyAsync(IReadOnlyList<FileStatus> changes, string sourceDir, string destDir)
        {
            return WithSiteOfflineAsync(destDir, () => _inner.ApplyAsync(changes, sourceDir, destDir));
        }

        public Task RollbackAsync(string backupDir, string destDir)
        {
            return WithSiteOfflineAsync(destDir, () => _inner.RollbackAsync(backupDir, destDir));
        }

        internal virtual async Task WithSiteOfflineAsync(string siteDir, Func<Task> action)
        {
            try
            {
                _site.TakeOffline(siteDir);
                await Task.Delay(_settings.SiteDelay);
                await action();
            }
            finally
            {
                _site.BringOnline(siteDir);
            }
        }
    }
}
