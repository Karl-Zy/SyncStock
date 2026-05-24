using DevExpress.XtraEditors;
using SyncStock.Views.UserControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SyncStock.Models.Accounts;

namespace SyncStock
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly Models.Accounts.User _currentUser;

        // Combine the parameter into the actual constructor block
        public MainForm(User user)
        {
            InitializeComponent();
            _currentUser = user;
            this.Text = $"SyncStock - Welcome {_currentUser.FirstName}";

            // Loading initial dashboard
            DashBoardUC dashboard = new DashBoardUC();
            dashboard.Dock = DockStyle.Fill;
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(dashboard);

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // important
            this.Bounds = Screen.PrimaryScreen.WorkingArea;

        }
        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadControl(UserControl control)
        {
            this.mainPanel.Controls.Clear();
            control.Dock = DockStyle.Fill;
            this.mainPanel.Controls.Add(control);
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
            LoadControl(new AuditorReviewUC());
        }

        private void reports_Click(object sender, EventArgs e)
        {
            LoadControl(new ReportUC());
        }
    }
}