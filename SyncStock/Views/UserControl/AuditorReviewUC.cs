using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using SyncStock.Database;
using SyncStock.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace SyncStock.Views.UserControl
{
    public partial class AuditorReviewUC : DevExpress.XtraEditors.XtraUserControl
    {
        private const string AllDepartmentsLabel = "All Departments";
        private const string AllStatusesLabel = "All Statuses";
        private const string AllMonthsLabel = "All Months";
        private string CurrentUserRole { get; set; } = "Auditor"; // placeholder
        private string CurrentUserId { get; set; } = "user-001"; // placeholder
        private bool CanLockDirectly =>
    string.Equals(CurrentUserRole, "Admin", StringComparison.OrdinalIgnoreCase);

        // Tracks whether the currently selected month is locked.
        private bool _selectedMonthIsLocked = false;

        // Pill labels shown in the grid (not raw Yes/No or DB status strings).
        private const string PillCapitalizable = "Capitalizable";
        private const string PillNonCapitalizable = "Expenses";
        private const string PillActive = "Active";
        private const string PillPendingReview = "Pending Review";
        private const string PillApproved = "Approved";

        private readonly Repository _repo = new Repository();
        private List<AuditorReviewItemDto> _reviewItems = new List<AuditorReviewItemDto>();

        public AuditorReviewUC()
        {
            InitializeComponent();

            // Designer only runs InitializeComponent — avoid DB calls and runtime-only grid setup.
            if (IsDesignTime())
                return;

            InitializeReviewScreen();
        }

        private bool IsDesignTime() =>
            DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        private void InitializeReviewScreen()
        {
            ReviewItemGV.OptionsBehavior.Editable = false;
            ReviewItemGV.OptionsView.ShowAutoFilterRow = false;

            ConfigureSearchControl();
            ConfigureGridColumns();
            LoadFilterDepartments();
            LoadFilterStatuses();
            LoadReviewItems();      // must be BEFORE LoadFilterMonths
            LoadFilterMonths();     // uses _reviewItems which is now populated
            LoadReviewItems();
            LoadFilterMonths();

            CmbFilterDepartment.SelectedIndexChanged += FilterChanged;
            CmbFilterList.SelectedIndexChanged += FilterChanged;
            CmbDate.SelectedIndexChanged += FilterChanged;
            ScFilter.EditValueChanged += FilterChanged;
            CmbFilterDepartment.SelectedIndexChanged += FilterChanged;
            CmbFilterList.SelectedIndexChanged += FilterChanged;
            CmbDate.SelectedIndexChanged += FilterChanged;
            ScFilter.EditValueChanged += FilterChanged;

            RefreshLockButtonState(); // ← add this
        }

        private void ConfigureSearchControl()
        {
            // Search is applied in code (same list as statistics) so totals always match the grid.
            ScFilter.Properties.NullValuePrompt = "Search accepted receipts...";
        }

        private void ConfigureGridColumns()
        {
            colPONumber.FieldName = nameof(AuditorReviewItemDto.PONumber);
            colItemName.FieldName = nameof(AuditorReviewItemDto.ItemName);
            colInvoiceNumber.FieldName = nameof(AuditorReviewItemDto.InvoiceNumber);
            colUnitPrice.FieldName = nameof(AuditorReviewItemDto.UnitPrice);
            colQuantity.FieldName = nameof(AuditorReviewItemDto.Quantity);
            colTotalAmount.FieldName = nameof(AuditorReviewItemDto.TotalAmount);
            colDateReceived.FieldName = nameof(AuditorReviewItemDto.DateReceived);
            colCapitalizable.FieldName = nameof(AuditorReviewItemDto.Capitalizable);
            colDepartment.FieldName = nameof(AuditorReviewItemDto.Department);
            colStatus.FieldName = nameof(AuditorReviewItemDto.Status);

            colUnitPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colUnitPrice.DisplayFormat.FormatString = "N2";
            colTotalAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colTotalAmount.DisplayFormat.FormatString = "N2";
            colDateReceived.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colDateReceived.DisplayFormat.FormatString = "dd MMM yyyy";
        }

        private void LoadFilterDepartments()
        {
            var departments = _repo.GetAllDepartments().ToList();

            CmbFilterDepartment.Properties.Items.Clear();
            CmbFilterDepartment.Properties.Items.Add(AllDepartmentsLabel);

            foreach (var dept in departments)
                CmbFilterDepartment.Properties.Items.Add(dept.DepartmentName);

            CmbFilterDepartment.Properties.TextEditStyle =
                DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            CmbFilterDepartment.SelectedIndex = 0;
        }

        private void LoadFilterStatuses()
        {
            CmbFilterList.Properties.Items.Clear();
            CmbFilterList.Properties.Items.Add(AllStatusesLabel);
            CmbFilterList.Properties.Items.Add(WorkflowStatus.Received);
            CmbFilterList.Properties.Items.Add(WorkflowStatus.Active);

            CmbFilterList.Properties.TextEditStyle =
                DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            CmbFilterList.SelectedIndex = 0;
        }

        // ── Replace LoadFilterMonths ────────────────────────────────────────────────

        private void LoadFilterMonths()
        {
            CmbDate.Properties.Items.Clear();
            CmbDate.Properties.Items.Add(AllMonthsLabel);

            var months = _reviewItems
                .Where(x => x.DateReceived != default(DateTime))
                .Select(x => new DateTime(x.DateReceived.Year, x.DateReceived.Month, 1))
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            // Ensure current month is always present in the list
            var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            if (!months.Contains(currentMonth))
                months.Insert(0, currentMonth);

            foreach (var month in months.OrderByDescending(d => d))
                CmbDate.Properties.Items.Add(month.ToString("MMMM yyyy"));

            CmbDate.Properties.TextEditStyle =
                DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            // ── Default to current month ──────────────────────────────────────────
            string currentMonthLabel = currentMonth.ToString("MMMM yyyy");
            int idx = CmbDate.Properties.Items.IndexOf(currentMonthLabel);
            CmbDate.SelectedIndex = idx >= 0 ? idx : 0;
        }

        private void LoadReviewItems()
        {
            try
            {
                _reviewItems = _repo.GetAuditorReviewItems().ToList();
                MergeDistinctStatusesIntoFilter();

                ApplyFiltersAndRefresh();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Unable to load accepted receiving items.{Environment.NewLine}{ex.Message}",
                    "Auditor Review",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void MergeDistinctStatusesIntoFilter()
        {
            var selected = CmbFilterList.Text;
            var known = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                AllStatusesLabel,
                WorkflowStatus.Received,
                WorkflowStatus.Active,
                WorkflowStatus.Pending
            };

            foreach (var status in _reviewItems
                .Select(x => x.Status)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (known.Add(status))
                    CmbFilterList.Properties.Items.Add(status);
            }

            if (!string.IsNullOrEmpty(selected) && CmbFilterList.Properties.Items.Contains(selected))
                CmbFilterList.Text = selected;
            else if (CmbFilterList.SelectedIndex < 0)
                CmbFilterList.SelectedIndex = 0;
        }

        private void ApplyFiltersAndRefresh()
        {
            var filtered = GetFilteredReviewItems().ToList();

            ReviewItemGV.ActiveFilterString = string.Empty;
            ReviewItemGV.FindFilterText = string.Empty;
            ReviewItemGC.DataSource = null;
            ReviewItemGC.DataSource = filtered;

            UpdateStatistics(filtered, filtered.Sum(x => x.Quantity));
        }

        private List<AuditorReviewItemDto> GetFilteredReviewItems()
        {
            IEnumerable<AuditorReviewItemDto> items = _reviewItems;

            if (CmbFilterDepartment.SelectedIndex > 0 &&
                !string.Equals(CmbFilterDepartment.Text, AllDepartmentsLabel, StringComparison.OrdinalIgnoreCase))
            {
                string department = CmbFilterDepartment.Text;
                items = items.Where(x =>
                    string.Equals(x.Department, department, StringComparison.OrdinalIgnoreCase));
            }

            if (CmbFilterList.SelectedIndex > 0 &&
                !string.Equals(CmbFilterList.Text, AllStatusesLabel, StringComparison.OrdinalIgnoreCase))
            {
                string status = CmbFilterList.Text;
                items = items.Where(x =>
                    string.Equals(x.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            if (CmbDate.SelectedIndex > 0 &&
                !string.Equals(CmbDate.Text, AllMonthsLabel, StringComparison.OrdinalIgnoreCase) &&
                DateTime.TryParseExact(
                    CmbDate.Text,
                    "MMMM yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime selectedMonth))
            {
                var start = selectedMonth.Date;
                var end = selectedMonth.AddMonths(1).AddDays(-1).Date;
                items = items.Where(x =>
                {
                    var received = x.DateReceived.Date;
                    return received >= start && received <= end;
                });
            }

            string searchText = GetSearchText();
            if (!string.IsNullOrWhiteSpace(searchText))
                items = items.Where(x => MatchesSearch(x, searchText));

            return items.ToList();
        }

        private string GetSearchText()
        {
            string text = Convert.ToString(ScFilter.EditValue);
            if (string.IsNullOrWhiteSpace(text))
                text = ScFilter.Text;

            return text?.Trim() ?? string.Empty;
        }

        private static bool MatchesSearch(AuditorReviewItemDto item, string search)
        {
            return Contains(item.PONumber, search)
                || Contains(item.ItemName, search)
                || Contains(item.InvoiceNumber, search)
                || Contains(item.Department, search)
                || Contains(item.Status, search)
                || Contains(item.Capitalizable, search)
                || Contains(PillCapitalizable, search)
                || Contains(PillNonCapitalizable, search)
                || Contains(PillPendingReview, search)
                || item.Quantity.ToString().IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                || item.UnitPrice.ToString("N2").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                || item.TotalAmount.ToString("N2").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                || item.DateReceived.ToString("dd MMM yyyy").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool Contains(string value, string search)
        {
            return !string.IsNullOrEmpty(value)
                && value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsCapitalized(AuditorReviewItemDto item) =>
            string.Equals(item.Capitalizable, "Yes", StringComparison.OrdinalIgnoreCase);

        private static bool IsPendingReview(AuditorReviewItemDto item) =>
            string.Equals(item.Status, WorkflowStatus.Received, StringComparison.OrdinalIgnoreCase);

        private void UpdateStatistics(IReadOnlyList<AuditorReviewItemDto> items, int totalQuantity)
        {
            int capitalizedCount = items.Count(IsCapitalized);
            int pendingCount = items.Count(IsPendingReview);
            decimal capitalizedValue = items.Where(IsCapitalized).Sum(x => x.TotalAmount);
            decimal totalValue = items.Sum(x => x.TotalAmount);

            TotalAssetsNum.Text = "₱" + totalValue.ToString("N2");
            labelControl5.Text = GetPeriodLabelText(totalQuantity);

            CapitalizedNum.Text = capitalizedCount.ToString();
            labelControl8.Text = "₱" + capitalizedValue.ToString("N2");
            PendingNum.Text = pendingCount.ToString();
        }

        private string GetPeriodLabelText(int totalQuantity)
        {
            if (CmbDate.SelectedIndex > 0 &&
                !string.Equals(CmbDate.Text, AllMonthsLabel, StringComparison.OrdinalIgnoreCase))
            {
                return $"{CmbDate.Text} · {totalQuantity:N0} items";
            }

            return $"{totalQuantity:N0} items · Current Period";
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndRefresh();
            RefreshLockButtonState();   // update Lock/Unlock button after every filter change
        }

        private void ReviewItemGV_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == nameof(AuditorReviewItemDto.Status))
            {
                if (!TryGetStatusPill(e.CellValue?.ToString(), out string label, out Color bgColor, out Color textColor))
                    return;

                DrawBadge(e, label, bgColor, textColor);
                return;
            }

            if (e.Column.FieldName == nameof(AuditorReviewItemDto.Capitalizable))
            {
                if (!TryGetCapitalizablePill(e.CellValue?.ToString(), out string label, out Color bgColor, out Color textColor))
                    return;

                DrawBadge(e, label, bgColor, textColor);
            }
        }

        private static bool TryGetStatusPill(string status, out string label, out Color bgColor, out Color textColor)
        {
            label = null;
            bgColor = Color.Empty;
            textColor = Color.Empty;

            if (string.Equals(status, WorkflowStatus.Active, StringComparison.OrdinalIgnoreCase))
            {
                label = PillActive;
                bgColor = Color.FromArgb(220, 247, 220);
                textColor = Color.FromArgb(30, 120, 30);
                return true;
            }

            if (string.Equals(status, WorkflowStatus.Approved, StringComparison.OrdinalIgnoreCase))
            {
                label = PillApproved;
                bgColor = Color.FromArgb(220, 247, 220);
                textColor = Color.FromArgb(30, 120, 30);
                return true;
            }

            if (string.Equals(status, WorkflowStatus.Received, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, WorkflowStatus.Pending, StringComparison.OrdinalIgnoreCase))
            {
                label = PillPendingReview;
                bgColor = Color.FromArgb(255, 243, 200);
                textColor = Color.FromArgb(160, 100, 0);
                return true;
            }

            return false;
        }

        private static bool TryGetCapitalizablePill(string value, out string label, out Color bgColor, out Color textColor)
        {
            label = null;
            bgColor = Color.Empty;
            textColor = Color.Empty;

            if (string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase))
            {
                label = PillCapitalizable;
                bgColor = Color.FromArgb(220, 235, 255);
                textColor = Color.FromArgb(30, 80, 180);
                return true;
            }

            if (string.Equals(value, "No", StringComparison.OrdinalIgnoreCase))
            {
                label = PillNonCapitalizable;
                bgColor = Color.FromArgb(243, 244, 246);
                textColor = Color.FromArgb(75, 85, 99);
                return true;
            }

            return false;
        }

        private static void DrawBadge(RowCellCustomDrawEventArgs e, string label, Color bgColor, Color textColor)
        {
            e.Handled = true;
            e.Appearance.FillRectangle(e.Cache, e.Bounds);

            Graphics g = e.Graphics;
            int padX = 8, padY = 4;
            Rectangle cell = e.Bounds;
            Rectangle badge = new Rectangle(
                cell.X + padX,
                cell.Y + padY,
                cell.Width - (padX * 2),
                cell.Height - (padY * 2));

            using (GraphicsPath path = GetRoundedRect(badge, 10))
            using (SolidBrush brush = new SolidBrush(bgColor))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(brush, path);
            }

            using (SolidBrush textBrush = new SolidBrush(textColor))
            using (Font font = new Font("Segoe UI", 8f, FontStyle.Regular))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(label, font, textBrush, badge, sf);
            }
        }

        private static GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.X + rect.Width - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.X + rect.Width - d, rect.Y + rect.Height - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        // ── Replace BtnLock_Click ───────────────────────────────────────────────────

        private void BtnLock_Click(object sender, EventArgs e)
        {
            if (!TryParseSelectedMonth(out DateTime selectedMonth))
            {
                XtraMessageBox.Show("Please select a specific month to lock.",
                    "Lock Month", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!CanLockDirectly)
            {
                XtraMessageBox.Show(
                    "You do not have permission to lock a month.\n" +
                    "Use 'Request Unlock' to submit a request to an Admin.",
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = XtraMessageBox.Show(
                $"Lock {selectedMonth:MMMM yyyy}?\n\nNo changes can be made to this month once locked.",
                "Confirm Lock", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                _repo.SaveMonthLock(new MonthLock
                {
                    MonthYear = selectedMonth,
                    IsLocked = true,
                    LockedByUserId = CurrentUserId,
                    LockedAt = DateTime.Now
                });

                XtraMessageBox.Show(
                    $"{selectedMonth:MMMM yyyy} has been locked.",
                    "Month Locked", MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefreshLockButtonState();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Failed to lock month.\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Replace BtnReqUnlock_Click ──────────────────────────────────────────────

        private void BtnReqUnlock_Click(object sender, EventArgs e)
        {
            if (!TryParseSelectedMonth(out DateTime selectedMonth))
            {
                XtraMessageBox.Show("Please select a specific month.",
                    "Unlock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── Admin: unlock directly ────────────────────────────────────────────
            if (CanLockDirectly)
            {
                var confirm = XtraMessageBox.Show(
                    $"Unlock {selectedMonth:MMMM yyyy}?\n\nThis will allow changes to this month again.",
                    "Confirm Unlock", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                try
                {
                    _repo.SaveMonthLock(new MonthLock
                    {
                        MonthYear = selectedMonth,
                        IsLocked = false,
                        LockedByUserId = CurrentUserId,
                        LockedAt = DateTime.Now
                    });

                    XtraMessageBox.Show(
                        $"{selectedMonth:MMMM yyyy} has been unlocked.",
                        "Month Unlocked", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    RefreshLockButtonState();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        $"Failed to unlock month.\n{ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return;
            }

            // ── Non-admin: submit an unlock request ───────────────────────────────
            var confirmReq = XtraMessageBox.Show(
                $"Submit an unlock request for {selectedMonth:MMMM yyyy}?\n\nAn Admin will review your request.",
                "Request Unlock", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmReq != DialogResult.Yes)
                return;

            try
            {
                _repo.SaveUnlockRequest(new UnlockRequest
                {
                    MonthYear = selectedMonth,
                    RequestedByUserId = CurrentUserId,
                    RequestedAt = DateTime.Now,
                    Status = "Pending"
                });

                XtraMessageBox.Show(
                    $"Your unlock request for {selectedMonth:MMMM yyyy} has been submitted.\n" +
                    "You will be notified once an Admin reviews it.",
                    "Request Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Failed to submit unlock request.\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ── Add these new methods ───────────────────────────────────────────────────

        /// <summary>
        /// Reads the DB to see if the selected month is locked, then updates
        /// BtnLock / BtnReqUnlock text and visibility accordingly.
        /// </summary>
        private void RefreshLockButtonState()
        {
            bool monthSelected = CmbDate.SelectedIndex > 0 &&
                !string.Equals(CmbDate.Text, AllMonthsLabel, StringComparison.OrdinalIgnoreCase);

            // Hide both buttons when "All Months" is selected — locking needs a specific month.
            BtnLock.Visible = monthSelected;
            BtnReqUnlock.Visible = monthSelected;

            if (!monthSelected)
                return;

            if (!TryParseSelectedMonth(out DateTime selectedMonth))
                return;

            var lockRecord = _repo.GetMonthLock(selectedMonth);
            _selectedMonthIsLocked = lockRecord != null && lockRecord.IsLocked;

            if (_selectedMonthIsLocked)
            {
                // Month is locked → show Unlock (or Request Unlock)
                BtnLock.Visible = false;
                BtnReqUnlock.Visible = true;
                BtnReqUnlock.Text = CanLockDirectly ? "Unlock Month" : "Request Unlock";
            }
            else
            {
                // Month is unlocked → show Lock button only to Admin, or also to others
                // (everyone sees Lock — non-admins will be blocked with a message on click)
                BtnLock.Visible = true;
                BtnLock.Text = "Lock Month";
                BtnReqUnlock.Visible = false;
            }
        }

        private bool TryParseSelectedMonth(out DateTime result)
        {
            return DateTime.TryParseExact(
                CmbDate.Text,
                "MMMM yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out result);
        }
    }
}