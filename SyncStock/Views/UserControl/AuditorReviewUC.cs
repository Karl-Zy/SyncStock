using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using SyncStock.Database;
using SyncStock.Models;
using System;
using System.Collections.Generic;
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

        private readonly Repository _repo = new Repository();
        private List<AuditorReviewItemDto> _reviewItems = new List<AuditorReviewItemDto>();

        public AuditorReviewUC()
        {
            InitializeComponent();
            InitializeReviewScreen();
        }

        private void InitializeReviewScreen()
        {
            ReviewItemGV.OptionsBehavior.Editable = false;
            ReviewItemGV.OptionsView.ShowAutoFilterRow = false;

            ConfigureSearchControl();
            ConfigureGridColumns();
            LoadFilterDepartments();
            LoadFilterStatuses();
            LoadReviewItems();

            CmbFilterDepartment.SelectedIndexChanged += FilterCombo_SelectedIndexChanged;
            CmbFilterList.SelectedIndexChanged += FilterCombo_SelectedIndexChanged;
            ReviewItemGV.ColumnFilterChanged += ReviewItemGV_ColumnFilterChanged;
        }

        private void ConfigureSearchControl()
        {
            ScFilter.Client = ReviewItemGC;
            ScFilter.Properties.NullValuePrompt = "Search accepted receipts...";
        }

        private void ConfigureGridColumns()
        {
            var currencyFormat = "N2";
            var dateFormat = "dd MMM yyyy";

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
            colUnitPrice.DisplayFormat.FormatString = currencyFormat;
            colTotalAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colTotalAmount.DisplayFormat.FormatString = currencyFormat;
            colDateReceived.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colDateReceived.DisplayFormat.FormatString = dateFormat;
        }

        private void LoadFilterDepartments()
        {
            var departments = _repo.GetAllDepartments().ToList();

            CmbFilterDepartment.Properties.Items.Clear();
            CmbFilterDepartment.Properties.Items.Add(AllDepartmentsLabel);

            foreach (var dept in departments)
                CmbFilterDepartment.Properties.Items.Add(dept.DepartmentName);

            CmbFilterDepartment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            CmbFilterDepartment.SelectedIndex = 0;
        }

        private void LoadFilterStatuses()
        {
            CmbFilterList.Properties.Items.Clear();
            CmbFilterList.Properties.Items.Add(AllStatusesLabel);
            CmbFilterList.Properties.Items.Add(WorkflowStatus.Received);
            CmbFilterList.Properties.Items.Add(WorkflowStatus.Active);

            CmbFilterList.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            CmbFilterList.SelectedIndex = 0;
        }

        private void LoadReviewItems()
        {
            try
            {
                _reviewItems = _repo.GetAuditorReviewItems().ToList();
                MergeDistinctStatusesIntoFilter();

                ReviewItemGC.DataSource = null;
                ReviewItemGC.DataSource = _reviewItems;

                ApplyComboFilters();
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

        private void ApplyComboFilters()
        {
            var filters = new List<string>();

            if (CmbFilterDepartment.SelectedIndex > 0 &&
                !string.Equals(CmbFilterDepartment.Text, AllDepartmentsLabel, StringComparison.OrdinalIgnoreCase))
            {
                filters.Add($"[Department] = '{EscapeFilterValue(CmbFilterDepartment.Text)}'");
            }

            if (CmbFilterList.SelectedIndex > 0 &&
                !string.Equals(CmbFilterList.Text, AllStatusesLabel, StringComparison.OrdinalIgnoreCase))
            {
                filters.Add($"[Status] = '{EscapeFilterValue(CmbFilterList.Text)}'");
            }

            ReviewItemGV.ActiveFilterString = string.Join(" And ", filters);
            UpdateStatistics();
        }

        private static string EscapeFilterValue(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private void UpdateStatistics()
        {
            int total = 0;
            int capitalized = 0;
            int pending = 0;

            for (int rowHandle = 0; rowHandle < ReviewItemGV.DataRowCount; rowHandle++)
            {
                var item = ReviewItemGV.GetRow(rowHandle) as AuditorReviewItemDto;
                if (item == null)
                    continue;

                total++;

                if (string.Equals(item.Capitalizable, "Yes", StringComparison.OrdinalIgnoreCase))
                    capitalized++;

                if (string.Equals(item.Status, WorkflowStatus.Received, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(item.Status, WorkflowStatus.Pending, StringComparison.OrdinalIgnoreCase))
                    pending++;
            }

            TotalAssetsNum.Text = total.ToString();
            CapitalizedNum.Text = capitalized.ToString();
            PendingNum.Text = pending.ToString();
        }

        private void FilterCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyComboFilters();
        }

        private void ReviewItemGV_ColumnFilterChanged(object sender, EventArgs e)
        {
            UpdateStatistics();
        }

        private void ReviewItemGV_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == nameof(AuditorReviewItemDto.Status))
            {
                string val = e.CellValue?.ToString();

                Color bgColor, textColor;

                if (string.Equals(val, "Active", StringComparison.OrdinalIgnoreCase))
                {
                    bgColor = Color.FromArgb(220, 247, 220);
                    textColor = Color.FromArgb(30, 120, 30);
                }
                else if (string.Equals(val, WorkflowStatus.Received, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(val, WorkflowStatus.Pending, StringComparison.OrdinalIgnoreCase))
                {
                    bgColor = Color.FromArgb(255, 243, 200);
                    textColor = Color.FromArgb(160, 100, 0);
                }
                else
                {
                    return;
                }

                DrawBadge(e, val, bgColor, textColor);
                e.Handled = true;
            }

            if (e.Column.FieldName == nameof(AuditorReviewItemDto.Capitalizable))
            {
                string val = e.CellValue?.ToString();

                if (string.Equals(val, "Yes", StringComparison.OrdinalIgnoreCase))
                {
                    DrawBadge(e, val,
                        Color.FromArgb(220, 235, 255),
                        Color.FromArgb(30, 80, 180));
                    e.Handled = true;
                }
            }
        }

        private void DrawBadge(RowCellCustomDrawEventArgs e, string text, Color bgColor, Color textColor)
        {
            Graphics g = e.Graphics;
            e.DefaultDraw();

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
                g.DrawString(text, font, textBrush, badge, sf);
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
