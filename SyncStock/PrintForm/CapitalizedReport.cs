using DevExpress.XtraReports.UI;
using SyncStock.Database;
using SyncStock.Models.Reports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace SyncStock.PrintForm
{
    public partial class CapitalizedReport : DevExpress.XtraReports.UI.XtraReport
    {
        Repository _repository = new Repository();

        public CapitalizedReport(IEnumerable<CapitalizedOrder> data)
        {
            InitializeComponent();
            this.objectDataSource1.DataSource = null;
            this.DataSource = data.ToList();
            this.DataMember = null;
        }

       

    }
}
