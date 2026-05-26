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

                case "Received Orders":
                    ReportGC.DataSource = _repo.GetAllReceivedOrders().ToList();
                    break;

                case "Reconciliation":                               // ← only ONE of these
                    {
                        var data = _repo.GetReconciliationItems().ToList();
                        ReportGC.DataSource = data;

                        if (ReportGV.Columns["AttachmentData"] != null)
                            ReportGV.Columns["AttachmentData"].Visible = false;

                        if (ReportGV.Columns["AttachmentPreview"] == null)
                            AddAttachmentImageColumn();

                        break;
                    }

                case "Capitalized Orders":
                    ReportGC.DataSource = _repo.GetAllCapitalizedOrder().ToList();
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
    }
}
