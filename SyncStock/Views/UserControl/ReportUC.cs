using DevExpress.DataAccess.Native.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraReports.UI;
using SyncStock.Database;
using SyncStock.Models;
using SyncStock.Models.Reports;
using SyncStock.PrintForm;
using System;
using System.Collections;
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
        private IList _CurrentFilteredData;
        public ReportUC()
        {
            InitializeComponent();
            ReportGV.CustomColumnDisplayText += ReportGV_CustomColumnDisplayText;
            ReportGV.RowStyle += ReportGV_RowStyle;
            
            LoadData();
        }

        private void ReportGV_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e) 
        {
            if (FilterBox.Text != "Reconciliation") return;

            var row  = ReportGV.GetRow(e.RowHandle) as Reconciliation;
            if (row == null) return;

            bool QuantityMismatch = row.OrderedQuantity != row.ReceivedQuantity;
            bool AmountMismatch = row.OrderedAmount != row.ReceivedAmount;

            if (QuantityMismatch || AmountMismatch) 
            {
                e.Appearance.BackColor = Color.FromArgb(255, 199, 206); // Light red background
                e.Appearance.ForeColor = Color.FromArgb(156, 0, 6); // Dark red text
                e.HighPriority = true;
            }
        }

        private void labelControl4_Click(object sender, EventArgs e)
        {

        }

        private void LoadData()
        {
            PeriodTypeBox.Enabled = false;
            SecondFilterBox.Enabled = false;

            // Kuhaon ang tanan distinct nga years gikan sa database
            // ug i-add sa YearBox dropdown para ma-select sa user
            var years = _repo.GetDistinctYear().ToList();
            foreach (var year in years)
                YearBox.Properties.Items.Add(year.ToString());

            // Default: show received orders
            var receivedItems = _repo.GetAllReceivedOrders().ToList();

            FilterBox.SelectedIndex = 1; 

            var now = DateTime.Now;
            // I-filter ang received items para sa current bulan ug tuig,
            // ug i-sum ang ReceivedAmount para makuha ang total cost
            decimal totalCost = receivedItems
                .Where(i => i.DateReceived.Month == now.Month && i.DateReceived.Year == now.Year)
                .Sum(i => i.ReceivedAmount);
            //same ra sa total cost pero i-sum ang ReceivedQuantity para makuha ang total items
            int totalItems = receivedItems
                .Where(i => i.DateReceived.Month == now.Month && i.DateReceived.Year == now.Year)
                .Sum(i => i.ReceivedQuantity);
            // I-display ang total cost ug total items sa labels
            totalMonthlyCostLBL.Text = "₱" +totalCost.ToString("N2");
            totalMonthlyItemsLBL.Text = totalItems.ToString();
            // I-assign ang filtered list sa class-level variable para magamit sa printing
            _CurrentFilteredData = receivedItems;
            //default nga i-display ang received orders sa grid
            ReportGC.DataSource = receivedItems;
        }

        private void ReportGC_Click(object sender, EventArgs e)
        {

        }

        private void PrintSummaryButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_CurrentFilteredData == null)
                {
                    MessageBox.Show("No data to print. Please select a report type first.",
                                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dataToPrint = GetVisibleFilteredData(); // ✅ Only the filtered rows
                XtraReport report = null;

                switch (FilterBox.Text)
                {
                    case "Purchased Orders":
                        report = new PurchaseReport(dataToPrint.Cast<PurchaseOrderBrief>());
                        break;

                    case "Received Orders":
                        report = new ReceivedReport(dataToPrint.Cast<ReceivedItemReports>());
                        break;

                    case "Capitalized Orders":
                        report = new CapitalizedReport(dataToPrint.Cast<CapitalizedOrder>());
                        break;

                    case "Reconciliation":
                        report = new ReconciliationReport(dataToPrint.Cast<Reconciliation>());
                        break;

                    default:
                        MessageBox.Show("Please select a report type first.",
                                        "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }

                report.ShowPreviewDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void FilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportGC.DataSource = null;
            ReportGV.Columns.Clear();
            _CurrentFilteredData = null;

            switch (FilterBox.Text)
            {
                case "Purchased Orders":
                    // Kuhaon ang tanan nga Purchase Orders gikan sa database
                    _CurrentFilteredData = _repo.GetPurchaseOrderBrief().ToList();
                    // I-display ang data sa grid
                    ReportGC.DataSource = _CurrentFilteredData;
                    YearBox.Clear();
                    PeriodTypeBox.Clear();
                    PeriodTypeBox.Enabled = false;
                    SecondFilterBox.Clear();
                    YearBox.Text = "SELECT YEAR";
                    PeriodTypeBox.Text = "SELECT PERIOD TYPE";
                    GridCaption.Text = "     INVENTORY PURCHASE REPORT";
                    break;

                case "Received Orders":
                    _CurrentFilteredData = _repo.GetAllReceivedOrders().ToList();
                    ReportGC.DataSource = _CurrentFilteredData;
                    YearBox.Clear();
                    PeriodTypeBox.Clear();
                    PeriodTypeBox.Enabled = false;
                    SecondFilterBox.Clear();
                    YearBox.Text = "SELECT YEAR";
                    PeriodTypeBox.Text = "SELECT PERIOD TYPE";
                    GridCaption.Text = "     INVENTORY RECEIVED REPORT";
                    break;

                case "Capitalized Orders":
                    _CurrentFilteredData = _repo.GetAllCapitalizedOrder().ToList();
                    ReportGC.DataSource = _CurrentFilteredData;
                    YearBox.Clear();
                    PeriodTypeBox.Clear();
                    PeriodTypeBox.Enabled = false;
                    SecondFilterBox.Clear();
                    YearBox.Text = "SELECT YEAR";
                    PeriodTypeBox.Text = "SELECT PERIOD TYPE";
                    GridCaption.Text = "     INVENTORY CAPITALIZED REPORT";

                    var col2 = ReportGV.Columns["IsCapitalizable"];
                    if (col2 != null)
                    {
                        col2.ColumnEdit = new RepositoryItemTextEdit();
                    }

                    break;

                case "Reconciliation":
                    _CurrentFilteredData = _repo.GetReconciliationItems().ToList();
                    ReportGC.DataSource = _CurrentFilteredData;
                    YearBox.Clear();
                    PeriodTypeBox.Clear();
                    PeriodTypeBox.Enabled = false;
                    SecondFilterBox.Clear();
                    YearBox.Text = "SELECT YEAR";
                    PeriodTypeBox.Text = "SELECT PERIOD TYPE";
                    GridCaption.Text = "     INVENTORY RECONCILIATION REPORT";

                    // I-hide ang AttachmentData column kay binary data siya,
                    // dili pwede ipakita direkta sa grid
                    if (ReportGV.Columns["AttachmentData"] != null)
                        ReportGV.Columns["AttachmentData"].Visible = false;

                    // Kung wala pay AttachmentPreview column,
                    // i-add ang custom button column para ma-view ang image
                    if (ReportGV.Columns["AttachmentPreview"] == null)
                        AddAttachmentImageColumn();

                    var col = ReportGV.Columns["IsCapitalizable"];
                    if (col != null)
                    {
                        col.ColumnEdit = new RepositoryItemTextEdit();
                    }

                    break;
            }
        }

        private void AddAttachmentImageColumn()
        {
            var btnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();

            btnEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            btnEdit.Buttons.Clear();

            btnEdit.Buttons.Add(
                new DevExpress.XtraEditors.Controls.EditorButton(
                    DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
                {
                    Caption = "View Image"
                });

            ReportGC.RepositoryItems.Add(btnEdit);

            var imgCol = new DevExpress.XtraGrid.Columns.GridColumn
            {
                Caption = "Attachment",
                Name = "AttachmentPreview",
                UnboundType = DevExpress.Data.UnboundColumnType.String,
                Visible = true,
                VisibleIndex = 10,
                ColumnEdit = btnEdit
            };

            imgCol.OptionsColumn.AllowEdit = true;
            imgCol.OptionsColumn.ReadOnly = false;

            ReportGV.Columns.Add(imgCol);

            btnEdit.ButtonClick += (s, e) =>
            {
                try
                {
                    int rowHandle = ReportGV.FocusedRowHandle;

                    var row = ReportGV.GetRow(rowHandle) as Reconciliation;

                    if (row == null)
                    {
                        MessageBox.Show("Row is null");
                        return;
                    }

                    if (row.AttachmentData == null || row.AttachmentData.Length == 0)
                    {
                        MessageBox.Show("No attachment available.");
                        return;
                    }

                    ShowImagePreview(
                        row.AttachmentData,
                        row.AttachmentFileName
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            };
        }

        private void AttachmentButtonEdit_ButtonClick(object sender,
    DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            XtraMessageBox.Show("yawa");
            // Kuhaon ang data sa row nga currently focused/gipili sa user
            // Gi-cast as Reconciliation para ma-access ang iyang properties
            var row = ReportGV.GetFocusedRow() as Reconciliation;

            if (row == null || row.AttachmentData == null || row.AttachmentData.Length == 0)
            {
                MessageBox.Show("No attachment available for this item.",
                                "No Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ShowImagePreview(row.AttachmentData, row.AttachmentFileName);
        }

        private void ShowImagePreview(byte[] imageData, string fileName)
        {
            try
            {                                                           // I-convert ang binary data (byte[]) ngadto sa MemoryStream
                                                                        // para mabasa sa Image.FromStream()
                                                                        // "using" para auto-close ang stream human magamit
                using (var ms = new System.IO.MemoryStream(imageData))
                {
                    var image = Image.FromStream(ms);                // I-convert ang MemoryStream ngadto sa Image object
                                                                     // para mapakita sa PictureBox

                    // Build a lightweight popup form
                    var previewForm = new Form
                    {
                        Text = $"Attachment Preview — {fileName ?? "Image"}",
                        Size = new Size(800, 600),
                        StartPosition = FormStartPosition.CenterParent,
                        FormBorderStyle = FormBorderStyle.Sizable,
                        MinimumSize = new Size(400, 300)
                    };

                    var pictureBox = new PictureBox
                    {
                        Image = image,
                        Dock = DockStyle.Fill,
                        SizeMode = PictureBoxSizeMode.Zoom   // keeps aspect ratio
                    };

                    previewForm.Controls.Add(pictureBox);
                    previewForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open attachment: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulationSecondFilter(int year)
        {
            switch (PeriodTypeBox.SelectedIndex)
            {
                case 0: //index sa monthly sa period type box
                    for (int month = 1; month <= 12; month++)
                        SecondFilterBox.Properties.Items.Add(new DateTime(year, month, 1).ToString("MMMM"));
                    SecondFilterBox.Enabled = true;
                    SecondFilterBox.Text = "Select Month";
                    break;

                case 1: //index sa quarterly sa period type box
                    SecondFilterBox.Properties.Items.Add("Q1");
                    SecondFilterBox.Properties.Items.Add("Q2");
                    SecondFilterBox.Properties.Items.Add("Q3");
                    SecondFilterBox.Properties.Items.Add("Q4");
                    SecondFilterBox.Enabled = true;
                    SecondFilterBox.Text = "Select Quarter";
                    break;

                default:
                    SecondFilterBox.Enabled = false;
                    break;
            }
        }

        private void ApplyPeriodFilter()
        {
            if (string.IsNullOrEmpty(YearBox.Text)) return;

            // If period is selected but no month/quarter chosen yet, just filter by year
            if (PeriodTypeBox.SelectedIndex == -1 || string.IsNullOrEmpty(SecondFilterBox.Text))
            {
                int y = int.Parse(YearBox.Text);

                // I-check kung unsang date column ang gamiton base sa report type
                // Received ug Capitalized = "DateReceived"
                // Purchased ug Reconciliation = "OrderDate"
                string col = (FilterBox.Text == "Received Orders" || FilterBox.Text == "Capitalized Orders")
                    ? "DateReceived" : "OrderDate";

                // I-filter ang grid para sa tibuok year lang
                // Pananglitan: 01/01/2026 hangtod 12/31/2026
                ReportGV.ActiveFilterString =
                    $"[{col}] >= #{new DateTime(y, 1, 1):MM/dd/yyyy}# AND [{col}] <= #{new DateTime(y, 12, 31):MM/dd/yyyy}#";
                return;
            }

            // Kung naa'y napili nga year, month/quarter,
            // i-parse ang year gikan sa YearBox
            int year = int.Parse(YearBox.Text);
            DateTime start, end;

            // I-check pud ang date column base sa report type
            string DateColumn = (FilterBox.Text == "Received Orders" || FilterBox.Text == "Capitalized Orders")
                ? "DateReceived" : "OrderDate";

            switch (PeriodTypeBox.SelectedIndex)
            {
                case 0: // Monthly
                        // I-parse ang bulan gikan sa SecondFilterBox
                        // Pananglitan: "January" -> 1, "February" -> 2, etc
                    int month = DateTime.ParseExact(SecondFilterBox.Text, "MMMM",
                                System.Globalization.CultureInfo.InvariantCulture).Month;

                    // I-set ang start sa unang adlaw sa bulan
                    // ug end sa katapusang adlaw sa bulan
                    start = new DateTime(year, month, 1);
                    end = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    break;

                case 1: // Quarterly
                        // I-set ang start ug end month base sa quarter nga napili
                    int startMonth, endMonth;
                    switch (SecondFilterBox.Text)
                    {
                        case "Q1": startMonth = 1; endMonth = 3; break;
                        case "Q2": startMonth = 4; endMonth = 6; break;
                        case "Q3": startMonth = 7; endMonth = 9; break;
                        case "Q4": startMonth = 10; endMonth = 12; break;
                        default: return;
                    }
                    start = new DateTime(year, startMonth, 1);
                    end = new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth));
                    break;

                default:
                    ReportGV.ActiveFilterString = string.Empty;
                    return;
            }
            // I-apply ang final filter sa grid base sa
            // napili nga date column, start date, ug end date
            // Pananglitan: [DateReceived] >= #01/01/2026# AND [DateReceived] <= #03/31/2026#
            ReportGV.ActiveFilterString =
                $"[{DateColumn}] >= #{start:MM/dd/yyyy}# AND [{DateColumn}] <= #{end:MM/dd/yyyy}#";
        }

        private void YearBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            PeriodTypeBox.SelectedIndex = -1;
            PeriodTypeBox.Text = "SELECT PERIOD TYPE";
            SecondFilterBox.Properties.Items.Clear();
            SecondFilterBox.SelectedIndex = -1;
            SecondFilterBox.Text = string.Empty;
            SecondFilterBox.Enabled = false;
            SecondFilterBox.Visible = true;
            ReportGV.ActiveFilterString = string.Empty;

            if (!int.TryParse(YearBox.Text, out int y)) return;

            PeriodTypeBox.Enabled = true;

            string col = (FilterBox.Text == "Received Orders" || FilterBox.Text == "Capitalized Orders")
                ? "DateReceived" : "OrderDate";

            ReportGV.ActiveFilterString =
                $"[{col}] >= #{new DateTime(y, 1, 1):MM/dd/yyyy}# AND [{col}] <= #{new DateTime(y, 12, 31):MM/dd/yyyy}#";
        }

        private void PeriodTypeBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            SecondFilterBox.Properties.Items.Clear();
            SecondFilterBox.SelectedIndex = -1;
            SecondFilterBox.Text = string.Empty;
            SecondFilterBox.Enabled = false;

            if (!int.TryParse(YearBox.Text, out int y)) return;

            // Re-apply year filter
            string col = (FilterBox.Text == "Received Orders" || FilterBox.Text == "Capitalized Orders")
                ? "DateReceived" : "OrderDate";

            ReportGV.ActiveFilterString =
                $"[{col}] >= #{new DateTime(y, 1, 1):MM/dd/yyyy}# AND [{col}] <= #{new DateTime(y, 12, 31):MM/dd/yyyy}#";

            PopulationSecondFilter(y);
        }

        private IList GetVisibleFilteredData()
        {
            var rows = new System.Collections.ArrayList();
            for (int i = 0; i < ReportGV.DataRowCount; i++)
            {
                // GetRow only returns rows that pass the active filter
                var row = ReportGV.GetRow(i);
                if (row != null)
                    rows.Add(row);
            }
            return rows;
        }

        private void SecondFilterBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SecondFilterBox.Text)) return;
            ApplyPeriodFilter();
        }

        private void ReportGV_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Value == null) return;

            if (e.Column.FieldName == "PoType" ||
                e.Column.FieldName == "POType")
            {
                switch (e.Value.ToString().ToUpper())
                {
                    case "GPO":
                        e.DisplayText = "LOCAL";
                        break;

                    case "OPO":
                        e.DisplayText = "ONLINE";
                        break;
                }
            }

            if (e.Column.FieldName == "IsCapitalizable")
            {
                e.DisplayText = (e.Value is bool b && b) ? "YES" : "NO";
            }
        }
    }


}