using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using SyncStock.Models;
using SyncStock.PrintForm;
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


        private void labelControl4_Click(object sender, EventArgs e)
        {

        }

        private void LoadData()
        {
            var items = _repo.GetAllReportItems().ToList();
            var orders = _repo.GetAllApprovedMonthlyCost().ToList();
            var totalItems = _repo.GetAllApprovedTotalItems();

            if (orders == null || orders.Count() == 0)
            {
                totalMonthlyCostLBL.Text = "0";
                totalMonthlyItemsLBL.Text = "0";

                return;
            }

            totalMonthlyCostLBL.Text = orders.Sum(o => o.TotalAmount).ToString("N2");
            totalMonthlyItemsLBL.Text = totalItems.ToString();
            ReportGC.DataSource = items;
        }

        private void ReportGC_Click(object sender, EventArgs e)
        {

        }

        private void PrintSummaryButton_Click(object sender, EventArgs e)
        {
            try
            {
                var report = new SummaryReport();           
                report.ShowPreviewDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Initializing Report: {ex.Message}");
            }
        }

        private void FilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportGC.DataSource = null;
            ReportGV.Columns.Clear();

            switch (FilterBox.Text) 
            {
                case "Purchased Orders":
                    ReportGC.DataSource = _repo.GetPurchaseOrderBrief().ToList();
                    break;

                case "Pending Orders":
                    ReportGC.DataSource = _repo.GetPendingPurchaseOrders().ToList();
                    break;

                case "Approved Order":
                    ReportGC.DataSource = _repo.GetAllReportItems().ToList();
                    break;
            }
        }
    }
}
