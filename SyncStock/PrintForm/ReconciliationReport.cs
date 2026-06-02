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
    public partial class ReconciliationReport : DevExpress.XtraReports.UI.XtraReport
    {
        public ReconciliationReport(IEnumerable<Reconciliation> data)
        {
            InitializeComponent();
            this.objectDataSource1.DataSource = null;
            this.DataSource = data.ToList();
            this.DataMember = null;
        }

    }
}
