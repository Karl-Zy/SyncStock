using DevExpress.XtraEditors;
using SyncStock.Views.UserControl;
using System;
using System.Windows.Forms;
using SyncStock.Models.Accounts;

namespace SyncStock
{
    public partial class MainForm : XtraForm
    {
        private readonly User _currentUser;

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

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadControl(UserControl control)
        {
            mainPanel.Controls.Clear();

            control.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(control);
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
    }
}