using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SyncStock.Views.UserControl
{
    public partial class DashBoardUC : DevExpress.XtraEditors.XtraUserControl
    {
        public DashBoardUC()
        {
            InitializeComponent();
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
    }
}
