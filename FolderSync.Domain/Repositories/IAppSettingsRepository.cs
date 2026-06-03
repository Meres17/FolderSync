namespace FolderSync.Domain.Repositories
{
    public interface IAppSettingsRepository
    {
        AppSettings Load();
        void Save(AppSettings settings);
    }
}
