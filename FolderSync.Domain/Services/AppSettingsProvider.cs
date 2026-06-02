namespace FolderSync.Domain.Services
{
    /// <summary>
    /// 持有目前設定的單一來源，存檔後可重新載入，讓相依物件圖維持穩定。
    /// </summary>
    public class AppSettingsProvider
    {
        private readonly IAppSettingsRepository _repository;
        private AppSettings _current;

        public AppSettingsProvider(IAppSettingsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _current = repository.Load();
        }

        public AppSettings Current => _current;

        public void Reload()
        {
            _current = _repository.Load();
        }
    }
}
