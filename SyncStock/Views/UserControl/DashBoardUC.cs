
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using SyncStock.Database;
using SyncStock.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace SyncStock.Views.UserControl
{
    public partial class DashBoardUC : XtraUserControl
    {
        private Repository _repo = new Repository();

        public DashBoardUC()
        {
            InitializeComponent();

            PendingOrdersGV.OptionsBehavior.Editable = false;
            ApprovedASAPOrdersGV.OptionsBehavior.Editable = false;

            this.Load += DashBoardUC_Load;

            MakeCircularPanel(panelControl13);
            MakeCircularPanel(panelControl14);
            MakeCircularPanel(panelControl15);
            MakeCircularPanel(panelControl16);

     
        }

        private void DashBoardUC_Load(object sender, EventArgs e)
        {
            LoadDashboard();
            pendingOrdersLBL.Text =
                "₱" +
                _repo.GetPendingOrderSummary()
                .Sum(x => x.TotalAmount)
                .ToString("N2");
        }

        private void MakeCircularPanel(PanelControl panel)
        {
            GraphicsPath path = new GraphicsPath();

            path.AddEllipse(0, 0, panel.Width, panel.Height);

            panel.Region = new Region(path);
        }

        private void LoadDashboard()
        {
            LoadCards();
            LoadPendingOrders();
            LoadASAPOrders();
            LoadMonthlyChart();
            LoadAssetCategoryChart();
        }

        // =========================================
        // DASHBOARD CARDS
        // =========================================

        private void LoadCards()
        {
            // ================================
            // TOP CARDS
            // ================================

            PendingOrdersNum.Text =
                _repo.GetPendingOrdersCount().ToString();

            approvedOrdersLBL.Text =
                "₱" + _repo.GetReceivedOrdersAmount().ToString("N2");

            ApproveAsapNum.Text =
                _repo.GetASAPOrders().Count.ToString();

            approvedAsapLBL.Text =
                "₱" + _repo.GetASAPOrdersAmount().ToString("N2");

            ApprovedOrdersNum.Text =
                _repo.GetReceivedOrdersCount().ToString();

            TotalApprovedValueNum.Text =
                "₱" + _repo.GetOverallPurchaseValue().ToString("N2");

            totalApprovedValueLBL.Text =
                "₱" + _repo.GetOverallPurchaseValue().ToString("N2");

            // ================================
            // APPROVED PURCHASE SUMMARY
            // ================================

            SumOfApprovedOrders.Text =
                "₱" + _repo.GetTotalApprovedValue().ToString("N2");

            NumberOfApprovedOrders.Text =
                _repo.GetApprovedOrdersCount() + " Approved Orders";

            // ================================
            // LOWER SUMMARY
            // ================================

            TotalItemsLBL.Text =
                _repo.GetItemsCount().ToString();

            DepartmentsLBL.Text =
                _repo.GetDepartmentsCount().ToString();

            AverageOrderLBL.Text =
                "₱" + _repo.GetAverageOrderValue().ToString("N2");
        }

        // =========================================
        // PENDING ORDERS GRID
        // =========================================

        private void LoadPendingOrders()
        {
            PendingOrdersGC.DataSource =
                _repo.GetPendingOrderSummary().ToList();

            PendingOrdersGV.Columns.Clear();

            PendingOrdersGV.PopulateColumns();

            // =====================================
            // GRID STYLE
            // =====================================

            PendingOrdersGV.OptionsView.EnableAppearanceOddRow = true;
            PendingOrdersGV.OptionsView.EnableAppearanceEvenRow = true;

            PendingOrdersGV.HorzScrollVisibility =
                DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;

            PendingOrdersGV.VertScrollVisibility =
                DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;

            PendingOrdersGV.Appearance.OddRow.BackColor =
                Color.FromArgb(250, 244, 235);

            PendingOrdersGV.Appearance.EvenRow.BackColor =
                Color.FromArgb(255, 250, 245);

            PendingOrdersGV.Appearance.FocusedRow.BackColor =
                Color.FromArgb(240, 224, 200);

            PendingOrdersGV.Appearance.FocusedRow.ForeColor =
                Color.FromArgb(30, 30, 30);

            PendingOrdersGV.Appearance.HideSelectionRow.BackColor =
                Color.FromArgb(248, 242, 231);

            PendingOrdersGV.RowHeight = 32;

            PendingOrdersGV.OptionsView.ShowGroupPanel = false;

            PendingOrdersGV.OptionsView.ColumnAutoWidth = false;

            PendingOrdersGV.OptionsSelection.EnableAppearanceFocusedCell = false;

            PendingOrdersGV.OptionsView.RowAutoHeight = false;

            // =====================================
            // HIDE IDS
            // =====================================

            if (PendingOrdersGV.Columns["PurchaseOrderID"] != null)
                PendingOrdersGV.Columns["PurchaseOrderID"].Visible = false;

            if (PendingOrdersGV.Columns["DepartmentID"] != null)
                PendingOrdersGV.Columns["DepartmentID"].Visible = false;

            // =====================================
            // COLUMN CAPTIONS
            // =====================================

            if (PendingOrdersGV.Columns["InvoiceNumber"] != null)
                PendingOrdersGV.Columns["InvoiceNumber"].Caption =
                    "Invoice Number";

            if (PendingOrdersGV.Columns["PONumber"] != null)
                PendingOrdersGV.Columns["PONumber"].Caption =
                    "Purchase Order Number";

            if (PendingOrdersGV.Columns["DepartmentName"] != null)
                PendingOrdersGV.Columns["DepartmentName"].Caption =
                    "Department";

            if (PendingOrdersGV.Columns["OrderDate"] != null)
                PendingOrdersGV.Columns["OrderDate"].Caption =
                    "Purchase Order Date";

            if (PendingOrdersGV.Columns["Status"] != null)
                PendingOrdersGV.Columns["Status"].Caption =
                    "PO Status";

            if (PendingOrdersGV.Columns["Priority"] != null)
                PendingOrdersGV.Columns["Priority"].Caption =
                    "Priority Level";

            if (PendingOrdersGV.Columns["POType"] != null)
                PendingOrdersGV.Columns["POType"].Caption =
                    "PO Type";

            if (PendingOrdersGV.Columns["OrderMode"] != null)
                PendingOrdersGV.Columns["OrderMode"].Caption =
                    "Order Mode";

            if (PendingOrdersGV.Columns["TotalItems"] != null)
                PendingOrdersGV.Columns["TotalItems"].Caption =
                    "Total Items";

            if (PendingOrdersGV.Columns["TotalAmount"] != null)
                PendingOrdersGV.Columns["TotalAmount"].Caption =
                    "Total Amount";

            // =====================================
            // ALIGNMENT
            // =====================================

            foreach (GridColumn col in PendingOrdersGV.Columns)
            {
                col.AppearanceHeader.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Center;

                col.AppearanceHeader.Font =
                    new Font("Segoe UI", 9f, FontStyle.Bold);

                col.AppearanceCell.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Center;
            }

            // =====================================
            // MONEY FORMAT
            // =====================================

            if (PendingOrdersGV.Columns["TotalAmount"] != null)
            {
                PendingOrdersGV.Columns["TotalAmount"].DisplayFormat.FormatType =
                    DevExpress.Utils.FormatType.Numeric;

                PendingOrdersGV.Columns["TotalAmount"].DisplayFormat.FormatString =
                    "₱{0:N2}";
            }

            // =====================================
            // DATE FORMAT
            // =====================================

            if (PendingOrdersGV.Columns["OrderDate"] != null)
            {
                PendingOrdersGV.Columns["OrderDate"].DisplayFormat.FormatType =
                    DevExpress.Utils.FormatType.DateTime;

                PendingOrdersGV.Columns["OrderDate"].DisplayFormat.FormatString =
                    "MMMM dd, yyyy";
            }

            // =====================================
            // DISPLAY TEXT
            // =====================================

            PendingOrdersGV.CustomColumnDisplayText += (s, e) =>
            {
                if (e.Column.FieldName == "POType")
                {
                    if (e.Value?.ToString() == "OPO")
                        e.DisplayText = "ONLINE";

                    else if (e.Value?.ToString() == "GPO")
                        e.DisplayText = "LOCAL";
                }

                if (e.Column.FieldName == "OrderMode")
                {
                    if (e.Value?.ToString() == "Single")
                        e.DisplayText = "Single";

                    else if (e.Value?.ToString() == "Grouped")
                        e.DisplayText = "Group";
                }
            };

            // =====================================
            // COLUMN WIDTHS
            // =====================================

            PendingOrdersGV.Columns["InvoiceNumber"].Width = 140;

            PendingOrdersGV.Columns["PONumber"].Width = 190;

            PendingOrdersGV.Columns["DepartmentName"].Width = 250;

            PendingOrdersGV.Columns["OrderDate"].Width = 170;

            PendingOrdersGV.Columns["Status"].Width = 120;

            PendingOrdersGV.Columns["Priority"].Width = 170;

            PendingOrdersGV.Columns["POType"].Width = 120;

            PendingOrdersGV.Columns["OrderMode"].Width = 120;

            PendingOrdersGV.Columns["TotalItems"].Width = 120;

            PendingOrdersGV.Columns["TotalAmount"].Width = 150;
        }



        // =========================================
        // ASAP ORDERS GRID
        // =========================================

        private void LoadASAPOrders()
        {
            ApprovedASAPOrdersGC.DataSource =
                _repo.GetASAPOrders().ToList();

            ApprovedASAPOrdersGV.Columns.Clear();

            ApprovedASAPOrdersGV.PopulateColumns();

            // =====================================
            // GRID STYLE
            // =====================================

            ApprovedASAPOrdersGV.OptionsView.EnableAppearanceOddRow = true;
            ApprovedASAPOrdersGV.OptionsView.EnableAppearanceEvenRow = true;

            ApprovedASAPOrdersGV.HorzScrollVisibility =
                DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;

            ApprovedASAPOrdersGV.VertScrollVisibility =
                DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;

            ApprovedASAPOrdersGV.Appearance.OddRow.BackColor =
                Color.FromArgb(223, 242, 223);

            ApprovedASAPOrdersGV.Appearance.EvenRow.BackColor =
                Color.FromArgb(240, 250, 240);

            ApprovedASAPOrdersGV.Appearance.FocusedRow.BackColor =
                Color.FromArgb(83, 237, 126);

            ApprovedASAPOrdersGV.Appearance.FocusedRow.ForeColor =
                Color.FromArgb(30, 30, 30);

            ApprovedASAPOrdersGV.Appearance.HideSelectionRow.BackColor =
                Color.FromArgb(198, 239, 206);

            ApprovedASAPOrdersGV.RowHeight = 32;

            ApprovedASAPOrdersGV.OptionsView.ShowGroupPanel = false;

            ApprovedASAPOrdersGV.OptionsView.ColumnAutoWidth = false;

            ApprovedASAPOrdersGV.OptionsSelection.EnableAppearanceFocusedCell = false;

            ApprovedASAPOrdersGV.OptionsView.RowAutoHeight = false;

            // =====================================
            // HIDE IDS
            // =====================================

            if (ApprovedASAPOrdersGV.Columns["PurchaseOrderID"] != null)
                ApprovedASAPOrdersGV.Columns["PurchaseOrderID"].Visible = false;

            if (ApprovedASAPOrdersGV.Columns["DepartmentID"] != null)
                ApprovedASAPOrdersGV.Columns["DepartmentID"].Visible = false;

            // =====================================
            // COLUMN CAPTIONS
            // =====================================

            if (ApprovedASAPOrdersGV.Columns["InvoiceNumber"] != null)
                ApprovedASAPOrdersGV.Columns["InvoiceNumber"].Caption =
                    "Invoice Number";

            if (ApprovedASAPOrdersGV.Columns["PONumber"] != null)
                ApprovedASAPOrdersGV.Columns["PONumber"].Caption =
                    "Purchase Order Number";

            if (ApprovedASAPOrdersGV.Columns["DepartmentName"] != null)
                ApprovedASAPOrdersGV.Columns["DepartmentName"].Caption =
                    "Department";

            if (ApprovedASAPOrdersGV.Columns["OrderDate"] != null)
                ApprovedASAPOrdersGV.Columns["OrderDate"].Caption =
                    "Purchase Order Date";

            if (ApprovedASAPOrdersGV.Columns["Status"] != null)
                ApprovedASAPOrdersGV.Columns["Status"].Caption =
                    "PO Status";

            if (ApprovedASAPOrdersGV.Columns["Priority"] != null)
                ApprovedASAPOrdersGV.Columns["Priority"].Caption =
                    "Priority Level";

            if (ApprovedASAPOrdersGV.Columns["POType"] != null)
                ApprovedASAPOrdersGV.Columns["POType"].Caption =
                    "PO Type";

            if (ApprovedASAPOrdersGV.Columns["OrderMode"] != null)
                ApprovedASAPOrdersGV.Columns["OrderMode"].Caption =
                    "Order Mode";

            if (ApprovedASAPOrdersGV.Columns["TotalItems"] != null)
                ApprovedASAPOrdersGV.Columns["TotalItems"].Caption =
                    "Total Items";

            if (ApprovedASAPOrdersGV.Columns["TotalAmount"] != null)
                ApprovedASAPOrdersGV.Columns["TotalAmount"].Caption =
                    "Total Amount";

            // =====================================
            // ALIGNMENT
            // =====================================

            foreach (GridColumn col in ApprovedASAPOrdersGV.Columns)
            {
                col.AppearanceHeader.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Center;

                col.AppearanceHeader.Font =
                    new Font("Segoe UI", 9f, FontStyle.Bold);

                col.AppearanceCell.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Center;
            }

            // =====================================
            // MONEY FORMAT
            // =====================================

            if (ApprovedASAPOrdersGV.Columns["TotalAmount"] != null)
            {
                ApprovedASAPOrdersGV.Columns["TotalAmount"].DisplayFormat.FormatType =
                    DevExpress.Utils.FormatType.Numeric;

                ApprovedASAPOrdersGV.Columns["TotalAmount"].DisplayFormat.FormatString =
                    "₱{0:N2}";
            }

            // =====================================
            // DATE FORMAT
            // =====================================

            if (ApprovedASAPOrdersGV.Columns["OrderDate"] != null)
            {
                ApprovedASAPOrdersGV.Columns["OrderDate"].DisplayFormat.FormatType =
                    DevExpress.Utils.FormatType.DateTime;

                ApprovedASAPOrdersGV.Columns["OrderDate"].DisplayFormat.FormatString =
                    "MMMM dd, yyyy";
            }

            // =====================================
            // DISPLAY TEXT
            // =====================================

            ApprovedASAPOrdersGV.CustomColumnDisplayText += (s, e) =>
            {
                if (e.Column.FieldName == "POType")
                {
                    if (e.Value?.ToString() == "OPO")
                        e.DisplayText = "ONLINE";

                    else if (e.Value?.ToString() == "GPO")
                        e.DisplayText = "LOCAL";
                }

                if (e.Column.FieldName == "OrderMode")
                {
                    if (e.Value?.ToString() == "Single")
                        e.DisplayText = "Single";

                    else if (e.Value?.ToString() == "Grouped")
                        e.DisplayText = "Group";
                }
            };

            // =====================================
            // COLUMN WIDTHS
            // =====================================

            ApprovedASAPOrdersGV.Columns["InvoiceNumber"].Width = 140;

            ApprovedASAPOrdersGV.Columns["PONumber"].Width = 190;

            ApprovedASAPOrdersGV.Columns["DepartmentName"].Width = 250;

            ApprovedASAPOrdersGV.Columns["OrderDate"].Width = 170;

            ApprovedASAPOrdersGV.Columns["Status"].Width = 120;

            ApprovedASAPOrdersGV.Columns["Priority"].Width = 170;

            ApprovedASAPOrdersGV.Columns["POType"].Width = 120;

            ApprovedASAPOrdersGV.Columns["OrderMode"].Width = 120;

            ApprovedASAPOrdersGV.Columns["TotalItems"].Width = 120;

            ApprovedASAPOrdersGV.Columns["TotalAmount"].Width = 150;
        }

        // =========================================
        // MONTHLY CHART
        // =========================================

        private void LoadMonthlyChart()
        {
            chartControl1.Series.Clear();

            Series series =
                new Series("Purchases", ViewType.Bar);

            var data = _repo.GetMonthlyPurchaseSummary();

            foreach (var item in data)
            {
                series.Points.Add(
                    new SeriesPoint(
                        item.Month,
                        Convert.ToDecimal(item.TotalAmount)));
            }

            chartControl1.Series.Add(series);
        }

        // =========================================
        // PIE CHART
        // =========================================

        private void LoadAssetCategoryChart()
        {
            AssetCategoryChart.Series.Clear();

            // CREATE PIE SERIES
            Series series =
                new Series("Assets", ViewType.Pie);

            // GET DATA
            var data = _repo.GetAssetCategorySummary();

            // ADD PIE SLICES
            foreach (var item in data)
            {
                series.Points.Add(
                    new SeriesPoint(
                        item.Category,
                        Convert.ToInt32(item.Total)));
            }

            // =====================================
            // PIE LABELS
            // =====================================

            // SHOW PERCENTAGE ON PIE
            series.Label.TextPattern = "{VP:p0}";

            // SHOW CATEGORY NAMES IN LEGEND
            series.LegendTextPattern = "{A}";

            // ENABLE LABELS
            series.LabelsVisibility =
                DevExpress.Utils.DefaultBoolean.True;

            // =====================================
            // LEGEND SETTINGS
            // =====================================

            AssetCategoryChart.Legend.Visibility =
                DevExpress.Utils.DefaultBoolean.True;

            AssetCategoryChart.Legend.AlignmentHorizontal =
                LegendAlignmentHorizontal.Right;

            AssetCategoryChart.Legend.AlignmentVertical =
                LegendAlignmentVertical.TopOutside;

            AssetCategoryChart.Legend.Direction =
                LegendDirection.LeftToRight;

            // =====================================
            // ADD SERIES
            // =====================================

            AssetCategoryChart.Series.Add(series);
        }
    }
}

