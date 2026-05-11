using DevExpress.XtraEditors;
using SyncStock.Database;
using SyncStock.Models.Accounts;
using System;
using System.Windows.Forms;

namespace SyncStock.Views
{
    public partial class LoginForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly Repository _repository = new Repository();

        public LoginForm()
        {
            InitializeComponent();
            rfidLoginUC.CardScanned += OnCardScanned;
            emailLoginUC.LoginRequested += OnLoginRequested;
            ShowRfid();
        }

        private void BtnRfidTab_Click(object sender, EventArgs e) => ShowRfid();
        private void BtnEmailTab_Click(object sender, EventArgs e) => ShowEmail();

        private void ShowRfid()
        {
            rfidLoginUC.Visible = true;
            emailLoginUC.Visible = false;
        }

        private void ShowEmail()
        {
            rfidLoginUC.Visible = false;
            emailLoginUC.Visible = true;
        }

        private void OnCardScanned(object sender, string uid)
        {
            User user = _repository.GetUserByRfid(uid);

            if (user == null)
            {
                XtraMessageBox.Show(
                    "No account found for this card.",
                    "Access Denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            OpenMainForm(user);
        }

        private void OnLoginRequested(object sender, (string UserName, string Password) credentials)
        {
            User user = _repository.GetUserByCredentials(credentials.UserName, credentials.Password);

            if (user == null)
            {
                XtraMessageBox.Show(
                    "Invalid username or password.",
                    "Access Denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            OpenMainForm(user);
        }

        private void OpenMainForm(User user)
        {
            MainForm mainForm = new MainForm(user);
            mainForm.Show();
            this.Hide();
        }
    }
}