using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using SyncStock.Database;

namespace SyncStock.PrintForm
{
    public partial class SummaryReport : DevExpress.XtraReports.UI.XtraReport
    {
        Repository _repository = new Repository();

        public SummaryReport()
        {
            InitializeComponent();
        }

       

    }
}
