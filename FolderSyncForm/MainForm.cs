using FolderSyncCore;

using FolderSyncForm.Extensions;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public MainForm()
        {
            InitializeComponent();
            var settings = AppSettings.Build();
            txtSource.Text = settings.Source;
            txtTarget.Text = settings.Target;
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
            gvDiff.DataSource = new FolderComparer(txtSource.Text, txtTarget.Text).GetDiffFiles();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var comparer = new FolderComparer(txtSource.Text, txtTarget.Text);
            comparer.Backup();
            MessageBox.Show("�ƻs���\�A�t���ɮפw�ƥ��ܥ��ؿ����Ubackup/�ӷ�_�ؼ�/�ɶ��аO");
        }

        private void btnUpdateSite_Click(object sender, EventArgs e)
        {
            var comparer = new FolderComparer(txtSource.Text, txtTarget.Text);

            if (comparer.ExistOffline())
            {
                comparer.CloseSite();
                comparer.Backup();
                comparer.OpenSite();
                MessageBox.Show("���x��s���\�A�t���ɮפw�ƥ��ܥ��ؿ����Ubackup/�ӷ�_�ؼ�/�ɶ��аO");
            }
            else
            {
                MessageBox.Show("�䤣��App_offline.htm�A�бN�ɮש�m�������ɮ���");
            }
        }

        private void btnBackupList_Click(object sender, EventArgs e)
        {
            gvDiff.DataSource = new FolderComparer(txtSource.Text, txtTarget.Text).GetBackupFolders();
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
            var dto = (FolderDTO)gvDiff.SelectedRows[0].DataBoundItem;

            new FolderComparer(txtSource.Text, txtTarget.Text).Restore(dto.������|);

            MessageBox.Show("�٭즨�\");
        }


    }
}
