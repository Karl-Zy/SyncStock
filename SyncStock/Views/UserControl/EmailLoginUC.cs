using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace SyncStock.Views
{
    public partial class EmailLoginUC : DevExpress.XtraEditors.XtraUserControl
    {
        public event EventHandler<(string UserName, string Password)> LoginRequested;

        public EmailLoginUC()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string userName = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                XtraMessageBox.Show("Please enter your username and password.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoginRequested?.Invoke(this, (userName, password));
        }
    }
}