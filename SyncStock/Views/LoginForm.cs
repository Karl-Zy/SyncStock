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
        public User LoggedInUser { get; private set; }

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

        private void OnLoginRequested(object sender, (string UserName, string Password) credentials)
        {
            // Check if Username is 'admin' AND Password is '123'
            if (credentials.UserName == "admin" && credentials.Password == "123")
            {
                // Credentials are correct
                User user = new User("admin", "123", "Asset", "Manager");
                FinishLogin(user);
            }
            else
            {
                // If EITHER the username or password is wrong, show this generic message.
                // This is the "Incorrect Password" logic you asked for.
                XtraMessageBox.Show("Invalid username or password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnCardScanned(object sender, string uid)
        {
            if (uid == "04:83:20:01:9B:0C:03")
            {
                User user = new User("Sean", "Sean123", "Sean", "Pait") { RfidUID = uid };
                FinishLogin(user);
            }
            else
            {
                // Generic message for RFID too
                XtraMessageBox.Show("Access Denied.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FinishLogin(User user)
        {
            // 1. Store the user so Program.cs can pass it to MainForm
            this.LoggedInUser = user;

            // 2. Set the result to OK. Program.cs is waiting for this!
            this.DialogResult = DialogResult.OK;

            // 3. Close the form
            this.Close();
        }

    }
}