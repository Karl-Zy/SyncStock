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
        private string selectedFilePath = string.Empty;
        private int _purchaseOrderId = 0;

        public ReceivingCustodianUC()
        {
            InitializeComponent();

            // 2. Initialize the repository
            _repo = new Repository();
            LoadPurchaseOrderItems();

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
                // Call the new flat-join method
                var incomingItems = _repo.GetPendingIncomingItemsDetails();

                // Bind the list directly to your GridControl container
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
                    // 1. Change "Qty" to "Quantity" to perfectly match your model property name
                    string poNumber = gvItemsView.GetRowCellValue(rowHandle, "PONumber")?.ToString();
                    string department = gvItemsView.GetRowCellValue(rowHandle, "Department")?.ToString();
                    string itemName = gvItemsView.GetRowCellValue(rowHandle, "ItemName")?.ToString();
                    object qty = gvItemsView.GetRowCellValue(rowHandle, "Quantity"); // <--- Fixed here
                    object amount = gvItemsView.GetRowCellValue(rowHandle, "Amount");

                    // 2. Dynamic Header Labels Update
                    lblReceivingReport.Text = $"Receiving Report From: {department}";
                    lblPONumber.Text = $"PO Number: {poNumber}";

                    // 3. Display Data inside your Read-Only Form Fields
                    txteditItemName.Text = itemName;
                    txteditExpectedQuan.Text = qty?.ToString();

                    // Format Expected Amount cleanly with commas/decimals
                    if (amount != null && decimal.TryParse(amount.ToString(), out decimal parsedAmount))
                    {
                        txteditExpectedAmount.Text = string.Format("{0:N2}", parsedAmount);
                    }
                    else
                    {
                        txteditExpectedAmount.Text = "0.00";
                    }

                    // 4. Leave User-Input fields open/ready for custodian input
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

        private void LoadPurchaseOrderItems()
        {
            if (_purchaseOrderId == 0) return;

            var items = _repo.GetItemsByPurchaseOrder(_purchaseOrderId);
            gcItems.DataSource = items.ToList(); // ← your GridControl name
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            // Setup the file filter for asset management tracking
            openFileDialogReceipt.Title = "Select Proof of Delivery Receipt or Photo";
            openFileDialogReceipt.Filter = "Image & PDF Files|*.jpg;*.jpeg;*.png;*.pdf|All Files|*.*";
            openFileDialogReceipt.FilterIndex = 1;
            openFileDialogReceipt.RestoreDirectory = true;
                
            // Open the file browser dialog window
            if (openFileDialogReceipt.ShowDialog() == DialogResult.OK)
            {
                // FIX: Assign the file name to the tracking variable cleanly
                selectedFilePath = openFileDialogReceipt.FileName;

                // Extract just the file name to display inside the label UI card
                string fileNameOnly = System.IO.Path.GetFileName(selectedFilePath);

                lblUploadGuide.Text = $"Selected: {fileNameOnly}";

                try
                {
                    // TEAM FIX: Define a relative folder inside your project build output
                    string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
                    string targetFolder = System.IO.Path.Combine(projectFolder, "UploadedProofs");

                    // Automatically build the 'UploadedProofs' directory if it doesn't exist on your classmate's PC
                    if (!System.IO.Directory.Exists(targetFolder))
                    {
                        System.IO.Directory.CreateDirectory(targetFolder);
                    }

                    // Create the full target destination path string
                    string destinationPath = System.IO.Path.Combine(targetFolder, fileNameOnly);

                    // Copy the file securely over to the shared project destination directory
                    System.IO.File.Copy(selectedFilePath, destinationPath, true);

                    // OPTIONAL: Store destinationPath into your SQL string parameters 
                    // when saving to your shared Dapper repository database module!
                    System.Diagnostics.Debug.WriteLine($"File saved to team path: {destinationPath}");
                }
                catch (Exception ex)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show($"Directory creation or file save failure: {ex.Message}");
                }
            }
        }

        private void btnUpload_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the item being hovered over the button is an actual file element
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // Changes the cursor to show a "+" copy icon
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void btnUpload_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // 1. Extract the file paths dropped onto the button surface
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files != null && files.Length > 0)
                {
                    // 2. Target the first file path string dropped
                    selectedFilePath = files[0];

                    // 3. Update your label indicator card text to confirm the upload
                    string fileNameOnly = System.IO.Path.GetFileName(selectedFilePath);
                    lblUploadGuide.Text = $"Dropped: {fileNameOnly}";

                    // 4. Relative directory handler for your team members
                    string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
                    string targetFolder = System.IO.Path.Combine(projectFolder, "UploadedProofs");

                    if (!System.IO.Directory.Exists(targetFolder))
                    {
                        System.IO.Directory.CreateDirectory(targetFolder);
                    }

                    string destinationPath = System.IO.Path.Combine(targetFolder, fileNameOnly);

                    // 5. Duplicate the item securely into your shared project runtime container folder
                    System.IO.File.Copy(selectedFilePath, destinationPath, true);

                    System.Diagnostics.Debug.WriteLine($"File dropped onto button and saved: {destinationPath}");
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show($"File drop ingestion error: {ex.Message}");
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            // Validation Guards
            if (string.IsNullOrEmpty(lblPONumber.Text) || lblPONumber.Text == "PO Number:")
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Validation Error: Please select a pending item from the table before confirming.", "Missing Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dateEdit.EditValue == null || string.IsNullOrWhiteSpace(dateEdit.Text))
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Validation Error: 'Date Received' is required.", "Missing Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateEdit.Focus();
                return;
            }
            if (spneditReceivedQuan.EditValue == null || Convert.ToInt32(spneditReceivedQuan.EditValue) <= 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Validation Error: 'Received Quantity' must be greater than 0.", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                spneditReceivedQuan.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txteditReceivedAmount.Text) || !decimal.TryParse(txteditReceivedAmount.Text, out decimal parsedReceivedAmount) || parsedReceivedAmount < 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Validation Error: Please enter a valid, non-negative 'Received Amount'.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    AttachmentPath = selectedFilePath,
                    Remarks = txteditRemarks.Text
                };

                // Save entry and update its status so it drops out of 'Pending'
                _repo.AddConfirmedItem(confirmationPayload);
                _repo.UpdatePurchaseOrderItemStatus(purePoNumber, txteditItemName.Text, "Received");

                DevExpress.XtraEditors.XtraMessageBox.Show("Asset inventory ledger updated and item confirmed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearReceivingForm();
                LoadDataFromRepository();
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show($"Inventory database submission failed: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearReceivingForm();
        }

        private void ClearReceivingForm()
        {
            // Reset global path variable tracking string reference
            selectedFilePath = string.Empty;

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

            // Reset drop zone file reference string guide card label
            lblUploadGuide.Text = "or drop file here";
        }
    }
}
