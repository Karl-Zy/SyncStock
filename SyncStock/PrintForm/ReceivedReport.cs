using DevExpress.XtraReports.UI;
using SyncStock.Models.Reports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace SyncStock.PrintForm
{
    public partial class ReceivedReport : DevExpress.XtraReports.UI.XtraReport
    {
        public ReceivedReport()
        {
            InitializeComponent();
        }

        public ReceivedReport(IEnumerable<ReceivedItemReports> data)
        {
            InitializeComponent();

            var list = data.ToList();

            this.objectDataSource1.DataSource = null;
            this.DataSource = list;
            this.DataMember = null;

            totalAmountXRL.Text =
                "₱ " + list.Sum(x => x.ReceivedAmount).ToString("N2");
        }

    }
}
