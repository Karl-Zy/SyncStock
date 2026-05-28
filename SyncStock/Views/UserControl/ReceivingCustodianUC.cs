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
        private bool _isEditMode = false;
        private int _editingConfirmedItemId = 0;

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

                if (!_isEditMode)
                {
                    LoadDataFromRepository();
                }
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

            gvItemsView.Appearance.FocusedRow.BackColor = Color.FromArgb(83, 237, 126);
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

            // Hide raw POType and OrderMode — show friendly computed columns instead
            var poTypeCol = gvItemsView.Columns["POType"];
            if (poTypeCol != null)
                poTypeCol.Visible = false;

            var orderModeCol = gvItemsView.Columns["OrderMode"];
            if (orderModeCol != null)
                orderModeCol.Visible = false;

            // "Order Mode" column — Single Order / Grouped Order
            var orderModeDisplayCol = gvItemsView.Columns["OrderModeDisplay"];
            if (orderModeDisplayCol != null)
            {
                orderModeDisplayCol.Caption = "Order Mode";
                orderModeDisplayCol.Width = 120;
            }

            // "Order Type" column — Local / Online
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

            if (string.IsNullOrWhiteSpace(searchText))
            {
                gcItems.DataSource = allItems.ToList();
                ApplyColumnAlignment();
                return;
            }

            string[] keywords = searchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            gcItems.DataSource = allItems.Where(x =>
            {
                string po = x.PONumber?.ToLower() ?? string.Empty;
                string item = x.ItemName?.ToLower() ?? string.Empty;
                string orderType = x.OrderType?.ToLower() ?? string.Empty;
                string orderMode = x.OrderModeDisplay?.ToLower() ?? string.Empty;

                return keywords.All(k =>
                    po.Contains(k) ||
                    item.Contains(k) ||
                    orderType.Contains(k) ||
                    orderMode.Contains(k)
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

                    Status = chckboxAsset.Checked
    ? WorkflowStatus.Active
    : WorkflowStatus.Received,

                    AttachmentData = _uploadedFileBytes,
                    AttachmentFileName = _uploadedFileName
                };

                if (_isEditMode)
                {
                    _repo.UpdateConfirmedItem(
                        _editingConfirmedItemId,
                        confirmationPayload);
                    XtraMessageBox.Show(
    "Item updated successfully!",
    "Success",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information);

                    this.FindForm()?.Close();
                    return;
                }
                else
                {
                    _repo.AddConfirmedItem(
                        confirmationPayload);
                }

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
            if (_isEditMode)
            {
                this.FindForm()?.Close();
                return;
            }

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

        // ApplyGridStyling
        // Mo-configure ni sa visual appearance ug behavior sa atong
        // DevExpress grid view (gvItemsView) kausa ra jud inig load-time.
        // Responsibilities:
        //   - Mo-disable sa column reordering/resizing para dili ma-guba sa user.
        //   - Mo-apply og alternating row colors para nindot basahon (readability).
        //   - Mo-highlight sa focused (selected) row gamit ang green color.
        //   - Mo-set og comfortable ug relax nga row height.
        //   - Mo-hook sa custom-draw event para mo-andar ang search highlighting.
        private void ApplyGridStyling()
        {
            // I-lock ang layout para dili ma-accidentally rearrange o tarugon sa mga users ang mga columns.
            gvItemsView.OptionsCustomization.AllowColumnMoving = false;
            gvItemsView.OptionsCustomization.AllowColumnResizing = false;

            // Alternating row colours (Zebra striping ang style).
            gvItemsView.OptionsView.EnableAppearanceOddRow = true;
            gvItemsView.OptionsView.EnableAppearanceEvenRow = true;
            gvItemsView.Appearance.OddRow.BackColor = Color.FromArgb(245, 250, 248); // Light mint nga color.
            gvItemsView.Appearance.EvenRow.BackColor = Color.White;

            // Selected row: Bright green para hayag kaayo ug nindot ang contrast sa text.
            gvItemsView.Appearance.FocusedRow.BackColor = Color.FromArgb(83, 237, 126);
            gvItemsView.Appearance.FocusedRow.ForeColor = Color.FromArgb(30, 30, 30);
            // Kung mawad-an og focus ang grid, mogamit og mas soft nga green para visible gihapon ang selection.
            gvItemsView.Appearance.HideSelectionRow.BackColor = Color.FromArgb(198, 239, 206);

            gvItemsView.RowHeight = 32; // Gi-medyo taas kaysa default para mas relax basahon o dali i-touch.

            gvItemsView.OptionsView.ShowGroupPanel = false; // I-tago ang unused grouping panel sa babaw.
            gvItemsView.OptionsView.ColumnAutoWidth = true;  // I-stretch ang mga columns para ma-fill ang tibuok width.
            gvItemsView.OptionsSelection.EnableAppearanceFocusedCell = false; // Dili na kailangan i-highlight ang kada cell.

            // I-konektar/i-wire up ang custom-draw handler nga mag-pintal sa yellow keyword highlights.
            gvItemsView.CustomDrawCell += gvItemsView_CustomDrawCell;
        }

        // ApplyColumnAlignment
        // Nag-define ni kung unsa nga mga columns ang i-display, unsa ilang pagkahan-ay,
        // ug giunsa sila pag-label ug pag-size. Gi-set sad ani ang header font,
        // alignment, ug gi-configure ang numeric formatting.
        //
        // Kailangan jud ni i-call matag human og assign sa DataSource kay 
        // i-reset man gud sa DevExpress ang column metadata basta mag-change ang source.
        private void ApplyColumnAlignment()
        {
            // Magsugod ta pinaagi sa pag-tago sa tanang auto-generated columns para naa kay
            // full control kung unsa ra ang makit-an ug kung unsa ilang pagkahan-ay.
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvItemsView.Columns)
                col.Visible = false;

            // I-define ang mga columns: (FieldName, Header Caption, Width sa pixels)
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

            // Pass 1: Himoon nga visible ang kada column dayon i-apply ang caption ug width.
            // Dili pa i-set ang 'VisibleIndex' diri kay kailangan man gud sa DevExpress nga visible
            // una ang tanang target columns sa dili pa sila i-re-index.
            foreach (var (field, caption, width) in columnOrder)
            {
                var col = gvItemsView.Columns[field];
                if (col == null) continue;
                col.Caption = caption;
                col.Width = width;
                col.Visible = true;
            }

            // Pass 2: I-assign na ang 'VisibleIndex' pabalik kay visible naman ang tanang columns,
            // para ma-enforce jud ang saktong display order nga gi-define nato sa babaw.
            for (int i = 0; i < columnOrder.Length; i++)
            {
                var col = gvItemsView.Columns[columnOrder[i].Item1];
                if (col != null)
                    col.VisibleIndex = i;
            }

            // Pass 3: I-style na ang kada visible column header ug ang cell alignment.
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvItemsView.Columns)
            {
                if (!col.Visible) continue;

                // Bold ug centered dapat ang mga column headers para sa tanang columns.
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Font = new Font("Segoe UI", 9f, FontStyle.Bold);

                // Right-align para sa mga numeric columns; dayon center-align para sa uban.
                if (col.FieldName == "Amount" || col.FieldName == "Quantity")
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                else
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            // I-apply ang Philippine Peso currency formatting ngadto sa Amount column.
            var amountCol = gvItemsView.Columns["Amount"];
            if (amountCol != null)
            {
                amountCol.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                amountCol.DisplayFormat.FormatString = "₱{0:N2}"; // e.g. ₱1,234.56
            }
        }

        // SetColumn  (Helper utility – dili pa kaayo gi-call externally karon)
        // Convenience wrapper para mapahigayon nga visible ang usa ka column
        // ug mo-set sa iyang caption, width, ug position sa usa ra ka tawag.
        // Mapuslanon kaayo ni kung mag-configure og tagsa-tagsa nga columns gawas 
        // sa bulk ApplyColumnAlignment flow.
        private void SetColumn(string fieldName, string caption, int width, int visibleIndex)
        {
            var col = gvItemsView.Columns[fieldName];
            if (col == null) return;
            col.Caption = caption;
            col.Width = width;
            col.Visible = true;
            col.VisibleIndex = visibleIndex;
        }

        // gvItemsView_CustomDrawCell  (Grid custom paint event)
        // Mo-fire ni sa kada visible cell matag repaint sa atong grid.
        // Kung naay active nga search term, man-mano ni nga mo-pinta sa cell
        // content nga naay yellow highlight sa luyo sa na-match nga text.

        // Algorithm:
        //   1. Mo-exit dayon (early exit) kung walay search text nga gi-set.
        //   2. Pangitaon ang index sa na-match nga pulong sulod sa display text sa cell.
        //   3. Sukdon ang width sa mga text segments sa dili pa ug diha sa mismong match.
        //   4. I-calculate ang horizontal center position para sa tibuok text.
        //   5. Mag-pinta og yellow rectangle sa luyo sa katong na-match nga part lang jud.
        //   6. I-drawing ang tibuok cell text sa ibabaw (aron makit-an ang yellow sa luyo).
        //   7. I-set ang e.Handled = true para i-skip sa DevExpress ang iyang default paint.
        private void gvItemsView_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // Walay i-highlight kung empty o walay sulod ang search box.
            if (string.IsNullOrWhiteSpace(_currentSearchText)) return;

            string cellText = e.DisplayText;
            string search = _currentSearchText.ToLower();
            int matchIdx = cellText?.ToLower().IndexOf(search) ?? -1;

            // Kung walay na-match ani nga cell – pasagdan lang ang DevExpress nga mo-pinta ani og normal.
            if (matchIdx < 0) return;

            // I-clear una ang cell background para malikayan ang mga ghost rendering artifacts.
            e.Graphics.FillRectangle(
                new SolidBrush(e.Appearance.BackColor), e.Bounds);

            Rectangle bounds = e.Bounds;

            // I-slice o putlon ang cell text into: before-match | match | (implied na ang after).
            string before = cellText.Substring(0, matchIdx);
            string matched = cellText.Substring(matchIdx, search.Length);

            Font font = e.Appearance.GetFont();
            SizeF beforeSize = e.Graphics.MeasureString(before, font);
            SizeF matchSize = e.Graphics.MeasureString(matched, font);
            float totalWidth = e.Graphics.MeasureString(cellText, font).Width;

            // I-center ang tibuok text horizontally sulod sa bounds sa cell.
            float startX = bounds.X + (bounds.Width - totalWidth) / 2f;
            float textY = bounds.Y + (bounds.Height - font.GetHeight()) / 2f;

            // I-build ang yellow highlight rectangle nga naka-position gyud sa na-match nga text.
            RectangleF highlightRect = new RectangleF(
                startX + beforeSize.Width - 1,  // Ang -1 kay para sa gamay nga left padding.
                textY,
                matchSize.Width + 2,             // Ang +2 kay para sa gamay nga right padding.
                font.GetHeight()
            );
            e.Graphics.FillRectangle(Brushes.Yellow, highlightRect);

            // I-drawing na ang tibuok cell text (naka-center) sa ibabaw sa yellow highlight.
            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using (SolidBrush textBrush = new SolidBrush(e.Appearance.ForeColor))
            {
                e.Graphics.DrawString(cellText, font, textBrush, bounds, sf);
            }

            // Ingna ang DevExpress nga dili na i-redraw kani nga cell – kay kita nay nag-manomano og handle ani.
            e.Handled = true;
        }

        // Para limpyo ang Receuved Amount field, i-strip ni ang currency symbol ug commas inig focus (enter) sa text box
        private void txteditReceivedAmount_Enter(object sender, EventArgs e)
        {
            // Strip the ₱ symbol when user clicks in to edit
            txteditReceivedAmount.Text = txteditReceivedAmount.Text.Replace("₱", "").Replace(",", "").Trim();
        }

        private void txteditReceivedAmount_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(txteditReceivedAmount.Text, out decimal value))
                txteditReceivedAmount.Text = string.Format("₱{0:N2}", value);
        }

       

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

                    Status = chckboxAsset.Checked
    ? WorkflowStatus.Active
    : WorkflowStatus.Received,

                    AttachmentData = _uploadedFileBytes,
                    AttachmentFileName = _uploadedFileName
                };

                if (_isEditMode)
                {
                    _repo.UpdateConfirmedItem(
                        _editingConfirmedItemId,
                        confirmationPayload);
                    XtraMessageBox.Show(
    "Item updated successfully!",
    "Success",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information);

                    this.FindForm()?.Close();
                    return;
                }
                else
                {
                    _repo.AddConfirmedItem(
                        confirmationPayload);
                }

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
            if (_isEditMode)
            {
                this.FindForm()?.Close();
                return;
            }

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

        public void LoadEditItem(AuditorReviewItemDto item)
        {

            _isEditMode = true;
            _editingConfirmedItemId =
    item.ConfirmedItemID;

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