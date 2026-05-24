using DevExpress.XtraEditors;
using SyncStock.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyncStock.Views.UserControl
{
    public partial class DashBoardUC : DevExpress.XtraEditors.XtraUserControl
    {
        private Repository _repo = new Repository();
        public DashBoardUC()
        {
            InitializeComponent();

            PendingOrdersGV.OptionsBehavior.Editable = false;
            ApprovedASAPOrdersGV.OptionsBehavior.Editable = false;

            this.Load += DashBoardUC_Load;
            MakeCircularPanel(panelControl13);
            MakeCircularPanel(panelControl14);
            MakeCircularPanel(panelControl16);
            MakeCircularPanel(panelControl15);

           
        }

        private void DashBoardUC_Load(object sender, EventArgs e)
        {
            LoadData(); 
        }
        private void MakeCircularPanel(PanelControl panel)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, panel.Width, panel.Height);
            panel.Region = new Region(path);
        }

        private void panelControl3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LoadData()
        {
            PendingOrdersNum.Text = _repo.GetPendingOrdersCount().ToString();
            ApproveAsapNum.Text = _repo.GetASAPOrders().Count.ToString();

            PendingOrdersGC.DataSource = _repo.GetPendingPurchaseOrders().ToList();
                //ApprovedASAPOrdersGC.DataSource = _repo.GetASAPOrders().ToList();
            var items = _repo.GetAllPurchaseOrderItems().ToList();
            decimal totalAmount = items.Sum(x => x.TotalPrice);
            pendingOrdersLBL.Text = "₱" + totalAmount.ToString("N2");
        }
    }
}
