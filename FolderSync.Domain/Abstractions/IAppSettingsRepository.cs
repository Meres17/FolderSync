namespace FolderSync.Domain
{
    public interface IAppSettingsRepository
    {
        AppSettings Load();
        void Save(AppSettings settings);
    }
}
