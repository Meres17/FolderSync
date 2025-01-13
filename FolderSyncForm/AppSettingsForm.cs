using FolderSyncCore;

using FolderSyncForm.Extensions;
using FolderSyncForm.Helpers;

namespace FolderSyncForm
{
    public partial class AppSettingsForm : Form
    {
        private readonly AppSettingsAppService _service;

        public AppSettingsForm()
        {
            InitializeComponent();
            var appSettings = AppSettings.Build();
            txtSource.Text = appSettings.Source;
            txtDest.Text = appSettings.Dest;
            txtIgnoreFolders.Text = string.Join(Environment.NewLine, appSettings.IgnoreFolders);
            txtIgnoreFiles.Text = string.Join(Environment.NewLine, appSettings.IgnoreFiles);
            _service = new AppSettingsAppService();
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
                _service.Save(txtSource.Text, txtDest.Text, txtIgnoreFolders.Text, txtIgnoreFiles.Text);
                MessageHelper.Ok("儲存成功");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex);
            }
        }
    }
}
