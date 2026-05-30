
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

// namespace nga naglangkob sa dashboard user control
namespace SyncStock.Views.UserControl
{
    // klase nga mao ang dashboard user control, nag-extend sa XtraUserControl
    public partial class DashBoardUC : XtraUserControl
    {
        // gi-instansya ang repository para sa database operations
        private Repository _repo = new Repository();

        // constructor sa dashboard user control
        public DashBoardUC()
        {
            // gi-initialize ang mga components sa form
            InitializeComponent();

            // gi-set ang pending orders grid nga dili ma-edit
            PendingOrdersGV.OptionsBehavior.Editable = false;

            // gi-set ang approved asap orders grid nga dili ma-edit
            ApprovedASAPOrdersGV.OptionsBehavior.Editable = false;

            // gi-subscribe sa load event sa user control
            this.Load += DashBoardUC_Load;

            // gi-apply ang circular shape sa panel 13
            MakeCircularPanel(panelControl13);

            // gi-apply ang circular shape sa panel 14
            MakeCircularPanel(panelControl14);

            // gi-apply ang circular shape sa panel 15
            MakeCircularPanel(panelControl15);

            // gi-apply ang circular shape sa panel 16
            MakeCircularPanel(panelControl16);
        }

        // event handler pag-load sa dashboard user control
        private void DashBoardUC_Load(object sender, EventArgs e)
        {
            // gi-load ang tanan nga dashboard data
            LoadDashboard();

            // gi-calculate ug gi-display ang total pending orders amount
            pendingOrdersLBL.Text =
                "₱" +
                _repo.GetPendingOrderSummary()
                .Sum(x => x.TotalAmount)
                .ToString("N2");

            SumOfReceivedOrders.Text =
        $"₱{_repo.GetReceivedOrdersAmount():N2}";
        }

        // method para himuon nga bilog ang usa ka panel gamit ang graphics path
        private void MakeCircularPanel(PanelControl panel)
        {
            // gi-create ang bag-ong graphics path object
            GraphicsPath path = new GraphicsPath();

            // gi-add ang ellipse nga katumbas sa gidak-on sa panel para himuon nga bilog
            path.AddEllipse(0, 0, panel.Width, panel.Height);

            // gi-apply ang circular region sa panel
            panel.Region = new Region(path);
        }

        // method para i-load ang tanan nga dashboard sections
        private void LoadDashboard()
        {
            // gi-load ang summary cards sa itaas
            LoadCards();

            // gi-load ang pending orders grid
            LoadPendingOrders();

            // gi-load ang asap orders grid
            LoadASAPOrders();

            // gi-load ang monthly purchases chart
            LoadMonthlyChart();

            // gi-load ang asset category pie chart
            LoadAssetCategoryChart();
        }

        // =========================================
        // dashboard cards
        // =========================================

        // method para i-populate ang summary cards sa dashboard
        private void LoadCards()
        {
            // ================================
            // top cards
            // ================================

            // gi-display ang total count sa pending orders
            PendingOrdersNum.Text =
                _repo.GetPendingOrdersCount().ToString();

            // gi-display ang total amount sa received orders
            approvedOrdersLBL.Text =
                "₱" + _repo.GetReceivedOrdersAmount().ToString("N2");

            // gi-display ang total count sa asap orders
            ApproveAsapNum.Text =
                _repo.GetASAPOrders().Count.ToString();

            // gi-display ang total amount sa asap orders
            approvedAsapLBL.Text =
                "₱" + _repo.GetASAPOrdersAmount().ToString("N2");

            // gi-display ang total count sa received orders
            ApprovedOrdersNum.Text =
                _repo.GetReceivedOrdersCount().ToString();

            // gi-display ang overall purchase value
            TotalApprovedValueNum.Text =
                "₱" + _repo.GetOverallPurchaseValue().ToString("N2");

            // gi-display ang overall purchase value sa laing label
            totalApprovedValueLBL.Text =
                "₱" + _repo.GetOverallPurchaseValue().ToString("N2");

            // ================================
            // approved purchase summary
            // ================================

            // gi-display ang total value sa approved orders
            SumOfReceivedOrders.Text =
                "₱" + _repo.GetTotalApprovedValue().ToString("N2");

            // ================================
            // lower summary
            // ================================

            // gi-display ang total count sa tanan nga items sa database
            TotalItemsLBL.Text =
                _repo.GetItemsCount().ToString();

            // gi-display ang total count sa mga departamento
            DepartmentsLBL.Text =
                _repo.GetDepartmentsCount().ToString();

            // gi-display ang average value sa usa ka order
            AverageOrderLBL.Text =
                "₱" + _repo.GetAverageOrderValue().ToString("N2");
        }

        // =========================================
        // pending orders grid
        // =========================================

        // method para i-load ug i-setup ang pending orders grid
        private void LoadPendingOrders()
        {
            // gi-assign ang pending orders data sa grid
            PendingOrdersGC.DataSource =
                _repo.GetPendingOrderSummary().ToList();

            // gi-clear ang mga column sa grid antes mag-populate
            PendingOrdersGV.Columns.Clear();

            // gi-populate ang mga column base sa data
            PendingOrdersGV.PopulateColumns();

            // =====================================
            // grid style
            // =====================================

            // gi-enable ang odd row appearance para sa alternating colors
            PendingOrdersGV.OptionsView.EnableAppearanceOddRow = true;

            // gi-enable ang even row appearance para sa alternating colors
            PendingOrdersGV.OptionsView.EnableAppearanceEvenRow = true;

            // gi-enable ang horizontal scroll bar
            PendingOrdersGV.HorzScrollVisibility =
                DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;

            // gi-enable ang vertical scroll bar
            PendingOrdersGV.VertScrollVisibility =
                DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;

            // gi-set ang odd row background color sa light orange
            PendingOrdersGV.Appearance.OddRow.BackColor =
                Color.FromArgb(250, 244, 235);

            // gi-set ang even row background color sa mas light orange
            PendingOrdersGV.Appearance.EvenRow.BackColor =
                Color.FromArgb(255, 250, 245);

            // gi-set ang focused row background color sa medium orange
            PendingOrdersGV.Appearance.FocusedRow.BackColor =
                Color.FromArgb(240, 224, 200);

            // gi-set ang focused row text color sa dark grey
            PendingOrdersGV.Appearance.FocusedRow.ForeColor =
                Color.FromArgb(30, 30, 30);

            // gi-set ang hidden selection row color sa light beige
            PendingOrdersGV.Appearance.HideSelectionRow.BackColor =
                Color.FromArgb(248, 242, 231);

            // gi-set ang row height sa 32 pixels
            PendingOrdersGV.RowHeight = 32;

            // gi-hide ang group panel sa ibabaw sa grid
            PendingOrdersGV.OptionsView.ShowGroupPanel = false;

            // gi-disable ang auto column width
            PendingOrdersGV.OptionsView.ColumnAutoWidth = false;

            // gi-disable ang highlighted cell appearance
            PendingOrdersGV.OptionsSelection.EnableAppearanceFocusedCell = false;

            // gi-disable ang auto row height
            PendingOrdersGV.OptionsView.RowAutoHeight = false;

            // =====================================
            // hide ids
            // =====================================

            // gi-hide ang purchase order id column
            if (PendingOrdersGV.Columns["PurchaseOrderID"] != null)
                PendingOrdersGV.Columns["PurchaseOrderID"].Visible = false;

            // gi-hide ang department id column
            if (PendingOrdersGV.Columns["DepartmentID"] != null)
                PendingOrdersGV.Columns["DepartmentID"].Visible = false;

            // =====================================
            // column captions
            // =====================================

            // gi-rename ang invoice number column
            if (PendingOrdersGV.Columns["InvoiceNumber"] != null)
                PendingOrdersGV.Columns["InvoiceNumber"].Caption = "Invoice Number";

            // gi-rename ang po number column
            if (PendingOrdersGV.Columns["PONumber"] != null)
                PendingOrdersGV.Columns["PONumber"].Caption = "Purchase Order Number";

            // gi-rename ang department name column
            if (PendingOrdersGV.Columns["DepartmentName"] != null)
                PendingOrdersGV.Columns["DepartmentName"].Caption = "Department";

            // gi-rename ang order date column
            if (PendingOrdersGV.Columns["OrderDate"] != null)
                PendingOrdersGV.Columns["OrderDate"].Caption = "Purchase Order Date";

            // gi-rename ang status column
            if (PendingOrdersGV.Columns["Status"] != null)
                PendingOrdersGV.Columns["Status"].Caption = "PO Status";

            // gi-rename ang priority column
            if (PendingOrdersGV.Columns["Priority"] != null)
                PendingOrdersGV.Columns["Priority"].Caption = "Priority Level";

            // gi-rename ang po type column
            if (PendingOrdersGV.Columns["POType"] != null)
                PendingOrdersGV.Columns["POType"].Caption = "PO Type";

            // gi-rename ang order mode column
            if (PendingOrdersGV.Columns["OrderMode"] != null)
                PendingOrdersGV.Columns["OrderMode"].Caption = "Order Mode";

            // gi-rename ang total items column
            if (PendingOrdersGV.Columns["TotalItems"] != null)
                PendingOrdersGV.Columns["TotalItems"].Caption = "Total Items";

            // gi-rename ang total amount column
            if (PendingOrdersGV.Columns["TotalAmount"] != null)
                PendingOrdersGV.Columns["TotalAmount"].Caption = "Total Amount";

            // =====================================
            // alignment
            // =====================================

            // gi-loop ang matag column para i-apply ang formatting
            foreach (GridColumn col in PendingOrdersGV.Columns)
            {
                // gi-center ang header text
                col.AppearanceHeader.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Center;

                // gi-set ang header font isip bold segoe ui 9pt
                col.AppearanceHeader.Font =
                    new Font("Segoe UI", 9f, FontStyle.Bold);

                // gi-center ang cell text
                col.AppearanceCell.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Center;
            }

            // =====================================
            // money format
            // =====================================

            // gi-set ang total amount column format isip currency
            if (PendingOrdersGV.Columns["TotalAmount"] != null)
            {
                // gi-set ang format type isip numeric
                PendingOrdersGV.Columns["TotalAmount"].DisplayFormat.FormatType =
                    DevExpress.Utils.FormatType.Numeric;

                // gi-set ang format string para ipakita ang peso sign ug 2 decimal places
                PendingOrdersGV.Columns["TotalAmount"].DisplayFormat.FormatString =
                    "₱{0:N2}";
            }

            // =====================================
            // date format
            // =====================================

            // gi-set ang order date column format
            if (PendingOrdersGV.Columns["OrderDate"] != null)
            {
                // gi-set ang format type isip datetime
                PendingOrdersGV.Columns["OrderDate"].DisplayFormat.FormatType =
                    DevExpress.Utils.FormatType.DateTime;

                // gi-set ang format string para ipakita ang buwan, adlaw, ug tuig
                PendingOrdersGV.Columns["OrderDate"].DisplayFormat.FormatString =
                    "MMMM dd, yyyy";
            }

            // =====================================
            // display text
            // =====================================

            // gi-subscribe sa event para i-customize ang display text sa mga column
            PendingOrdersGV.CustomColumnDisplayText += (s, e) =>
            {
                // gi-check kung po type column kini
                if (e.Column.FieldName == "POType")
                {
                    // kung opo, ipakita isip online
                    if (e.Value?.ToString() == "OPO")
                        e.DisplayText = "ONLINE";

                    // kung gpo, ipakita isip local
                    else if (e.Value?.ToString() == "GPO")
                        e.DisplayText = "LOCAL";
                }

                // gi-check kung order mode column kini
                if (e.Column.FieldName == "OrderMode")
                {
                    // kung single, ipakita isip individual
                    if (e.Value?.ToString() == "Single")
                        e.DisplayText = "Individual";

                    // kung grouped, ipakita isip group
                    else if (e.Value?.ToString() == "Grouped")
                        e.DisplayText = "Group";
                }
            };

            // =====================================
            // column widths
            // =====================================

            // gi-set ang lapad sa invoice number column
            PendingOrdersGV.Columns["InvoiceNumber"].Width = 140;

            // gi-set ang lapad sa po number column
            PendingOrdersGV.Columns["PONumber"].Width = 190;

            // gi-set ang lapad sa department name column
            PendingOrdersGV.Columns["DepartmentName"].Width = 250;

            // gi-set ang lapad sa order date column
            PendingOrdersGV.Columns["OrderDate"].Width = 170;

            // gi-set ang lapad sa status column
            PendingOrdersGV.Columns["Status"].Width = 120;

            // gi-set ang lapad sa priority column
            PendingOrdersGV.Columns["Priority"].Width = 170;

            // gi-set ang lapad sa po type column
            PendingOrdersGV.Columns["POType"].Width = 120;

            // gi-set ang lapad sa order mode column
            PendingOrdersGV.Columns["OrderMode"].Width = 120;

            // gi-set ang lapad sa total items column
            PendingOrdersGV.Columns["TotalItems"].Width = 120;

            // gi-set ang lapad sa total amount column
            PendingOrdersGV.Columns["TotalAmount"].Width = 150;
        }

        // =========================================
        // asap orders grid
        // =========================================

        // method para i-load ug i-setup ang asap orders grid
        private void LoadASAPOrders()
        {
            // gi-assign ang asap orders data sa grid
            ApprovedASAPOrdersGC.DataSource =
                _repo.GetASAPOrders().ToList();

            // gi-clear ang mga column sa grid antes mag-populate
            ApprovedASAPOrdersGV.Columns.Clear();

            // gi-populate ang mga column base sa data
            ApprovedASAPOrdersGV.PopulateColumns();

            // =====================================
            // grid style
            // =====================================

            // gi-enable ang odd row appearance
            ApprovedASAPOrdersGV.OptionsView.EnableAppearanceOddRow = true;

            // gi-enable ang even row appearance
            ApprovedASAPOrdersGV.OptionsView.EnableAppearanceEvenRow = true;

            // gi-enable ang horizontal scroll bar
            ApprovedASAPOrdersGV.HorzScrollVisibility =
                DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;

            // gi-enable ang vertical scroll bar
            ApprovedASAPOrdersGV.VertScrollVisibility =
                DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;

            // gi-set ang odd row background color sa light green
            ApprovedASAPOrdersGV.Appearance.OddRow.BackColor =
                Color.FromArgb(223, 242, 223);

            // gi-set ang even row background color sa mas light green
            ApprovedASAPOrdersGV.Appearance.EvenRow.BackColor =
                Color.FromArgb(240, 250, 240);

            // gi-set ang focused row background color sa bright green
            ApprovedASAPOrdersGV.Appearance.FocusedRow.BackColor =
                Color.FromArgb(83, 237, 126);

            // gi-set ang focused row text color sa dark grey
            ApprovedASAPOrdersGV.Appearance.FocusedRow.ForeColor =
                Color.FromArgb(30, 30, 30);

            // gi-set ang hidden selection row color sa soft green
            ApprovedASAPOrdersGV.Appearance.HideSelectionRow.BackColor =
                Color.FromArgb(198, 239, 206);

            // gi-set ang row height sa 32 pixels
            ApprovedASAPOrdersGV.RowHeight = 32;

            // gi-hide ang group panel
            ApprovedASAPOrdersGV.OptionsView.ShowGroupPanel = false;

            // gi-disable ang auto column width
            ApprovedASAPOrdersGV.OptionsView.ColumnAutoWidth = false;

            // gi-disable ang highlighted cell appearance
            ApprovedASAPOrdersGV.OptionsSelection.EnableAppearanceFocusedCell = false;

            // gi-disable ang auto row height
            ApprovedASAPOrdersGV.OptionsView.RowAutoHeight = false;

            // =====================================
            // hide ids
            // =====================================

            // gi-hide ang purchase order id column
            if (ApprovedASAPOrdersGV.Columns["PurchaseOrderID"] != null)
                ApprovedASAPOrdersGV.Columns["PurchaseOrderID"].Visible = false;

            // gi-hide ang department id column
            if (ApprovedASAPOrdersGV.Columns["DepartmentID"] != null)
                ApprovedASAPOrdersGV.Columns["DepartmentID"].Visible = false;

            // =====================================
            // column captions
            // =====================================

            // gi-rename ang invoice number column
            if (ApprovedASAPOrdersGV.Columns["InvoiceNumber"] != null)
                ApprovedASAPOrdersGV.Columns["InvoiceNumber"].Caption = "Invoice Number";

            // gi-rename ang po number column
            if (ApprovedASAPOrdersGV.Columns["PONumber"] != null)
                ApprovedASAPOrdersGV.Columns["PONumber"].Caption = "Purchase Order Number";

            // gi-rename ang department name column
            if (ApprovedASAPOrdersGV.Columns["DepartmentName"] != null)
                ApprovedASAPOrdersGV.Columns["DepartmentName"].Caption = "Department";

            // gi-rename ang order date column
            if (ApprovedASAPOrdersGV.Columns["OrderDate"] != null)
                ApprovedASAPOrdersGV.Columns["OrderDate"].Caption = "Purchase Order Date";

            // gi-rename ang status column
            if (ApprovedASAPOrdersGV.Columns["Status"] != null)
                ApprovedASAPOrdersGV.Columns["Status"].Caption = "PO Status";

            // gi-rename ang priority column
            if (ApprovedASAPOrdersGV.Columns["Priority"] != null)
                ApprovedASAPOrdersGV.Columns["Priority"].Caption = "Priority Level";

            // gi-rename ang po type column
            if (ApprovedASAPOrdersGV.Columns["POType"] != null)
                ApprovedASAPOrdersGV.Columns["POType"].Caption = "PO Type";

            // gi-rename ang order mode column
            if (ApprovedASAPOrdersGV.Columns["OrderMode"] != null)
                ApprovedASAPOrdersGV.Columns["OrderMode"].Caption = "Order Mode";

            // gi-rename ang total items column
            if (ApprovedASAPOrdersGV.Columns["TotalItems"] != null)
                ApprovedASAPOrdersGV.Columns["TotalItems"].Caption = "Total Items";

            // gi-rename ang total amount column
            if (ApprovedASAPOrdersGV.Columns["TotalAmount"] != null)
                ApprovedASAPOrdersGV.Columns["TotalAmount"].Caption = "Total Amount";

            // =====================================
            // alignment
            // =====================================

            // gi-loop ang matag column para i-apply ang formatting
            foreach (GridColumn col in ApprovedASAPOrdersGV.Columns)
            {
                // gi-center ang header text
                col.AppearanceHeader.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Center;

                // gi-set ang header font isip bold segoe ui 9pt
                col.AppearanceHeader.Font =
                    new Font("Segoe UI", 9f, FontStyle.Bold);

                // gi-center ang cell text
                col.AppearanceCell.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Center;
            }

            // =====================================
            // money format
            // =====================================

            // gi-set ang total amount column format isip currency
            if (ApprovedASAPOrdersGV.Columns["TotalAmount"] != null)
            {
                // gi-set ang format type isip numeric
                ApprovedASAPOrdersGV.Columns["TotalAmount"].DisplayFormat.FormatType =
                    DevExpress.Utils.FormatType.Numeric;

                // gi-set ang format string para ipakita ang peso sign ug 2 decimal places
                ApprovedASAPOrdersGV.Columns["TotalAmount"].DisplayFormat.FormatString =
                    "₱{0:N2}";
            }

            // =====================================
            // date format
            // =====================================

            // gi-set ang order date column format
            if (ApprovedASAPOrdersGV.Columns["OrderDate"] != null)
            {
                // gi-set ang format type isip datetime
                ApprovedASAPOrdersGV.Columns["OrderDate"].DisplayFormat.FormatType =
                    DevExpress.Utils.FormatType.DateTime;

                // gi-set ang format string para ipakita ang buwan, adlaw, ug tuig
                ApprovedASAPOrdersGV.Columns["OrderDate"].DisplayFormat.FormatString =
                    "MMMM dd, yyyy";
            }

            // =====================================
            // display text
            // =====================================

            // gi-subscribe sa event para i-customize ang display text sa mga column
            ApprovedASAPOrdersGV.CustomColumnDisplayText += (s, e) =>
            {
                // gi-check kung po type column kini
                if (e.Column.FieldName == "POType")
                {
                    // kung opo, ipakita isip online
                    if (e.Value?.ToString() == "OPO")
                        e.DisplayText = "ONLINE";

                    // kung gpo, ipakita isip local
                    else if (e.Value?.ToString() == "GPO")
                        e.DisplayText = "LOCAL";
                }

                // gi-check kung order mode column kini
                if (e.Column.FieldName == "OrderMode")
                {
                    // kung single, ipakita isip individual
                    if (e.Value?.ToString() == "Single")
                        e.DisplayText = "Individual";

                    // kung grouped, ipakita isip group
                    else if (e.Value?.ToString() == "Grouped")
                        e.DisplayText = "Group";
                }
            };

            // =====================================
            // column widths
            // =====================================

            // gi-set ang lapad sa invoice number column
            ApprovedASAPOrdersGV.Columns["InvoiceNumber"].Width = 140;

            // gi-set ang lapad sa po number column
            ApprovedASAPOrdersGV.Columns["PONumber"].Width = 190;

            // gi-set ang lapad sa department name column
            ApprovedASAPOrdersGV.Columns["DepartmentName"].Width = 250;

            // gi-set ang lapad sa order date column
            ApprovedASAPOrdersGV.Columns["OrderDate"].Width = 170;

            // gi-set ang lapad sa status column
            ApprovedASAPOrdersGV.Columns["Status"].Width = 120;

            // gi-set ang lapad sa priority column
            ApprovedASAPOrdersGV.Columns["Priority"].Width = 170;

            // gi-set ang lapad sa po type column
            ApprovedASAPOrdersGV.Columns["POType"].Width = 120;

            // gi-set ang lapad sa order mode column
            ApprovedASAPOrdersGV.Columns["OrderMode"].Width = 120;

            // gi-set ang lapad sa total items column
            ApprovedASAPOrdersGV.Columns["TotalItems"].Width = 120;

            // gi-set ang lapad sa total amount column
            ApprovedASAPOrdersGV.Columns["TotalAmount"].Width = 150;
        }

        // =========================================
        // monthly chart
        // =========================================

        // method para i-load ang monthly purchases bar chart
        private void LoadMonthlyChart()
        {
            // gi-clear ang tanan nga series sa chart bago mag-populate
            chartControl1.Series.Clear();

            // gi-create ang bag-ong bar chart series nga ginganlag purchases
            Series series = new Series("Purchases", ViewType.Bar);

            // gi-fetch ang monthly purchase summary data gikan sa database
            var data = _repo.GetMonthlyPurchaseSummary();

            // gi-loop ang matag buwan para mag-add sa data point sa chart
            foreach (var item in data)
            {
                // gi-add ang data point gamit ang buwan isip label ug total amount isip value
                series.Points.Add(
                    new SeriesPoint(
                        item.Month,
                        Convert.ToDecimal(item.TotalAmount)));
            }

            // gi-add ang series sa chart control
            chartControl1.Series.Add(series);
        }

        // =========================================
        // pie chart
        // =========================================

        // method para i-load ang asset category pie chart
        private void LoadAssetCategoryChart()
        {
            // gi-clear ang tanan nga series sa pie chart
            AssetCategoryChart.Series.Clear();

            // gi-create ang bag-ong pie chart series nga ginganlag assets
            Series series = new Series("Assets", ViewType.Pie);

            // gi-fetch ang asset category summary data gikan sa database
            var data = _repo.GetAssetCategorySummary();

            // gi-loop ang matag kategorya para mag-add sa pie slice
            foreach (var item in data)
            {
                // gi-add ang pie slice gamit ang kategorya isip label ug total isip value
                series.Points.Add(
                    new SeriesPoint(
                        item.Category,
                        Convert.ToInt32(item.Total)));
            }

            // =====================================
            // pie labels
            // =====================================

            // gi-set ang label pattern para ipakita ang percentage sa matag slice
            series.Label.TextPattern = "{VP:p0}";

            // gi-set ang legend text pattern para ipakita ang kategorya sa legend
            series.LegendTextPattern = "{A}";

            // gi-enable ang labels sa pie chart
            series.LabelsVisibility =
                DevExpress.Utils.DefaultBoolean.True;

            // =====================================
            // legend settings
            // =====================================

            // gi-enable ang visibility sa legend
            AssetCategoryChart.Legend.Visibility =
                DevExpress.Utils.DefaultBoolean.True;

            // gi-set ang legend position sa tuo
            AssetCategoryChart.Legend.AlignmentHorizontal =
                LegendAlignmentHorizontal.Right;

            // gi-set ang legend vertical alignment sa ibabaw sa gawas
            AssetCategoryChart.Legend.AlignmentVertical =
                LegendAlignmentVertical.TopOutside;

            // gi-set ang legend direction gikan sa wala papunta sa tuo
            AssetCategoryChart.Legend.Direction =
                LegendDirection.LeftToRight;

            // =====================================
            // add series
            // =====================================

            // gi-add ang pie series sa chart control
            AssetCategoryChart.Series.Add(series);
        }

        // empty event handler, walay gibuhat
        private void labelControl13_Click(object sender, EventArgs e) { }
    }
}