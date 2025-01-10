using FolderSyncCore;

using FolderSyncForm.Extensions;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private readonly FolderSyncAppService _service;
        private string _source => txtSource.Text;
        private string _dest => txtDest.Text;
        private string _type => cbType.Text;

        public MainForm()
        {
            InitializeComponent();
            var appSettings = AppSettings.Build();
            txtSource.Text = appSettings.Source;
            txtDest.Text = appSettings.Dest;

            _service = new FolderSyncAppService(appSettings);
            cbType.Items.AddRange(_service.GetNames());
            cbType.SelectedIndex = 0;

            // 設定每個按鈕的 ToolTip 說明
            toolTip.SetToolTip(cbType, ".NET站台模式 先關閉站台再操作資料夾\r\n資料夾模式 直接操作資料夾");
            toolTip.SetToolTip(btnSource, "選擇 新檔案 資料夾");
            toolTip.SetToolTip(btnDest, "選擇 被覆蓋 資料夾");
            toolTip.SetToolTip(btnCompare, "比較 檔案 差異");
            toolTip.SetToolTip(btnCopy, "備份後覆蓋資料夾");
            toolTip.SetToolTip(btnBackupList, "顯示 備份清單");
            toolTip.SetToolTip(btnRestore, "從 備份清單 還原檔案");
            toolTip.SetToolTip(btnDeleteBackup, "刪除 選擇的備份紀錄");
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            txtSource.BindFolderDialog();
        }

        private void btnDest_Click(object sender, EventArgs e)
        {
            txtDest.BindFolderDialog();
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            try
            {
                gv.DataSource = _service.GetDiffFiles(_source, _dest);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                _service.Overwrite(_source, _dest, _type);
                MessageBox.Show(_type + "更新成功");
                gv.DataSource = _service.GetDiffFiles(_source, _dest);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBackupList_Click(object sender, EventArgs e)
        {
            gv.DataSource = _service.GetBackupFolders(_source, _dest, _type);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                _service.Restore(gv, _dest, _type);
                MessageBox.Show(_type + "還原成功");
                gv.DataSource = _service.GetDiffFiles(_source, _dest);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gv.DataSource = _service.GetBackupFolders(_source, _dest, _type);
            }
        }

        private void btnDeleteBackup_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckDelete())
                {
                    _service.DeleteBackup(gv);
                    MessageBox.Show("刪除成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gv.DataSource = _service.GetBackupFolders(_source, _dest, _type);
            }
        }

        private static bool CheckDelete()
        {
            var result = MessageBox.Show("確定刪除備份紀錄嗎", "刪除備份", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            return result == DialogResult.OK;
        }
    }
}
