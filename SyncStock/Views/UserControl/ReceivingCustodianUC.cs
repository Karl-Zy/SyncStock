using DevExpress.XtraEditors;
using SyncStock.Database;
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
        private Repository _repo = new Repository();
        private int _purchaseOrderId = 0;
        private byte[] _uploadedFileBytes = null;
        private string _uploadedFileName = null;

        // Allowed file types for attachment uploads
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

        public ReceivingCustodianUC()
        {
            InitializeComponent();
            _repo = new Repository();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode) // Prevents the database from running inside the VS Designer
            {
                LoadDataFromRepository();
            }
        }

        private void LoadDataFromRepository()
        {
            try
            {
                var incomingItems = _repo.GetPendingIncomingItemsDetails();
                gcItems.DataSource = incomingItems;
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    $"Failed to load pending items: {ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

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

                    // Dynamic Header Labels Update
                    lblReceivingReport.Text = $"Receiving Report From: {department}";
                    lblPONumber.Text = $"PO Number: {poNumber}";

                    // Display Data inside Read-Only Form Fields
                    txteditItemName.Text = itemName;
                    txteditExpectedQuan.Text = qty?.ToString();

                    // Format Expected Amount cleanly with commas/decimals
                    if (amount != null && decimal.TryParse(amount.ToString(), out decimal parsedAmount))
                        txteditExpectedAmount.Text = string.Format("{0:N2}", parsedAmount);
                    else
                        txteditExpectedAmount.Text = "0.00";

                    // Leave User-Input fields open/ready for custodian input
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

                    // Strict file type validation
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

                    // Strict file type validation
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

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            // Validation Guards
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
                _repo.UpdatePurchaseOrderItemStatus(purePoNumber, txteditItemName.Text, "Received");

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
            // Reset layout title block structures back to raw defaults
            lblReceivingReport.Text = "Receiving Report From:";
            lblPONumber.Text = "PO Number:";

            // Reset uneditable visual labels
            txteditItemName.Text = "";
            txteditExpectedQuan.Text = "";
            txteditExpectedAmount.Text = "0.00";

            // Clear user interaction elements safely
            dateEdit.EditValue = null;
            chckboxAsset.Checked = false;
            spneditReceivedQuan.EditValue = 0;
            txteditReceivedAmount.Text = "";
            txteditRemarks.Text = "";

            // Reset upload state completely
            _uploadedFileBytes = null;
            _uploadedFileName = null;
            lblUploadGuide.Text = "or drop file here";
        }
    }
}