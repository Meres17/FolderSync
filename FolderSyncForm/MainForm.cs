using FolderSyncCore;

using FolderSyncForm.Extensions;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private readonly FolderService _service;
        private readonly Site _site;
        private string _source => txtSource.Text;
        private string _target => txtTarget.Text;

        public MainForm()
        {
            InitializeComponent();
            _site = new Site();
            var appSettings = AppSettings.Build();
            _service = new FolderService(appSettings);
            txtSource.Text = appSettings.Source;
            txtTarget.Text = appSettings.Target;
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            txtSource.BindFolderDialog();
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            txtTarget.BindFolderDialog();
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            gvDiff.DataSource = _service.GetDiffFiles(_source, _target);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            _service.Backup(_source, _target);
            MessageBox.Show("�ƻs���\�A�t���ɮפw�ƥ��ܥ��ؿ����Ubackup/�ӷ�_�ؼ�/�ɶ��аO");
        }

        private void btnUpdateSite_Click(object sender, EventArgs e)
        {

            if (_site.CanClose())
            {
                _site.CloseSite(_target);
                _service.Backup(_source, _target);
                _site.OpenSite(_target);
                MessageBox.Show("���x��s���\�A�t���ɮפw�ƥ��ܥ��ؿ����Ubackup/�ӷ�_�ؼ�/�ɶ��аO");
            }
            else
            {
                MessageBox.Show("�䤣��App_offline.htm�A�бN�ɮש�m�������ɮ���");
            }
        }

        private void btnBackupList_Click(object sender, EventArgs e)
        {
            gvDiff.DataSource = _service.GetBackupFolders(_source, _target);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (gvDiff.SelectedRows.Count == 0)
            {
                MessageBox.Show("�п�ܭn�٭쪺�ɮ�");
                return;
            }

            if (gvDiff.SelectedRows.Count > 1)
            {
                MessageBox.Show("�n�٭쪺�ɮץu���@��");
                return;
            }

            // ���o����� FolderDTO ����
            var backupDir = (FolderDTO)gvDiff.SelectedRows[0].DataBoundItem;

            _service.Restore(backupDir.������|, _target);

            MessageBox.Show("�٭즨�\");
        }
    }
}
