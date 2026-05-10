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

namespace SyncStock.Views.UserControl
{
    public partial class ReceivingUC : DevExpress.XtraEditors.XtraUserControl
    {
        public ReceivingUC()
        {
            InitializeComponent();
            LoadDummyData();
        }
        private bool isFormReady = false;

        private void ReceivingUC_Load(object sender, EventArgs e)
        {
            isFormReady = false; // Mute events

            LoadDummyData();

            // Set visibility to Never one last time to be safe
            layoutControlGroup1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            isFormReady = true; // Now we are ready for user clicks
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // If the form is still loading, do nothing
            if (!isFormReady) return;

            // Only run if a valid row is selected
            if (e.FocusedRowHandle >= 0)
            {
                layoutControlGroup1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.Dock = DockStyle.Top;
                this.Height = 1100;

                if (this.Parent is Panel p)
                {
                    p.AutoScroll = true;
                }
            }
        }

        private void LoadDummyData()
        {
            // 1. Create the table structure
            DataTable dt = new DataTable();
            dt.Columns.Add("PO_Number", typeof(string));
            dt.Columns.Add("Item_Name", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Expected_Qty", typeof(int));

            // 2. Add some fake rows
            dt.Rows.Add("PO-2026-001", "Dell Laptop", "IT Department", 5);
            dt.Rows.Add("PO-2026-002", "Office Chair", "HR Department", 10);
            dt.Rows.Add("PO-2026-003", "Projector", "Marketing", 2);

            // 3. Bind it to your GridControl
            gridControl1.DataSource = dt;

            gridView1.PopulateColumns();
            gridView1.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        }
    }
}
