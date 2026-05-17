using DevExpress.XtraEditors;
using SyncStock.Database;
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
    public partial class ReportUC : DevExpress.XtraEditors.XtraUserControl
    {
        Repository _repo = new Repository();
        public ReportUC()
        {
            InitializeComponent();
            LoadData();
        }

       
        private void labelControl1_Click(object sender, EventArgs e)
        {

        }

        private void labelControl4_Click(object sender, EventArgs e)
        {

        }

        private void LoadData()
        {
            var orders = _repo.GetAllApprovedMonthlyCost().ToList();

            if (orders == null || orders.Count() == 0)
            {
                totalMonthlyCostLBL.Text = "0";

                return;
            }

            totalMonthlyCostLBL.Text = orders.Sum(o => o.TotalAmount).ToString("N2");
            
        }
    }
}
