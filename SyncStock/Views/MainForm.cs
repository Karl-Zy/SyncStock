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

namespace SyncStock
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        public MainForm()
        {
            InitializeComponent();

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