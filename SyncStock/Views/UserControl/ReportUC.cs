using DevExpress.DataAccess.Native.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using SyncStock.Database;
using SyncStock.Models;
using SyncStock.Models.Reports;
using SyncStock.PrintForm;
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
                var report = new CapitalizedReport();           
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

                case "Received Orders":
                    ReportGC.DataSource = _repo.GetAllReceivedOrders().ToList();
                    break;

                case "Capitalized Orders":
                    ReportGC.DataSource = _repo.GetAllCapitalizedOrder().ToList();
                    break;

                case "Reconciliation":
                    var data = _repo.GetReconciliationItems().ToList();
                    ReportGC.DataSource = data;

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
            var row = ReportGV.GetFocusedRow() as ReconcilationItem;

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

        private void PeriodTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecondFilterBox.Properties.Items.Clear();
            var years = _repo.GetDistinctYear().ToList();

            switch (PeriodTypeBox.SelectedIndex) 
            {
                case 0:
                    SecondFilterBox.Text = "Monthly";
                    foreach (var year in years) 
                    {
                        for (int month = 1; month <= 12; month++) 
                        {
                            string monthName = new DateTime(year, month, 1).ToString("MMMM yyyy");
                            SecondFilterBox.Properties.Items.Add(monthName);
                        }
                    }
                    break;
                case 1:
                    SecondFilterBox.Text = "Quarterly";
                    foreach (var year in years) 
                    {
                        SecondFilterBox.Properties.Items.Add($"Q1 {year}");
                        SecondFilterBox.Properties.Items.Add($"Q2 {year}");
                        SecondFilterBox.Properties.Items.Add($"Q3 {year}");
                        SecondFilterBox.Properties.Items.Add($"Q4 {year}");
                    }
                    break;
                case 2:
                    SecondFilterBox.Text = "Yearly";
                    foreach (var year in years) 
                    {
                        SecondFilterBox.Properties.Items.Add(year.ToString());
                    }
                    break;
            }
        }

        private void ApplyPeriodFilter() 
        {
            if (string.IsNullOrEmpty(SecondFilterBox.Text)) return;

            string DateColumn = (FilterBox.Text == "Received Orders" || FilterBox.Text == "Capitalized Orders")
                ? "DateReceived"
                : "OrderDate";
            DateTime start, end;

            switch (PeriodTypeBox.Text) 
            {
                case "Monthly":
                    DateTime SelectedMonth = DateTime.ParseExact(SecondFilterBox.Text, "MMMM yyyy", null);
                    start = new DateTime(SelectedMonth.Year, SelectedMonth.Month, 1);
                    end = new DateTime(SelectedMonth.Year, SelectedMonth.Month, DateTime.DaysInMonth(SelectedMonth.Year, SelectedMonth.Month));
                    break;

                case "Quarterly":
                    string[] Parts = SecondFilterBox.Text.Split(' ');
                    string Quarter = Parts[0];
                    int QYear = int.Parse(Parts[1]);
                    int StartMonth, EndMonth;
                    switch (Quarter)
                    {
                        case "Q1": StartMonth = 1; EndMonth = 3; break;
                        case "Q2": StartMonth = 4; EndMonth = 6; break;
                        case "Q3": StartMonth = 7; EndMonth = 9; break;
                        case "Q4": StartMonth = 10; EndMonth = 12; break;
                        default: return;
                    }
                    start = new DateTime(QYear, StartMonth, 1);
                    end = new DateTime(QYear, EndMonth, DateTime.DaysInMonth(QYear, EndMonth));
                    break;

                case "Yearly":
                    int SelectedYear = int.Parse(SecondFilterBox.Text);
                    start = new DateTime(SelectedYear, 1, 1);
                    end = new DateTime(SelectedYear, 12, 31);
                    break;

                default:
                    ReportGV.ActiveFilterString = string.Empty;
                    return;

            }

            ReportGV.ActiveFilterString = $"[{DateColumn}] >= #{start:MM/dd/yyyy}# AND [{DateColumn}] <= #{end:MM/dd/yyyy}#";
        }

        private void SecondFilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyPeriodFilter();
        }
    }

        

       

       
    }

