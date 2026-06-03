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
    // INHERITANCE 
    // XtraUserControl kay ang DevExpress base class sa tanan custom user controls
    // Sa pag inherit ana kay ang themes ug skins
    public partial class ReceivingCustodianUC : DevExpress.XtraEditors.XtraUserControl
    {
        // ENCAPSULATION 
        // Dli ni makita sa mga external classes, ug dili nila ma access directly.
        // Ang access sa database kay controlled ra sa mga methods sa ubos, dili diretso.


        // ABSTRACTION
        // Kani ang tig-kuhag data sa database (Repository).
        // dli na kailangan refer sa mga SQL queries diri, kay ang Repository na ang bahala ana.
        private readonly Repository _repo = new Repository();

        // Dinhi gi-save ang sulod sa gi-upload nga resibo. 
        // "null" (walay sulod) ni kung wala pay gi-pili.
        private byte[] _uploadedFileBytes = null;

        // Ang mismong ngalan sa file nga gi-upload (e.g. "receipt.pdf").
        private string _uploadedFileName = null;
        private bool _isEditMode = false;
        private int _editingConfirmedItemId = 0;

        // Gi-remind ani kung unsa ang gi-type sa search box 
        // para ma-highlight ang text inig display sa screen.
        private string _currentSearchText = string.Empty;

        // Listahan sa mga file format nga pwede i-upload (.jpg, .png, .pdf).
        // Gi-check ni sa button ug sa drag-and-drop.
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
        private bool _userClickedRow = false;
        private bool _isEditMode = false;
        private int _editingConfirmedItemId = 0;
        public ReceivingCustodianUC()
        {
            InitializeComponent();
        }


        // POLYMORPHISM
        // Ang 'OnLoad' kay gikan sa original nga WinForms (Control).
        // Gi-override nato ni para mo-andar ang atoang settings inig sugod sa screen,
        // dayun gina-tawag gihapon ang base.OnLoad para dili maguba ang system.

        // Ang 'DesignMode' guard kay para dili mo-error o mo-andar ang 
        // database inig open sa Visual Studio Designer panel.
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
                gvItemsView.RowClick += (s, me) =>
                {
                    _userClickedRow = true;
                    if (gvItemsView.FocusedRowHandle == me.RowHandle)
                        gvItemsView_FocusedRowChanged(null,
                            new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs(me.RowHandle, me.RowHandle));
                };
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

        // LoadDataFromRepository
        // Gi-kuha ani ang tanang 'pending incoming PO items' gikan sa database
        // ug gi-bira padung sa grid control (gcItems).
        // 
        // Gi-format sab og balik ang mga columns pagkahuman og butang sa data
        // kay limpyohan ug i-reset man gud sa DevExpress ang column settings 
        // inig change sa DataSource.
        private void LoadDataFromRepository()
        {
            try
            {
                // ABSTRACTION
                // igo ra ta magkuha sa data
                var incomingItems = _repo.GetPendingIncomingItemsDetails();
                gcItems.DataSource = incomingItems;
                ApplyColumnAlignment(); // re apply ang headers ug widths
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

        // gvItemsView_FocusedRowChanged (Grid row-selection event)
        // mag run ni kada click sa user og laing row sa grid view.
        // Basahon ani ang cell values sa gi-pili nga row, dayon i-populate 
        // sa ubos nga section "Receiving Form" fields para ma-review sa custodian 
        // ang expected values before nila i-input ang actual data.
        private void gvItemsView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            int rowHandle = e.FocusedRowHandle;

            // Gi check sa 'IsValidRowHandle' para dili ma-apil ang mga header rows 
            // o katong mga empty-grid states—kay i-represent man gud na sa DevExpress 
            // gamit ang mga negative sentinel values, so kailangan jud i-filter.
            if (gvItemsView.IsValidRowHandle(rowHandle))
            {
                try
                {
                    // Mag-bira sa mga field values gikan sa gi-pili nga grid row.
                    string poNumber = gvItemsView.GetRowCellValue(rowHandle, "PONumber")?.ToString();
                    string department = gvItemsView.GetRowCellValue(rowHandle, "Department")?.ToString();
                    string itemName = gvItemsView.GetRowCellValue(rowHandle, "ItemName")?.ToString();
                    object qty = gvItemsView.GetRowCellValue(rowHandle, "Quantity");
                    object amount = gvItemsView.GetRowCellValue(rowHandle, "Amount");

                    // I-update ang mga informational labels sa pinakababaw sa form.
                    lblReceivingReport.Text = $"Receiving Report From: {department}";
                    lblPONumber.Text = $"PO Number: {poNumber}";

                    // I-populate ang read-only "Expected" fields para ma-compare sa custodian
                    // kung unsa ang gi-order versus sa unsa jud ang ni-abot.
                    txteditItemName.Text = itemName;
                    txteditExpectedQuan.Text = qty?.ToString();

                    // I-format ang expected amount into a 2-decimal number.
                    // Kung mo-fail ang pag-parse, i-default lang og "0.00" para dili blank tan-awon.
                    if (amount != null && decimal.TryParse(amount.ToString(), out decimal parsedAmount))
                        txteditExpectedAmount.Text = string.Format("{0:N2}", parsedAmount);
                    else
                        txteditExpectedAmount.Text = "0.00";

                    // Pre-fillan og mga sensible defaults ang mga editable "Received" fields.
                    dateEdit.EditValue = DateTime.Today;            // I-default lang sa date karon.
                    spneditReceivedQuan.EditValue = qty;            // I-default sa expected quantity.
                    txteditReceivedAmount.Text = "";                // Kailangan jud ni i-type sa custodian.
                    txteditRemarks.Text = "";
                    chckboxAsset.Checked = false;
                }
                catch (Exception ex)
                {
                    // Non-critical: I-log lang sa debug output imbis nga mang-disturbo ta sa user gamit ang pop-up.
                    System.Diagnostics.Debug.WriteLine($"Error assigning grid values to form: {ex.Message}");
                }
            }
        }

        // searchControl_TextChanged  (Live-search event)
        // Mo-fire ni kada keystroke o liso sa text sa search box.
        // Mo-filter ni sa mga grid rows client-side (no extra DB calls jud 
        // kada pindot sa key) pinaagi sa pag-match sa tanang space-separated keywords 
        // batok sa PO number, item name, order type, order mode, ug department.
        // Mo-trigger sad ni og grid repaint para ma-update ang mga highlighted matches.
        private void searchControl_TextChanged(object sender, EventArgs e)
        {
            _currentSearchText = searchControl.Text?.Trim() ?? string.Empty;
            string searchLower = _currentSearchText.ToLower();

            // Mag-bira permi og fresh copy gikan sa repo para non-destructive ang pag-filter
            // (meaning, walay data nga permanently ma-remove o mawala).
            var allItems = _repo.GetPendingIncomingItemsDetails();

            // Kung empty ang search box, i-restore lang ang full list dayon mo-exit early.
            if (string.IsNullOrWhiteSpace(searchLower))
            {
                gcItems.DataSource = allItems.ToList();
                ApplyColumnAlignment();
                gvItemsView.RefreshData();
                return;
            }

            // I-split into individual words para ang "Laptop Office" mo-match gihapon sa mga rows
            // nga naay sulod sa duha ka mga pulong maski asa dapit (AND logic via keywords.All).
            string[] keywords = searchLower.Split(
                new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // I-apply na ang multi-keyword filter across tanang searchable columns.
            gcItems.DataSource = allItems.Where(x =>
            {
                string po = x.PONumber?.ToLower() ?? string.Empty;
                string item = x.ItemName?.ToLower() ?? string.Empty;
                string orderType = x.OrderType?.ToLower() ?? string.Empty;
                string orderMode = x.OrderModeDisplay?.ToLower() ?? string.Empty;
                string dept = x.Department?.ToLower() ?? string.Empty;

                // Makalubot lang ang row kung ang kada keyword makit-an sa maski usa lang ka column.
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

        // ===========================================================
        // BUTTON HANDLERS
        // ===========================================================

        // btnUpload_Click  (Upload button – file dialog)
        // Mo-open ni og file-open dialog nga restricted lang jud para sa image ug PDF types.
        // Kung mag-select ang user og valid nga file, basahon ni padung sa memory as
        // a byte array para ma-apil ra puhon inig save sa DB record.
        private void btnUpload_Click(object sender, EventArgs e)
        {
            // I-configure ang dialog gamit ang mga friendly filter labels.
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

                    // ENCAPSULATION: Gi-keep sulod sa kani nga method ang validation 
                    // para ang tibuok class makasiguro nga permi ra clean data ang madawat.
                    if (!_allowedExtensions.Contains(fileExtension))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(
                            "Invalid file type. Only JPG, JPEG, PNG, and PDF files are allowed.",
                            "Invalid File",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    // Basahon ang tibuok file padung sa memory, dayon i-save sa mga private fields.
                    _uploadedFileBytes = System.IO.File.ReadAllBytes(selectedFilePath);
                    _uploadedFileName = System.IO.Path.GetFileName(selectedFilePath);

                    // I-inform ang user kung unsa nga file ang naka-queue na para i-upload.
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

        // btnUpload_DragEnter  (Drag-and-drop: enter zone)
        // I-change ani ang cursor feedback ngadto sa "Copy" basta mag-drag 
        // og file ang user ibabaw sa upload button, aron mo-signal nga valid 
        // ug pwede ra jud kaayo i-drop dinhi.
        private void btnUpload_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;  // Mo-display sa "+" cursor icon.
            else
                e.Effect = DragDropEffects.None;  // Mo-display sa "blocked" cursor icon.
        }

        // btnUpload_DragDrop  (Drag-and-drop: file released)
        // Mo-handle ni basta i-buhat na og drop ang file didto sa ibabaw sa upload button.
        // Mo-execute gihapon ni og same extension validation parehas sa file dialog handler
        // sa dili pa basahon ang file padung sa shared byte-array fields.
        // Ang pinakaunang gi-drop nga file ra ang gamiton kung daghan ang gi-sabay og drop.
        private void btnUpload_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files != null && files.Length > 0)
                {
                    string droppedFilePath = files[0]; // Ang pinakaunang file ra jud ang i-process nato.
                    string fileExtension = System.IO.Path.GetExtension(droppedFilePath).ToLower();

                    // Same validation gihapon sa btnUpload_Click para permi consistent.
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

        // btnConfirm_Click  (Confirm / Submit button)
        // Kani ang core submission handler. Mo-run ni og multi-step validation 
        // chain, dayon mo-build og ConfirmedItems payload para i-save diretso 
        // sa database pinaagi sa atong repository.
        //
        // Validation order:
        //   1. Kailangan naay gi-pili nga grid row (dapat naay PO number).
        //   2. Kailangan naay sulod ang Date Received.
        //   3. Ang Received Quantity kailangan jud > 0.
        //   4. Ang Received Amount kailangan valid ug non-negative decimal.
        //
        // On success:
        //   - Mo-insert og bag-ong confirmed-item record.
        //   - I-update ang status sa PO line-item ngadto sa "Received".
        //   - I-clear ang form dayon i-refresh ang grid para updated.
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            // --- Validation Step 1: Siguraduhon nga naay gi-pili nga row sa grid ---
            if (string.IsNullOrEmpty(lblPONumber.Text) || lblPONumber.Text == "PO Number:")
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Validation Error: Please select a pending item from the table before confirming.",
                    "Missing Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Validation Step 2: Date Received ---
            if (dateEdit.EditValue == null || string.IsNullOrWhiteSpace(dateEdit.Text))
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Validation Error: 'Date Received' is required.",
                    "Missing Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateEdit.Focus();
                return;
            }

            // --- Check if the selected date falls in a locked month ---
            DateTime receivedDate = Convert.ToDateTime(dateEdit.EditValue);
            var monthLock = _repo.GetMonthLock(receivedDate);
            if (monthLock != null && monthLock.IsLocked)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    $"Validation Error: The month of {receivedDate:MMMM yyyy} is locked. No transactions or changes are allowed.",
                    "Month Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateEdit.Focus();
                return;
            }

            // --- Validation Step 3: Dapat mas dako sa 0 ang Received Quantity ---
            if (spneditReceivedQuan.EditValue == null ||
                Convert.ToInt32(spneditReceivedQuan.EditValue) <= 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Validation Error: 'Received Quantity' must be greater than 0.",
                    "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                spneditReceivedQuan.Focus();
                return;
            }

            // --- Validation Step 4: Ang Received Amount kailangan valid ug dili negative decimal ---
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
                // I-strip o tangtangon ang "PO Number:" prefix nga gi-add lang para sa display.
                string purePoNumber = lblPONumber.Text.Replace("PO Number:", "").Trim();

                // I-build ang data transfer object (DTO) nga i-save sa repository padung database.
                // ABSTRACTION: Plain model class ra ang ConfirmedItems;
                // ang repo ray nakahibalo sa SQL, wala kay labot ani nga handler.
                var confirmationPayload = new ConfirmedItems
                {
                    PONumber = purePoNumber,
                    ItemName = txteditItemName.Text,

                    DateReceived = Convert.ToDateTime(dateEdit.EditValue),
                    IsCapitalizable = chckboxAsset.Checked,
                    Status = chckboxAsset.Checked ? WorkflowStatus.Active : WorkflowStatus.Received,
                    ExpectedQuantity = Convert.ToInt32(txteditExpectedQuan.Text),
                    ReceivedQuantity = Convert.ToInt32(spneditReceivedQuan.EditValue),

                    ExpectedAmount = Convert.ToDecimal(txteditExpectedAmount.Text),
                    ReceivedAmount = parsedReceivedAmount,

                    Remarks = txteditRemarks.Text,
                    AttachmentData = _uploadedFileBytes,
                    AttachmentFileName = _uploadedFileName
                };

                if (_isEditMode)
                {
                    _repo.UpdateConfirmedItem(
                        _editingConfirmedItemId,
                        confirmationPayload);
                }
                else
                {
                    _repo.AddConfirmedItem(
                        confirmationPayload);

                    _repo.UpdatePurchaseOrderItemStatus(
                        purePoNumber,
                        txteditItemName.Text,
                        WorkflowStatus.Received);
                }

                // ✅ Fixed — passes itemName so only THIS item's PO status updates
                _repo.UpdatePurchaseOrderItemStatus(
                    purePoNumber,
                    txteditItemName.Text,
                    WorkflowStatus.Received);  // Enum value nga nag-represent sa "Received" stage.

                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Asset inventory ledger updated and item confirmed successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (_isEditMode)
                {
                    this.FindForm()?.Close();
                }
                else
                {
                    // I-reset ang form dayon i-reload ang grid para makuha na ang 
                    // bag-ong gi-confirm nga item gikan sa "pending" list.
                    ClearReceivingForm();
                    LoadDataFromRepository();
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    $"Inventory database submission failed: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // btnCancel_Click  (Cancel button)
        // Mo-discard ni sa mga unsaved inputs pinaagi sa pag-reset 
        // sa tanang form fields ngadto sa ilang default o empty states.
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_isEditMode)
            {
                this.FindForm()?.Close();
                return;
            }

            ClearReceivingForm();
        }

        // btnCancel_Click  (Cancel button)
        // Mo-discard ni sa mga unsaved inputs pinaagi sa pag-reset 
        // sa tanang form fields ngadto sa ilang default o empty states.
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearReceivingForm();
        }

        // ===========================================================
        // VISUAL / HELPER METHODS
        // ===========================================================

        // ClearReceivingForm
        // Mo-reset ni sa kada editable control sa right-hand panel 
        // balik sa ilang default state. Gi-call ni right after sa successful 
        // submission ug basta mo-cancel ang user.
        // Mo-clear sad ni sa in-memory file attachment fields para dili 
        // ma-accidentally carry over ang previous upload.
        private void ClearReceivingForm()
        {
            // I-reset ang mga informational labels.
            lblReceivingReport.Text = "Receiving Report From:";
            lblPONumber.Text = "PO Number:";

            // I-clear ang mga expected-value display fields.
            txteditItemName.Text = "";
            txteditExpectedQuan.Text = "";
            txteditExpectedAmount.Text = "0.00";

            // I-reset ang tanang "Received" input controls ngadto sa ilang default values.
            dateEdit.EditValue = null;
            chckboxAsset.Checked = false;
            spneditReceivedQuan.EditValue = 0;
            txteditReceivedAmount.Text = "";
            txteditRemarks.Text = "";

            // I-discard o tangtangon ang maski unsa nga previously selected attachment.
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