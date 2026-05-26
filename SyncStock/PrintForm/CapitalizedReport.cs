using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using SyncStock.Database;

namespace SyncStock.PrintForm
{
    public partial class CapitalizedReport : DevExpress.XtraReports.UI.XtraReport
    {
        Repository _repository = new Repository();

        public CapitalizedReport()
        {
            InitializeComponent();
        }

       

    }
}
