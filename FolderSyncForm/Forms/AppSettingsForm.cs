
using FolderSyncForm.Extensions;
using FolderSyncForm.Helpers;

namespace FolderSyncForm.Forms
{
    public partial class AppSettingsForm : Form
    {
        private readonly AppSettingsService _service = default!;

        /// <summary>
        /// 設計工具專用建構子，執行期請使用 DI 注入的多參數建構子。
        /// </summary>
        public AppSettingsForm()
        {
            InitializeComponent();
        }

        public AppSettingsForm(AppSettingsService service, AppSettingsProvider settings)
        {
            InitializeComponent();
            var appSettings = settings.Current;
            txtSource.Text = appSettings.Source;
            txtDest.Text = appSettings.Dest;
            txtIgnoreFolders.Text = string.Join(Environment.NewLine, appSettings.IgnoreFolders);
            txtIgnoreFiles.Text = string.Join(Environment.NewLine, appSettings.IgnoreFiles);
            txtSiteDelay.Text = appSettings.SiteDelay.ToString();
            _service = service;
            FormTheme.Apply(this);
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            txtSource.BindFolderDialog();
        }

        private void btnDest_Click(object sender, EventArgs e)
        {
            txtDest.BindFolderDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _service.Save(
                    txtSource.Text,
                    txtDest.Text,
                    txtIgnoreFolders.Text,
                    txtIgnoreFiles.Text,
                    txtSiteDelay.Text);
                this.DialogResult = DialogResult.OK;
                Dialog.Alert("儲存成功");
                this.Close();
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
        }
    }
}
