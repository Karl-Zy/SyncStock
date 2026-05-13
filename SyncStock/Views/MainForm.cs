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
        private User _currentUser;

        public MainForm(User user) // Must accept a 'User' object
        public MainForm()
        {
            InitializeComponent();
            _currentUser = user;
            this.Text = $"SyncStock - Welcome {_currentUser.FirstName}";

            DashBoardUC dashboard = new DashBoardUC();

            dashboard.Dock = DockStyle.Fill;

            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(dashboard);

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
            LoadControl(new ReceivingUC());
        }

        private void auditorReview_Click(object sender, EventArgs e)
        {
            LoadControl(new AuditorReviewUC());
        }

        private void reports_Click(object sender, EventArgs e)
        {
            LoadControl(new ReportsUC());
        }
    }
}