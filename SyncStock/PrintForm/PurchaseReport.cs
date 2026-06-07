using DevExpress.XtraReports.UI;
using SyncStock.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace SyncStock.PrintForm
{
    public partial class PurchaseReport : DevExpress.XtraReports.UI.XtraReport
    {
        public PurchaseReport()
        {
            InitializeComponent();
        }

        public PurchaseReport(IEnumerable<PurchaseOrderBrief> data)
        {
            InitializeComponent();

            var list = data.ToList();

            this.objectDataSource1.DataSource = null;
            this.DataSource = list;
            this.DataMember = null;

            totalAmountXRL.Text =
                "₱ " + list.Sum(x => x.TotalPrice).ToString("N2");
        }

    }
}
