using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Items;
using DevExpress.XtraReports.Design;
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
        private string _currentSearchText = string.Empty;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
        private bool _userClickedRow = false;

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
                txteditReceivedAmount.GotFocus += (s, me) => BeginInvoke(new Action(() => txteditReceivedAmount.SelectAll()));
                txteditReceivedAmount.KeyPress += txteditReceivedAmount_KeyPress;
                txteditReceivedAmount.Leave += txteditReceivedAmount_Leave;
                lcMainContainer.AllowCustomization = false;
                dlcPendingIncoming.AllowCustomization = false;
                dlcReceivingReport.AllowCustomization = false;
                dlcPendingIncoming.MouseDown += (s, me) => txteditReceivedAmount_Leave(null, EventArgs.Empty);
                dlcReceivingReport.MouseDown += (s, me) => txteditReceivedAmount_Leave(null, EventArgs.Empty);
                pnlPendingIncoming.MouseDown += (s, me) => txteditReceivedAmount_Leave(null, EventArgs.Empty);
                pnlReceivingReport.MouseDown += (s, me) => txteditReceivedAmount_Leave(null, EventArgs.Empty);
                scrlControl.MouseDown += (s, me) => txteditReceivedAmount_Leave(null, EventArgs.Empty);
                dateEdit.EditValueChanged += (s, me) => UpdateRequiredLabels();
                spneditReceivedQuan.EditValueChanged += (s, me) => UpdateRequiredLabels();
                txteditReceivedAmount.EditValueChanged += (s, me) => UpdateRequiredLabels();
                txteditReceivedAmount.Leave += (s, me) => UpdateRequiredLabels();
                gvItemsView.RowClick += (s, me) => _userClickedRow = true;
                lblDateReceived.AllowHtmlStringInCaption = true;
                lblReceivedQuan.AllowHtmlStringInCaption = true;
                lblReceivedAmount.AllowHtmlStringInCaption = true;
                this.MouseWheel += (s, me) => ScrollForm(-me.Delta);
                scrlControl.MouseWheel += (s, me) => ScrollForm(-me.Delta);
                spneditReceivedQuan.KeyPress += (s, me) =>
                {
                    if (me.KeyChar == '-')
                        me.Handled = true;
                };
                HookMouseWheel(this);
                UpdateRequiredLabels();
            }
        }

        private void UpdateRequiredLabels()
        {
            lblDateReceived.AppearanceItemCaption.ForeColor = Color.Black;
            lblReceivedQuan.AppearanceItemCaption.ForeColor = Color.Black;
            lblReceivedAmount.AppearanceItemCaption.ForeColor = Color.Black;

            lblDateReceived.Text = (dateEdit.EditValue == null || string.IsNullOrWhiteSpace(dateEdit.Text))
                ? "Date Received <color=Crimson>*</color>"
                : "Date Received";
            lblReceivedQuan.Text = (spneditReceivedQuan.EditValue == null || Convert.ToInt32(spneditReceivedQuan.EditValue) <= 0)
                ? "Received Quantity <color=Crimson>*</color>"
                : "Received Quantity";
            lblReceivedAmount.Text = string.IsNullOrWhiteSpace(txteditReceivedAmount.Text)
                ? "Received Amount <color=Crimson>*</color>"
                : "Received Amount";
        }

        private void ScrollForm(int delta)
        {
            Point current = scrlControl.AutoScrollPosition;
            scrlControl.AutoScrollPosition = new Point(0, -current.Y + delta);
        }

        private void HookMouseWheel(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is DevExpress.XtraGrid.GridControl) continue;

                child.MouseWheel += (s, me) => ScrollForm(-me.Delta);
                HookMouseWheel(child);
            }
        }

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

        private void gvItemsView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (!_userClickedRow) return;
            _userClickedRow = false;

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
                        txteditExpectedAmount.Text = string.Format("₱{0:N2}", parsedAmount);
                    else
                        txteditExpectedAmount.Text = "₱0.00";

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

        private void searchControl_TextChanged(object sender, EventArgs e)
        {
            _currentSearchText = searchControl.Text?.Trim() ?? string.Empty;
            string searchLower = _currentSearchText.ToLower();

            var allItems = _repo.GetPendingIncomingItemsDetails();

            if (string.IsNullOrWhiteSpace(searchLower))
            {
                gcItems.DataSource = allItems.ToList();
                ApplyColumnAlignment();
                gvItemsView.RefreshData();
                return;
            }

            string[] keywords = searchLower.Split(
                new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            gcItems.DataSource = allItems.Where(x =>
            {
                string po = x.PONumber?.ToLower() ?? string.Empty;
                string item = x.ItemName?.ToLower() ?? string.Empty;
                string orderType = x.OrderType?.ToLower() ?? string.Empty;
                string orderMode = x.OrderModeDisplay?.ToLower() ?? string.Empty;
                string dept = x.Department?.ToLower() ?? string.Empty;

                return keywords.All(k =>
                    po.Contains(k) ||
                    item.Contains(k) ||
                    orderType.Contains(k) ||
                    orderMode.Contains(k) ||
                    dept.Contains(k)
                );
            }).ToList();

            ApplyColumnAlignment();
            gvItemsView.RefreshData();
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

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblPONumber.Text) || lblPONumber.Text == "PO Number:")
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Validation Error: Please select a pending item from the table before confirming.",
                    "Missing Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dateEdit.EditValue == null || string.IsNullOrWhiteSpace(dateEdit.Text))
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Validation Error: 'Date Received' is required.",
                    "Missing Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateEdit.Focus();
                return;
            }

            if (spneditReceivedQuan.EditValue == null ||
                Convert.ToInt32(spneditReceivedQuan.EditValue) <= 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Validation Error: 'Received Quantity' must be greater than 0.",
                    "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                spneditReceivedQuan.Focus();
                return;
            }

            string rawAmount = txteditReceivedAmount.Text.Replace("₱", "").Replace(",", "").Trim();
            if (string.IsNullOrWhiteSpace(rawAmount) ||
                !decimal.TryParse(rawAmount, out decimal parsedReceivedAmount) ||
                parsedReceivedAmount < 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Validation Error: Please enter a valid, non-negative 'Received Amount'.",
                    "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    ExpectedAmount = decimal.Parse(txteditExpectedAmount.Text.Replace("₱", "").Replace(",", "").Trim()),
                    ReceivedAmount = parsedReceivedAmount,

                    Remarks = txteditRemarks.Text,
                    AttachmentData = _uploadedFileBytes,
                    AttachmentFileName = _uploadedFileName
                };

                _repo.AddConfirmedItem(confirmationPayload);

                _repo.UpdatePurchaseOrderItemStatus(
                    purePoNumber,
                    txteditItemName.Text,
                    WorkflowStatus.Received);

                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Asset inventory ledger updated and item confirmed successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearReceivingForm();
                LoadDataFromRepository();
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    $"Inventory database submission failed: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ClearReceivingForm()
        {
            lblReceivingReport.Text = "Receiving Report From:";
            lblPONumber.Text = "PO Number:";

            txteditItemName.Text = "";
            txteditExpectedQuan.Text = "";
            txteditExpectedAmount.Text = "₱0.00";

            dateEdit.EditValue = null;
            chckboxAsset.Checked = false;
            spneditReceivedQuan.EditValue = 0;
            txteditReceivedAmount.Text = "";
            txteditRemarks.Text = "";

            _uploadedFileBytes = null;
            _uploadedFileName = null;
            lblUploadGuide.Text = "or drop file here";
        }

        private void ApplyGridStyling()
        {
            gvItemsView.OptionsCustomization.AllowColumnMoving = false;
            gvItemsView.OptionsCustomization.AllowColumnResizing = false;

            gvItemsView.OptionsView.EnableAppearanceOddRow = true;
            gvItemsView.OptionsView.EnableAppearanceEvenRow = true;
            gvItemsView.Appearance.OddRow.BackColor = Color.FromArgb(245, 250, 248);
            gvItemsView.Appearance.EvenRow.BackColor = Color.White;

            gvItemsView.Appearance.FocusedRow.BackColor = Color.FromArgb(83, 237, 126);
            gvItemsView.Appearance.FocusedRow.ForeColor = Color.FromArgb(30, 30, 30);
            gvItemsView.Appearance.HideSelectionRow.BackColor = Color.FromArgb(198, 239, 206);

            gvItemsView.RowHeight = 32;

            gvItemsView.OptionsView.ShowGroupPanel = false;
            gvItemsView.OptionsView.ColumnAutoWidth = true;
            gvItemsView.OptionsSelection.EnableAppearanceFocusedCell = false;

            gvItemsView.CustomDrawCell += gvItemsView_CustomDrawCell;
        }

        private void ApplyColumnAlignment()
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvItemsView.Columns)
                col.Visible = false;

            var columnOrder = new[]
            {
                ("PONumber",         "PO Number",    80),
                ("Department",       "Department",   160),
                ("ItemName",         "Item Name",    140),
                ("Quantity",         "Quantity",     80),
                ("Amount",           "Amount",       120),
                ("OrderModeDisplay", "Order Mode",   120),
                ("OrderType",        "Order Type",   100),
                ("DateOrdered",      "Date Ordered", 110),
            };

            foreach (var (field, caption, width) in columnOrder)
            {
                var col = gvItemsView.Columns[field];
                if (col == null) continue;
                col.Caption = caption;
                col.Width = width;
                col.Visible = true;
            }

            for (int i = 0; i < columnOrder.Length; i++)
            {
                var col = gvItemsView.Columns[columnOrder[i].Item1];
                if (col != null)
                    col.VisibleIndex = i;
            }

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvItemsView.Columns)
            {
                if (!col.Visible) continue;

                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Font = new Font("Segoe UI", 9f, FontStyle.Bold);

                if (col.FieldName == "Amount" || col.FieldName == "Quantity")
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                else
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            var amountCol = gvItemsView.Columns["Amount"];
            if (amountCol != null)
            {
                amountCol.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                amountCol.DisplayFormat.FormatString = "₱{0:N2}";
            }
        }

        private void gvItemsView_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentSearchText)) return;

            string cellText = e.DisplayText;
            string search = _currentSearchText.ToLower();
            int matchIdx = cellText?.ToLower().IndexOf(search) ?? -1;

            if (matchIdx < 0) return;

            e.Graphics.FillRectangle(
                new SolidBrush(e.Appearance.BackColor), e.Bounds);

            Rectangle bounds = e.Bounds;

            string before = cellText.Substring(0, matchIdx);
            string matched = cellText.Substring(matchIdx, search.Length);

            Font font = e.Appearance.GetFont();
            SizeF beforeSize = e.Graphics.MeasureString(before, font);
            SizeF matchSize = e.Graphics.MeasureString(matched, font);
            float totalWidth = e.Graphics.MeasureString(cellText, font).Width;

            float startX = bounds.X + (bounds.Width - totalWidth) / 2f;
            float textY = bounds.Y + (bounds.Height - font.GetHeight()) / 2f;

            RectangleF highlightRect = new RectangleF(
                startX + beforeSize.Width - 1,
                textY,
                matchSize.Width + 2,
                font.GetHeight()
            );
            e.Graphics.FillRectangle(Brushes.Yellow, highlightRect);

            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using (SolidBrush textBrush = new SolidBrush(e.Appearance.ForeColor))
            {
                e.Graphics.DrawString(cellText, font, textBrush, bounds, sf);
            }

            e.Handled = true;
        }

        private void txteditReceivedAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '.' && txteditReceivedAmount.Text.Contains('.'))
                e.Handled = true;
        }

        private void txteditReceivedAmount_Leave(object sender, EventArgs e)
        {
            string raw = txteditReceivedAmount.Text.Replace("₱", "").Replace(",", "").Trim();

            if (!string.IsNullOrWhiteSpace(raw) && decimal.TryParse(raw, out decimal value))
                txteditReceivedAmount.Text = string.Format("₱{0:N2}", value);
            else
                txteditReceivedAmount.Text = "";
        }

        public void LoadEditItem(AuditorReviewItemDto item)
        {


            // HIDE TOP SECTION
            gcItems.Visible = false;
            searchControl.Visible = false;

            lblReceivingReport.Text =
                $"Editing Item From: {item.Department}";

            lblPONumber.Text =
                $"PO Number: {item.PONumber}";

            txteditItemName.Text =
                item.ItemName;

            txteditExpectedQuan.Text =
                item.ExpectedQuantity.ToString();

            txteditExpectedAmount.Text =
                item.ExpectedAmount.ToString("N2");

            spneditReceivedQuan.EditValue =
                item.ReceivedQuantity;

            txteditReceivedAmount.Text =
                item.ReceivedAmount.ToString("N2");
            dateEdit.EditValue = item.DateReceived;

            txteditRemarks.Text =
                item.Remarks;

            chckboxAsset.Checked = item.IsCapitalizable;

            // LOCK NON-EDITABLE FIELDS
            txteditItemName.Enabled = false;
            txteditExpectedQuan.Enabled = false;
            txteditExpectedAmount.Enabled = false;

            // EDITABLE FIELDS
            spneditReceivedQuan.Enabled = true;
            txteditReceivedAmount.Enabled = true;
            txteditRemarks.Enabled = true;
            btnUpload.Enabled = true;

            // OPTIONAL
            btnConfirm.Text = "Update Item";

        }
    }
}