using DevExpress.XtraEditors;
using SyncStock.Models.Accounts;
using SyncStock.Views;
using SyncStock.Views.Theme;
using SyncStock.Views.UserControl;
using System;
using System.Windows.Forms;

namespace SyncStock
{
    public partial class MainForm : XtraForm
    {
        private readonly User _currentUser;

        public bool LogoutRequested { get; private set; }
        public MainForm(User user)
        {
            InitializeComponent();

            _currentUser = user;

            this.Text = $"SyncStock - Welcome {_currentUser.FirstName}";

            // HIDE ALL MENUS FIRST
            dashBoard.Visible = false;
            purchaseOrder.Visible = false;
            reveivingCustodian.Visible = false;
            auditorReview.Visible = false;
            reports.Visible = false;

            // =========================
            // ROLE-BASED ACCESS
            // =========================

            // ADMIN ACCESS
            if (_currentUser.Role == "Admin")
            {
                dashBoard.Visible = true;
                purchaseOrder.Visible = true;
                reveivingCustodian.Visible = true;
                auditorReview.Visible = true;
                reports.Visible = true;
            }

            // PURCHASER ACCESS
            else if (_currentUser.Role == "Purchaser")
            {
                dashBoard.Visible = true;
                purchaseOrder.Visible = true;
            }

            // RECEIVING ACCESS
            else if (_currentUser.Role == "Receiving")
            {
                dashBoard.Visible = true;
                reveivingCustodian.Visible = true;
            }

            // ASSET ACCESS
            else if (_currentUser.Role == "Asset")
            {
                dashBoard.Visible = true;
                auditorReview.Visible = true;
                reports.Visible = true;
            }

            // LOAD DASHBOARD
            DashBoardUC dashboard = new DashBoardUC();

            dashboard.Dock = DockStyle.Fill;

            mainPanel.Controls.Clear();

            mainPanel.Controls.Add(dashboard);

            // FORM SETTINGS
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.MaximizeBox = false;

            this.StartPosition = FormStartPosition.CenterScreen;

            // FULLSCREEN WORK AREA
            this.Bounds = Screen.PrimaryScreen.WorkingArea;
        }



        private void LoadControl(UserControl control)
        {
            mainPanel.Controls.Clear();

            control.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(control);

            // REAPPLY CURRENT THEME
            ThemeManager.SetTheme(
                ThemeManager.CurrentMode
            );
        }

        private void dashBoard_Click(object sender, EventArgs e)
        {
            LoadControl(new DashBoardUC());
        }

        private void purchaseOrder_Click(object sender, EventArgs e)
        {
            LoadControl(new PurchaseOrderUC());
        }

        private void reveivingCustodian_Click(object sender, EventArgs e)
        {
            LoadControl(new ReceivingCustodianUC());
        }

        private void auditorReview_Click(object sender, EventArgs e)
        {
            LoadControl(new AuditorReviewUC(_currentUser));
        }

        private void reports_Click(object sender, EventArgs e)
        {
            LoadControl(new ReportUC());
        }

       

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult result =
           XtraMessageBox.Show(
               "Are you sure you want to logout?",
               "Logout",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question
           );

            if (result == DialogResult.Yes)
            {
                LogoutRequested = true;

                this.Close();
            }
        }

       


    }
}