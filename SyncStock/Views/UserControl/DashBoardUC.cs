using DevExpress.XtraEditors;
using SyncStock.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

            LoadData();


            MakeCircularPanel(panelControl13);
            MakeCircularPanel(panelControl14);
            MakeCircularPanel(panelControl16);
            MakeCircularPanel(panelControl15);
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
        }
    }
}
