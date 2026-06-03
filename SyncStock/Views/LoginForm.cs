using DevExpress.XtraEditors;
using SyncStock.Database;
using SyncStock.Models.Accounts;
using SyncStock.Views.UserControl;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SyncStock;

namespace SyncStock.Views
{
    public partial class LoginForm : XtraForm
    {
        private readonly Repository _repository = new Repository();

        public User LoggedInUser { get; private set; }

        public LoginForm()
        {
            InitializeComponent();

            rfidLoginUC.CardScanned += OnCardScanned;
            emailLoginUC.LoginRequested += OnLoginRequested;

            ShowRfid();

            this.AcceptButton = emailLoginUC.LoginButton;
            this.Load += LoginForm_Load;
        }

        private void BtnRfidTab_Click(object sender, EventArgs e)
        {
            ShowRfid();
        }

        private void BtnEmailTab_Click(object sender, EventArgs e)
        {
            ShowEmail();
        }

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
            if (credentials.UserName == "admin" &&
                credentials.Password == "123")
            {
                User user = new User(
                    "admin",
                    "123",
                    "Asset",
                    "Manager",
                    "Admin");

                FinishLogin(user);
            }
            else if (credentials.UserName == "receiving" &&
                     credentials.Password == "123")
            {
                User user = new User(
                     "Sean",
                     "Sean123",
                     "Sean",
                     "Pait",
                     "Receiving");
                FinishLogin(user);


            }
            else if (credentials.UserName == "purchaser" &&
                     credentials.Password == "123")
            {
                User user = new User(
                     "Jane",
                     "Jane123",
                     "Jane",
                     "Smith",
                     "Purchaser");
                FinishLogin(user);

                
            }
            else if (credentials.UserName == "asset" &&
                     credentials.Password == "123")
            {
                User user = new User(
                     "Bob",
                     "Bob123",
                     "Bob",
                     "Johnson",
                     "Asset");
                FinishLogin(user);
            }
            else
            {
                XtraMessageBox.Show(
                    "Invalid username or password.",
                    "Login Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OnCardScanned(object sender, string uid)
        {
            if (uid == "04:83:20:01:9B:0C:03")
            {
                User user = new User(
                     "Sean",
                     "Sean123",
                     "Sean",
                     "Pait",
                     "Receiving")
                {
                    RfidUID = uid
                };

                FinishLogin(user);
            }
            else
            {
                XtraMessageBox.Show(
                    "Access Denied.",
                    "Login Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void FinishLogin(User user)
        {
            LoggedInUser = user;
            SessionManager.CurrentUser = user;
            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void RoundPanel(PanelControl panel, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            path.StartFigure();

            path.AddArc(
                new Rectangle(0, 0, radius, radius),
                180,
                90);

            path.AddArc(
                new Rectangle(panel.Width - radius, 0, radius, radius),
                270,
                90);

            path.AddArc(
                new Rectangle(panel.Width - radius, panel.Height - radius, radius, radius),
                0,
                90);

            path.AddArc(
                new Rectangle(0, panel.Height - radius, radius, radius),
                90,
                90);

            path.CloseFigure();

            panel.Region = new Region(path);
        }

        private void RoundButton(Control btn, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            path.StartFigure();

            path.AddArc(
                new Rectangle(0, 0, radius, radius),
                180,
                90);

            path.AddArc(
                new Rectangle(btn.Width - radius, 0, radius, radius),
                270,
                90);

            path.AddArc(
                new Rectangle(btn.Width - radius, btn.Height - radius, radius, radius),
                0,
                90);

            path.AddArc(
                new Rectangle(0, btn.Height - radius, radius, radius),
                90,
                90);

            path.CloseFigure();

            btn.Region = new Region(path);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            RoundPanel(PnlCard, 25);

            RoundButton(BtnRfidTab, 20);

            RoundButton(BtnEmailTab, 20);
        }
    }
}