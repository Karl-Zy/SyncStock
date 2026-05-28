using DevExpress.DataAccess.Native.Data;
using DevExpress.XtraEditors;
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

            LoadData();
        }


        private void labelControl4_Click(object sender, EventArgs e)
        {

        }

        private void LoadData()
        {
            PeriodTypeBox.Enabled = false;
            SecondFilterBox.Enabled = false;

            var years = _repo.GetDistinctYear().ToList();
            foreach (var year in years)
                YearBox.Properties.Items.Add(year.ToString());

            // Default: show received orders
            var receivedItems = _repo.GetAllReceivedOrders().ToList();

            var now = DateTime.Now;
            decimal totalCost = receivedItems
                .Where(i => i.DateReceived.Month == now.Month && i.DateReceived.Year == now.Year)
                .Sum(i => i.ReceivedAmount);

            int totalItems = receivedItems
                .Where(i => i.DateReceived.Month == now.Month && i.DateReceived.Year == now.Year)
                .Sum(i => i.ReceivedQuantity);

            totalMonthlyCostLBL.Text = totalCost.ToString("N2");
            totalMonthlyItemsLBL.Text = totalItems.ToString();

            _CurrentFilteredData = receivedItems;
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

                XtraReport report = null;

                switch (FilterBox.Text)
                {
                    case "Purchased Orders":
                        report = new PurchaseReport(_CurrentFilteredData.Cast<PurchaseOrderBrief>());
                        break;

                    case "Received Orders":
                        report = new ReceivedReport(_CurrentFilteredData.Cast<ReceivedItemReports>());
                        break;

                    case "Capitalized Orders":
                        report = new CapitalizedReport(_CurrentFilteredData.Cast<CapitalizedOrder>());
                        break;

                    case "Reconciliation":
                        report = new ReconciliationReport(_CurrentFilteredData.Cast<Reconciliation>());
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
                    _CurrentFilteredData = _repo.GetPurchaseOrderBrief().ToList();
                    ReportGC.DataSource = _CurrentFilteredData;
                    YearBox.Clear();
                    PeriodTypeBox.Clear();
                    PeriodTypeBox.Enabled = false;
                    SecondFilterBox.Clear();
                    YearBox.Text = "SELECT YEAR";
                    PeriodTypeBox.Text = "SELECT PERIOD TYPE";
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

                    if (ReportGV.Columns["AttachmentData"] != null)
                        ReportGV.Columns["AttachmentData"].Visible = false;

                    if (ReportGV.Columns["AttachmentPreview"] == null)
                        AddAttachmentImageColumn();
                    break;
            }
        }

        private void AddAttachmentImageColumn()
        {
            // Button editor that shows "View Image" text
            var btnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            btnEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            btnEdit.Buttons.Clear();
            btnEdit.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(
                DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
            {
                Caption = "View Image"
            });

            // Wire up the click
            btnEdit.ButtonClick += AttachmentButtonEdit_ButtonClick;

            ReportGC.RepositoryItems.Add(btnEdit);

            var imgCol = new DevExpress.XtraGrid.Columns.GridColumn
            {
                FieldName = "AttachmentData",
                Caption = "Attachment",
                Name = "AttachmentPreview",
                Visible = true,
                VisibleIndex = ReportGV.Columns.Count,
                ColumnEdit = btnEdit
            };

            ReportGV.Columns.Add(imgCol);
        }

        private void AttachmentButtonEdit_ButtonClick(object sender,
    DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // Get the focused row's data
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
            {
                using (var ms = new System.IO.MemoryStream(imageData))
                {
                    var image = Image.FromStream(ms);

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
            if (PeriodTypeBox.SelectedIndex != -1 && string.IsNullOrEmpty(SecondFilterBox.Text))
            {
                int y = int.Parse(YearBox.Text);
                string col = (FilterBox.Text == "Received Orders" || FilterBox.Text == "Capitalized Orders")
                    ? "DateReceived" : "OrderDate";
                ReportGV.ActiveFilterString =
                    $"[{col}] >= #{new DateTime(y, 1, 1):MM/dd/yyyy}# AND [{col}] <= #{new DateTime(y, 12, 31):MM/dd/yyyy}#";
                return;
            }

            int year = int.Parse(YearBox.Text);
            DateTime start, end;

            string DateColumn = (FilterBox.Text == "Received Orders" || FilterBox.Text == "Capitalized Orders")
                ? "DateReceived" : "OrderDate";

            switch (PeriodTypeBox.SelectedIndex)
            {
                case 0: // Monthly
                    int month = DateTime.ParseExact(SecondFilterBox.Text, "MMMM",
                                System.Globalization.CultureInfo.InvariantCulture).Month;
                    start = new DateTime(year, month, 1);
                    end = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    break;

                case 1: // Quarterly
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

            ReportGV.ActiveFilterString =
                $"[{DateColumn}] >= #{start:MM/dd/yyyy}# AND [{DateColumn}] <= #{end:MM/dd/yyyy}#";
        }

        private void YearBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            SecondFilterBox.Properties.Items.Clear();
            SecondFilterBox.Enabled = false;
            SecondFilterBox.Visible = true;
            ReportGV.ActiveFilterString = string.Empty;

            if (!string.IsNullOrEmpty(YearBox.Text))
            {
                PeriodTypeBox.Enabled = true;
                ApplyPeriodFilter();
            }
        }

        private void PeriodTypeBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            SecondFilterBox.Properties.Items.Clear();
            SecondFilterBox.Enabled = false;
            ReportGV.ActiveFilterString = string.Empty;

            if (!string.IsNullOrEmpty(YearBox.Text))
            {
                PopulationSecondFilter(int.Parse(YearBox.Text));
            }
        }

        private void SecondFilterBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            ApplyPeriodFilter();
        }
    }


}

