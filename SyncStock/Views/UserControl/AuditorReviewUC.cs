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

        // Pill labels shown in the grid (not raw Yes/No or DB status strings).
        private const string PillCapitalizable = "Capitalizable";
        private const string PillNonCapitalizable = "Non-Capitalizable";
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

            CmbFilterDepartment.SelectedIndexChanged += FilterChanged;
            CmbFilterList.SelectedIndexChanged += FilterChanged;
            CmbDate.SelectedIndexChanged += FilterChanged;
            ScFilter.EditValueChanged += FilterChanged;
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

        private void LoadFilterMonths()
        {
            CmbDate.Properties.Items.Clear();
            CmbDate.Properties.Items.Add(AllMonthsLabel);

            var months = _reviewItems
                .Where(x => x.DateReceived != default(DateTime))
                .Select(x => new DateTime(x.DateReceived.Year, x.DateReceived.Month, 1))
                .Distinct()
                .OrderByDescending(d => d);

            foreach (var month in months)
                CmbDate.Properties.Items.Add(month.ToString("MMMM yyyy"));

            CmbDate.Properties.TextEditStyle =
                DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            CmbDate.SelectedIndex = 0;
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
    }
}