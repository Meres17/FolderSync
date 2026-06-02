namespace FolderSync.Domain.Interfaces
{
    public interface IAppSettingsRepository
    {
        AppSettings Load();
        void Save(AppSettings settings);
    }
}
