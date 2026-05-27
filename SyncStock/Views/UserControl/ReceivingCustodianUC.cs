using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Items;
using SyncStock.Database;
using SyncStock.Models;
using SyncStock.Models.Models_Receiving_;
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
    public partial class ReceivingCustodianUC : DevExpress.XtraEditors.XtraUserControl
    {
        private readonly Repository _repo = new Repository();
        private byte[] _uploadedFileBytes = null;
        private string _uploadedFileName = null;

        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

        public ReceivingCustodianUC()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                ApplyGridStyling();
                LoadDataFromRepository();
            }
        }

        // ─────────────────────────────────────────────
        // GRID STYLING
        // ─────────────────────────────────────────────

        private void ApplyGridStyling()
        {
            gvItemsView.OptionsView.EnableAppearanceOddRow = true;
            gvItemsView.OptionsView.EnableAppearanceEvenRow = true;
            gvItemsView.Appearance.OddRow.BackColor = Color.FromArgb(245, 250, 248);
            gvItemsView.Appearance.EvenRow.BackColor = Color.White;

            gvItemsView.Appearance.FocusedRow.BackColor = Color.FromArgb(144, 238, 144);
            gvItemsView.Appearance.FocusedRow.ForeColor = Color.FromArgb(30, 30, 30);
            gvItemsView.Appearance.HideSelectionRow.BackColor = Color.FromArgb(198, 239, 206);

            gvItemsView.RowHeight = 32;

            gvItemsView.OptionsView.ShowGroupPanel = false;
            gvItemsView.OptionsView.ColumnAutoWidth = true;
            gvItemsView.OptionsSelection.EnableAppearanceFocusedCell = false;
        }

        private void ApplyColumnAlignment()
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvItemsView.Columns)
            {
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Font = new Font("Segoe UI", 9f, FontStyle.Bold);

                if (col.FieldName == "Amount" || col.FieldName == "Quantity" ||
                    col.FieldName == "ItemID" || col.FieldName == "PurchaseOrderID")
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                else
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            // Format Amount as currency
            var amountCol = gvItemsView.Columns["Amount"];
            if (amountCol != null)
            {
                amountCol.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                amountCol.DisplayFormat.FormatString = "₱{0:N2}";
                amountCol.Width = 120;
            }

            // Hide raw POType — show friendly OrderType instead
            var poTypeCol = gvItemsView.Columns["POType"];
            if (poTypeCol != null)
                poTypeCol.Visible = false;

            var orderTypeCol = gvItemsView.Columns["OrderType"];
            if (orderTypeCol != null)
            {
                orderTypeCol.Caption = "Order Type";
                orderTypeCol.Width = 100;
            }
        }

        // ─────────────────────────────────────────────
        // DATA LOADING
        // ─────────────────────────────────────────────

        private void LoadDataFromRepository()
        {
            try
            {
                var incomingItems = _repo.GetPendingIncomingItemsDetails();
                gcItems.DataSource = incomingItems;
                ApplyColumnAlignment();
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    $"Failed to load pending items: {ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────
        // GRID ROW SELECTION
        // ─────────────────────────────────────────────

        private void gvItemsView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            int rowHandle = e.FocusedRowHandle;

            if (gvItemsView.IsValidRowHandle(rowHandle))
            {
                try
                {
                    string poNumber = gvItemsView.GetRowCellValue(rowHandle, "PONumber")?.ToString();
                    string department = gvItemsView.GetRowCellValue(rowHandle, "Department")?.ToString();
                    string itemName = gvItemsView.GetRowCellValue(rowHandle, "ItemName")?.ToString();
                    object qty = gvItemsView.GetRowCellValue(rowHandle, "Quantity");
                    object amount = gvItemsView.GetRowCellValue(rowHandle, "Amount");

                    lblReceivingReport.Text = $"Receiving Report From: {department}";
                    lblPONumber.Text = $"PO Number: {poNumber}";

                    txteditItemName.Text = itemName;
                    txteditExpectedQuan.Text = qty?.ToString();

                    if (amount != null && decimal.TryParse(amount.ToString(), out decimal parsedAmount))
                        txteditExpectedAmount.Text = string.Format("{0:N2}", parsedAmount);
                    else
                        txteditExpectedAmount.Text = "0.00";

                    dateEdit.EditValue = DateTime.Today;
                    spneditReceivedQuan.EditValue = qty;
                    txteditReceivedAmount.Text = "";
                    txteditRemarks.Text = "";
                    chckboxAsset.Checked = false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error assigning grid values to form: {ex.Message}");
                }
            }
        }

        // ─────────────────────────────────────────────
        // SEARCH — PO Number, Item Name, Order Type
        // ─────────────────────────────────────────────

        private void searchControl_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchControl.Text?.Trim().ToLower() ?? string.Empty;
            var allItems = _repo.GetPendingIncomingItemsDetails();

            // Empty search — show everything
            if (string.IsNullOrWhiteSpace(searchText))
            {
                gcItems.DataSource = allItems.ToList();
                ApplyColumnAlignment();
                return;
            }

            // Multi-keyword support — e.g. "laptop online" matches both words
            string[] keywords = searchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            gcItems.DataSource = allItems.Where(x =>
            {
                string po = x.PONumber?.ToLower() ?? string.Empty;
                string item = x.ItemName?.ToLower() ?? string.Empty;
                string orderType = x.OrderType?.ToLower() ?? string.Empty; // "local" or "online"

                // Every keyword must match at least one of the three fields
                return keywords.All(k =>
                    po.Contains(k) ||
                    item.Contains(k) ||
                    orderType.Contains(k)
                );
            }).ToList();

            ApplyColumnAlignment();
        }

        // ─────────────────────────────────────────────
        // FILE UPLOAD
        // ─────────────────────────────────────────────

        private void btnUpload_Click(object sender, EventArgs e)
        {
            openFileDialogReceipt.Title = "Select Proof of Delivery Receipt or Photo";
            openFileDialogReceipt.Filter = "Image & PDF Files|*.jpg;*.jpeg;*.png;*.pdf|All Files|*.*";
            openFileDialogReceipt.FilterIndex = 1;
            openFileDialogReceipt.RestoreDirectory = true;

            if (openFileDialogReceipt.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string selectedFilePath = openFileDialogReceipt.FileName;
                    string fileExtension = System.IO.Path.GetExtension(selectedFilePath).ToLower();

                    if (!_allowedExtensions.Contains(fileExtension))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(
                            "Invalid file type. Only JPG, JPEG, PNG, and PDF files are allowed.",
                            "Invalid File",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    _uploadedFileBytes = System.IO.File.ReadAllBytes(selectedFilePath);
                    _uploadedFileName = System.IO.Path.GetFileName(selectedFilePath);
                    lblUploadGuide.Text = $"Selected: {_uploadedFileName}";
                }
                catch (Exception ex)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        $"File read failure: {ex.Message}",
                        "Upload Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void btnUpload_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void btnUpload_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files != null && files.Length > 0)
                {
                    string droppedFilePath = files[0];
                    string fileExtension = System.IO.Path.GetExtension(droppedFilePath).ToLower();

                    if (!_allowedExtensions.Contains(fileExtension))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(
                            "Invalid file type. Only JPG, JPEG, PNG, and PDF files are allowed.",
                            "Invalid File",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    _uploadedFileBytes = System.IO.File.ReadAllBytes(droppedFilePath);
                    _uploadedFileName = System.IO.Path.GetFileName(droppedFilePath);
                    lblUploadGuide.Text = $"Dropped: {_uploadedFileName}";
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    $"File drop error: {ex.Message}",
                    "Drop Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────
        // CONFIRM / CANCEL
        // ─────────────────────────────────────────────

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblPONumber.Text) || lblPONumber.Text == "PO Number:")
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Validation Error: Please select a pending item from the table before confirming.",
                    "Missing Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            if (dateEdit.EditValue == null || string.IsNullOrWhiteSpace(dateEdit.Text))
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Validation Error: 'Date Received' is required.",
                    "Missing Date",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                dateEdit.Focus();
                return;
            }
            if (spneditReceivedQuan.EditValue == null || Convert.ToInt32(spneditReceivedQuan.EditValue) <= 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Validation Error: 'Received Quantity' must be greater than 0.",
                    "Invalid Quantity",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                spneditReceivedQuan.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txteditReceivedAmount.Text) ||
                !decimal.TryParse(txteditReceivedAmount.Text, out decimal parsedReceivedAmount) ||
                parsedReceivedAmount < 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Validation Error: Please enter a valid, non-negative 'Received Amount'.",
                    "Invalid Amount",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txteditReceivedAmount.Focus();
                return;
            }

            try
            {
                string purePoNumber = lblPONumber.Text.Replace("PO Number:", "").Trim();

                var confirmationPayload = new ConfirmedItems
                {
                    PONumber = purePoNumber,
                    ItemName = txteditItemName.Text,
                    DateReceived = Convert.ToDateTime(dateEdit.EditValue),
                    IsCapitalizable = chckboxAsset.Checked,
                    ExpectedQuantity = Convert.ToInt32(txteditExpectedQuan.Text),
                    ReceivedQuantity = Convert.ToInt32(spneditReceivedQuan.EditValue),
                    ExpectedAmount = Convert.ToDecimal(txteditExpectedAmount.Text),
                    ReceivedAmount = parsedReceivedAmount,
                    Remarks = txteditRemarks.Text,
                    AttachmentData = _uploadedFileBytes,
                    AttachmentFileName = _uploadedFileName
                };

                _repo.AddConfirmedItem(confirmationPayload);

                // ✅ Fixed — passes itemName so only THIS item's PO status updates
                _repo.UpdatePurchaseOrderItemStatus(
                    purePoNumber,
                    txteditItemName.Text,
                    WorkflowStatus.Received);

                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Asset inventory ledger updated and item confirmed successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearReceivingForm();
                LoadDataFromRepository();
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    $"Inventory database submission failed: {ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearReceivingForm();
        }

        private void ClearReceivingForm()
        {
            lblReceivingReport.Text = "Receiving Report From:";
            lblPONumber.Text = "PO Number:";

            txteditItemName.Text = "";
            txteditExpectedQuan.Text = "";
            txteditExpectedAmount.Text = "0.00";

            dateEdit.EditValue = null;
            chckboxAsset.Checked = false;
            spneditReceivedQuan.EditValue = 0;
            txteditReceivedAmount.Text = "";
            txteditRemarks.Text = "";

            _uploadedFileBytes = null;
            _uploadedFileName = null;
            lblUploadGuide.Text = "or drop file here";
        }
    }
}