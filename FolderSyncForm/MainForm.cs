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

            // �]�w�C�ӫ��s�� ToolTip ����
            toolTip.SetToolTip(cbType, ".NET���x�Ҧ� ���������x�A�ާ@��Ƨ�\r\n��Ƨ��Ҧ� �����ާ@��Ƨ�");
            toolTip.SetToolTip(btnSource, "��� �s�ɮ� ��Ƨ�");
            toolTip.SetToolTip(btnDest, "��� �Q�л\ ��Ƨ�");
            toolTip.SetToolTip(btnCompare, "��� �ɮ� �t��");
            toolTip.SetToolTip(btnCopy, "�ƥ����л\��Ƨ�");
            toolTip.SetToolTip(btnBackupList, "��� �ƥ��M��");
            toolTip.SetToolTip(btnRestore, "�q �ƥ��M�� �٭��ɮ�");
            toolTip.SetToolTip(btnDeleteBackup, "�R�� ��ܪ��ƥ�����");
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
                MessageBox.Show(_type + "��s���\");
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
                MessageBox.Show(_type + "�٭즨�\");
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
                    MessageBox.Show("�R�����\");
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
            var result = MessageBox.Show("�T�w�R���ƥ�������", "�R���ƥ�", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            return result == DialogResult.OK;
        }
    }
}
