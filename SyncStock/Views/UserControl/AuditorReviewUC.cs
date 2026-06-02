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
using SyncStock.Models.Accounts;

namespace SyncStock.Views.UserControl
{
    public partial class AuditorReviewUC : DevExpress.XtraEditors.XtraUserControl
    {
        // Mga filter label nga makita sa combo boxes
        private const string AllDepartmentsLabel = "All Departments";
        private const string AllStatusesLabel = "All Statuses";
        private const string AllMonthsLabel = "All Months";

        // Para sa currently selected item sa grid
        private AuditorReviewItemDto _selectedItem;

        // Role sa current logged in user
        private string CurrentUserRole { get; set; }

        // User ID sa current logged in user
        private string CurrentUserId { get; set; }

        // Gina check kung Admin ba ang current user
        // Aron ma-restrict ang critical actions like sa edit,
        // lock/unlock, ug approval sa authorized users lang
        // Dayun mubalik ug TRUE kung Admin ang role
        private bool IsAdmin =>
            string.Equals(
                CurrentUserRole,
                "Admin",
                StringComparison.OrdinalIgnoreCase);

        // Mubalik ug TRUE kung ang user pwede mag-lock ug bulan
        // Sa karon, Admin ra ang makahimo niini
        private bool CanLockDirectly => string.Equals(CurrentUserRole, "Admin", StringComparison.OrdinalIgnoreCase);


        // Pill labels nga makita sa grid para sa capitalizable column
        private const string PillCapitalizable = "Capitalizable";
        private const string PillNonCapitalizable = "Expenses";

        // Pill labels para sa status column
        private const string PillActive = "Approved";
        private const string PillPendingReview = "To be Approved";
        private const string PillApproved = "Approved";

        // Repository object para sa database operations
        // Aron centralized ug reusable ang DB access
        // Matawag na dayun ang methods sud sa repository para makuha ang data nga needed sa UI
        private readonly Repository _repo = new Repository();

        // Listahan sa tanan review items gikan sa database
        // Aron adunay local copy sa loaded records
        // Available ang loaded data para sa filtering ug searching
        private List<AuditorReviewItemDto> _reviewItems = new List<AuditorReviewItemDto>();

        // Aron available ang user details sa tibuok class
        private readonly User _currentUser;

        // Default constructor para sa designer
        // Gigamit lang kung naghimo ug form sa design time
        public AuditorReviewUC()
        {
            InitializeComponent();
        }

        // Main constructor nga mudawat sa current user
        // Aron ma-setup ang permissions ug screen
        // based sa logged-in user
        public AuditorReviewUC(User currentUser)
        {
            InitializeComponent();

            // I-save ang user object para magamit sa uban nga methods
            _currentUser = currentUser;

            // I-kuha ang role ug username gikan sa user object
            CurrentUserRole = _currentUser.Role;
            CurrentUserId = _currentUser.UserName;

            // Kung naa sa design mode, undangon na ang setup
            // Para dili mag-crash ang designer
            if (IsDesignTime())
                return;

            // Sugdi ang pag-setup sa screen
            InitializeReviewScreen();
        }

        // Mubalik ug TRUE kung naa sa design mode ang form
        // Gigamit para dili mag-run ang DB calls sa designer
        private bool IsDesignTime() =>
            DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        // I-setup ang tanang components sa review screen
        // Tibuok initialization nga gi-group dinhi para maayo ang order
        private void InitializeReviewScreen()
        {
            // Dili pwede i-edit ang grid directly
            ReviewItemGV.OptionsBehavior.Editable = false;

            // Itago ang auto filter row sa tuktok sa grid
            ReviewItemGV.OptionsView.ShowAutoFilterRow = false;

            // I-configure ang search box
            ConfigureSearchControl();

            // I-configure ang mga column sa grid
            ConfigureGridColumns();

            // I-load ang mga department sa filter combo box
            LoadFilterDepartments();

            // I-load ang mga status sa filter combo box
            LoadFilterStatuses();

            // I-load ang mga review item gikan sa database
            LoadReviewItems();

            // I-load ang mga bulan sa filter combo box
            // (ginabuhat human sa LoadReviewItems kay gikinahanglan ang data)
            LoadFilterMonths();

            // I-hook ang event handlers para sa grid ug filters
            ReviewItemGV.FocusedRowChanged += ReviewItemGV_FocusedRowChanged;
            CmbFilterDepartment.SelectedIndexChanged += FilterChanged;
            CmbFilterList.SelectedIndexChanged += FilterChanged;
            CmbDate.SelectedIndexChanged += FilterChanged;
            ScFilter.EditValueChanged += FilterChanged;

            // I-highlight ang tibuok row kung gi-click
            ReviewItemGV.FocusRectStyle =
                DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;

            // Dili na kinahanglan i-highlight ang individual cell
            ReviewItemGV.OptionsSelection.EnableAppearanceFocusedCell = false;

            // Usa ra ka row ang mapili sa usa ka higayon
            ReviewItemGV.OptionsSelection.MultiSelect = false;

            // I-disable ang edit ug remarks buttons sa una
            // Mu-enable ra sila kung may napili na nga row
            BtnEdit.Enabled = false;
            BtnRemarks.Enabled = false;

            // I-hook ang custom draw para sa unbound columns (OrderType ug OrderMode)
            ReviewItemGV.CustomUnboundColumnData +=
                ReviewItemGV_CustomUnboundColumnData;

            // I-update ang Lock/Unlock button depende sa pinili nga bulan
            RefreshLockButtonState();
        }

        // Gina-supply ang data para sa unbound columns sa grid
        // Kay dili sila direkta gikan sa model, kailangan i-compute ang value
        private void ReviewItemGV_CustomUnboundColumnData(
            object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            // Kung dili get data operation, undangon na
            if (!e.IsGetData)
                return;

            // I-cast ang row ngadto sa DTO para ma-access ang properties
            var item = e.Row as AuditorReviewItemDto;

            // Kung null ang row, undangon na
            if (item == null)
                return;

            // Kung OrderType column, i-convert ang GPO/non-GPO ngadto sa LOCAL/ONLINE
            if (e.Column == colOrderType)
            {
                e.Value =
                    item.POType == "GPO"
                    ? "LOCAL"
                    : "ONLINE";
            }

            // Kung OrderMode column, i-convert ang Group/non-Group ngadto sa GROUP/SINGLE
            if (e.Column == colOrderMode)
            {
                e.Value =
                    item.OrderMode == "Group"
                    ? "GROUP"
                    : "SINGLE";
            }
        }

        // I-configure ang search box
        // Ibutang ang placeholder text para mahibalo ang user unsa ang i-type
        private void ConfigureSearchControl()
        {
            ScFilter.Properties.NullValuePrompt = "Search accepted receipts...";
        }

        // I-bind ang mga column sa grid sa DTO properties
        // Aron mahibalo ang grid unsa ang i-display sa matag column
        private void ConfigureGridColumns()
        {
            // I-bind ang matag column sa katumbas nga property sa DTO
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

            // Unbound columns — ang data ginacompute, dili direkta gikan sa DTO
            colOrderType.UnboundType = DevExpress.Data.UnboundColumnType.String;
            colOrderMode.UnboundType = DevExpress.Data.UnboundColumnType.String;

            // I-format ang unit price ug total amount para may comma ug 2 decimal places
            colUnitPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colUnitPrice.DisplayFormat.FormatString = "N2";
            colTotalAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colTotalAmount.DisplayFormat.FormatString = "N2";

            // I-format ang date para mas madaling basahon (e.g. 01 Jan 2025)
            colDateReceived.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colDateReceived.DisplayFormat.FormatString = "dd MMM yyyy";
        }

        // I-load ang mga department gikan sa database
        // Ibutang sa combo box para magamit sa filter
        private void LoadFilterDepartments()
        {
            var departments = _repo.GetAllDepartments().ToList();

            // I-clear una para dili mag-duplicate kung ma-reload
            CmbFilterDepartment.Properties.Items.Clear();

            // Ibutang ang "All Departments" bilang unang option
            CmbFilterDepartment.Properties.Items.Add(AllDepartmentsLabel);

            // I-add ang matag department gikan sa database
            foreach (var dept in departments)
                CmbFilterDepartment.Properties.Items.Add(dept.DepartmentName);

            // Dili pwede mag-type sa combo box, pinili ra ang pwede
            CmbFilterDepartment.Properties.TextEditStyle =
                DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            // Pilion ang "All Departments" sa default
            CmbFilterDepartment.SelectedIndex = 0;
        }

        // I-load ang mga status option sa filter combo box
        // Manual ra ang listahan, dili gikan sa database
        private void LoadFilterStatuses()
        {
            CmbFilterList.Properties.Items.Clear();

            // I-add ang "All Statuses" bilang unang option
            CmbFilterList.Properties.Items.Add(AllStatusesLabel);

            // I-add ang duha ka main statuses
            CmbFilterList.Properties.Items.Add("Pending Review");
            CmbFilterList.Properties.Items.Add(WorkflowStatus.Active);

            // Dili pwede mag-type, pinili ra
            CmbFilterList.Properties.TextEditStyle =
                DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            // Pilion ang "All Statuses" sa default
            CmbFilterList.SelectedIndex = 0;
        }

        // I-load ang mga bulan gikan sa loaded review items
        // Aron makita sa filter kung unsang mga bulan adunay datos
        private void LoadFilterMonths()
        {
            CmbDate.Properties.Items.Clear();

            // I-add ang "All Months" bilang unang option
            CmbDate.Properties.Items.Add(AllMonthsLabel);

            // Kuhaon ang distinct nga mga bulan gikan sa loaded items
            // I-group sa year-month level aron dili mag-duplicate
            var months = _reviewItems
                .Where(x => x.DateReceived != default(DateTime))
                .Select(x => new DateTime(x.DateReceived.Year, x.DateReceived.Month, 1))
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            // Isiguro nga ang current bulan kanunay naa sa listahan
            // Bisan pa kung walay datos niini
            var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            if (!months.Contains(currentMonth))
                months.Insert(0, currentMonth);

            // I-add ang matag bulan sa combo box (pinakabag-o una)
            foreach (var month in months.OrderByDescending(d => d))
                CmbDate.Properties.Items.Add(month.ToString("MMMM yyyy"));

            // Dili pwede mag-type, pinili ra
            CmbDate.Properties.TextEditStyle =
                DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            // Pilion ang current bulan sa default
            string currentMonthLabel = currentMonth.ToString("MMMM yyyy");
            int idx = CmbDate.Properties.Items.IndexOf(currentMonthLabel);
            CmbDate.SelectedIndex = idx >= 0 ? idx : 0;
        }

        // I-load ang mga review item gikan sa database
        // I-refresh usab ang grid ug statistics human ma-load
        private void LoadReviewItems()
        {
            try
            {
                // Kuhaon ang tanang review items gikan sa repository
                _reviewItems = _repo.GetAuditorReviewItems().ToList();

                // I-add ang bag-o nga statuses sa filter combo box kung naa
                MergeDistinctStatusesIntoFilter();

                // I-apply ang mga filter ug i-refresh ang grid
                ApplyFiltersAndRefresh();
            }
            catch (Exception ex)
            {
                // Ipakita ang error kung may problema sa pag-load
                XtraMessageBox.Show(
                    $"Unable to load accepted receiving items.{Environment.NewLine}{ex.Message}",
                    "Auditor Review",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // I-add ang bag-o nga statuses sa filter combo box
        // Gikan sa actual data sa database, dili lang sa hardcoded list
        private void MergeDistinctStatusesIntoFilter()
        {
            // I-save ang currently selected status para dili ma-reset
            var selected = CmbFilterList.Text;

            // Listahan sa mga status nga nahibal-an na namo
            var known = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                AllStatusesLabel,
                WorkflowStatus.Received,
                WorkflowStatus.Active,
                WorkflowStatus.Pending
            };

            // I-add ang bag-o nga statuses kung wala pa sa listahan
            foreach (var status in _reviewItems
                .Select(x => x.Status)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (known.Add(status))
                    CmbFilterList.Properties.Items.Add(status);
            }

            // I-restore ang dati nga napili nga status kung valid pa
            if (!string.IsNullOrEmpty(selected) && CmbFilterList.Properties.Items.Contains(selected))
                CmbFilterList.Text = selected;
            else if (CmbFilterList.SelectedIndex < 0)
                CmbFilterList.SelectedIndex = 0;
        }

        // I-apply ang tanan filters ug i-refresh ang grid ug statistics
        private void ApplyFiltersAndRefresh()
        {
            // Kuhaon ang filtered list base sa mga napili nga filter
            var filtered = GetFilteredReviewItems().ToList();

            // I-clear ang existing filters sa grid
            ReviewItemGV.ActiveFilterString = string.Empty;
            ReviewItemGV.FindFilterText = string.Empty;

            // I-bind ang filtered data sa grid
            ReviewItemGC.DataSource = null;
            ReviewItemGC.DataSource = filtered;

            // I-update ang statistics cards sa ibabaw
            UpdateStatistics(filtered, filtered.Sum(x => x.Quantity));
        }

        // Ibalik ang filtered na listahan base sa mga napili nga filter
        // I-apply ang department, status, bulan, ug search text filters
        private List<AuditorReviewItemDto> GetFilteredReviewItems()
        {
            IEnumerable<AuditorReviewItemDto> items = _reviewItems;

            // I-filter by department kung may napili ug dili "All Departments"
            if (CmbFilterDepartment.SelectedIndex > 0 &&
                !string.Equals(CmbFilterDepartment.Text, AllDepartmentsLabel, StringComparison.OrdinalIgnoreCase))
            {
                string department = CmbFilterDepartment.Text;
                items = items.Where(x =>
                    string.Equals(x.Department, department, StringComparison.OrdinalIgnoreCase));
            }

            // I-filter by status kung may napili ug dili "All Statuses"
            if (CmbFilterList.SelectedIndex > 0 &&
                !string.Equals(CmbFilterList.Text, AllStatusesLabel, StringComparison.OrdinalIgnoreCase))
            {
                string status = CmbFilterList.Text;
                items = items.Where(x =>
                    string.Equals(x.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            // I-filter by bulan kung may napili ug dili "All Months"
            if (CmbDate.SelectedIndex > 0 &&
                !string.Equals(CmbDate.Text, AllMonthsLabel, StringComparison.OrdinalIgnoreCase) &&
                DateTime.TryParseExact(
                    CmbDate.Text,
                    "MMMM yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime selectedMonth))
            {
                // Kuhaon ang unang ug katapusan nga adlaw sa napili nga bulan
                var start = selectedMonth.Date;
                var end = selectedMonth.AddMonths(1).AddDays(-1).Date;
                items = items.Where(x =>
                {
                    var received = x.DateReceived.Date;
                    return received >= start && received <= end;
                });
            }

            // I-filter by search text kung may gi-type ang user
            string searchText = GetSearchText();
            if (!string.IsNullOrWhiteSpace(searchText))
                items = items.Where(x => MatchesSearch(x, searchText));

            return items.ToList();
        }

        // Kuhaon ang search text gikan sa search control
        // Duha ka paagi ang gicheck para mas sigurado
        private string GetSearchText()
        {
            string text = Convert.ToString(ScFilter.EditValue);
            if (string.IsNullOrWhiteSpace(text))
                text = ScFilter.Text;

            return text?.Trim() ?? string.Empty;
        }

        // Icheck kung ang usa ka item nag-match sa search text
        // I-search sa daghang fields para mas komprehensibo
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

        // Helper method: icheck kung adunay gihanap nga text sa usa ka string
        // Case-insensitive ang search
        private static bool Contains(string value, string search)
        {
            return !string.IsNullOrEmpty(value)
                && value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // Icheck kung ang item kay capitalizable
        // Mubalik ug TRUE kung ang Capitalizable property kay "Yes"
        private static bool IsCapitalized(AuditorReviewItemDto item) =>
            string.Equals(item.Capitalizable, "Yes", StringComparison.OrdinalIgnoreCase);

        // Icheck kung ang item kay naa pa sa Pending Review status
        private static bool IsPendingReview(AuditorReviewItemDto item) =>
            string.Equals(item.Status, "Pending Review", StringComparison.OrdinalIgnoreCase);

        // I-update ang statistics cards sa ibabaw sa screen
        // Gipakita ang total value, capitalized count, ug pending count
        private void UpdateStatistics(IReadOnlyList<AuditorReviewItemDto> items, int totalQuantity)
        {
            // Ihap ang capitalized ug pending items
            int capitalizedCount = items.Count(IsCapitalized);
            int pendingCount = items.Count(IsPendingReview);

            // Ikwenta ang total value sa capitalized ug tanan items
            decimal capitalizedValue = items.Where(IsCapitalized).Sum(x => x.TotalAmount);
            decimal totalValue = items.Sum(x => x.TotalAmount);

            // I-display ang mga computed values sa labels
            TotalAssetsNum.Text = "₱" + totalValue.ToString("N2");
            labelControl5.Text = GetPeriodLabelText(totalQuantity);

            CapitalizedNum.Text = capitalizedCount.ToString();
            labelControl8.Text = "₱" + capitalizedValue.ToString("N2");
            PendingNum.Text = pendingCount.ToString();
        }

        // Ibalik ang text para sa period label sa statistics
        // Kung may pinili nga bulan, ipakita ang bulan; kung wala, "Current Period" ra
        private string GetPeriodLabelText(int totalQuantity)
        {
            // Kung may pinili nga specific nga bulan, ipakita kana
            if (CmbDate.SelectedIndex > 0 &&
                !string.Equals(CmbDate.Text, AllMonthsLabel, StringComparison.OrdinalIgnoreCase))
            {
                return $"{CmbDate.Text} · {totalQuantity:N0} items";
            }

            // Kung "All Months" ang pinili, ipakita ang "Current Period"
            return $"{totalQuantity:N0} items · Current Period";
        }

        // Gi-trigger kung mag-usab ang bisan unsang filter
        // I-refresh ang grid ug ang Lock/Unlock button state
        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndRefresh();

            // I-update ang Lock/Unlock button depende sa napili nga bulan
            RefreshLockButtonState();
        }

        // Gi-trigger kung mag-usab ang napili nga row sa grid
        // I-enable o i-disable ang Edit ug Remarks buttons depende sa selection
        private void ReviewItemGV_FocusedRowChanged(
            object sender,
            FocusedRowChangedEventArgs e)
        {
            // Kuhaon ang napili nga item gikan sa grid
            _selectedItem =
                ReviewItemGV.GetRow(e.FocusedRowHandle)
                as AuditorReviewItemDto;

            bool hasSelection = _selectedItem != null;

            // Check if the selected item's month is locked
            bool isItemMonthLocked = false;
            if (_selectedItem != null)
            {
                var itemMonth = new DateTime(_selectedItem.DateReceived.Year, _selectedItem.DateReceived.Month, 1);
                var lockRecord = _repo.GetMonthLock(itemMonth);
                isItemMonthLocked = lockRecord != null && lockRecord.IsLocked;
            }

            // Pwede lang i-edit ang item kung:
            // 1. May napili nga row
            // 2. Dili locked ang month sa item
            // 3. Dili "Pending Review" ang status (approved na)
            bool canEdit =
                hasSelection &&
                !isItemMonthLocked &&
                !string.Equals(
                    _selectedItem.Status,
                    "Pending Review",
                    StringComparison.OrdinalIgnoreCase);

            // Edit button: Admin ra ang makaka-edit
            BtnEdit.Enabled =
                canEdit &&
                IsAdmin;

            // Remarks button: Admin ra, ug kinahanglan adunay sulod ang remarks
            BtnRemarks.Enabled =
                canEdit &&
                IsAdmin &&
                !string.IsNullOrWhiteSpace(_selectedItem.Remarks);
        }

        // Custom draw handler para sa Status ug Capitalizable columns
        // Gina-draw ang pill/badge nga design imbes plain text
        private void ReviewItemGV_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            // Kung Status column, i-draw ang status pill
            if (e.Column.FieldName == nameof(AuditorReviewItemDto.Status))
            {
                var item = ReviewItemGV.GetRow(e.RowHandle) as AuditorReviewItemDto;
                string status = e.CellValue?.ToString();

                if (string.IsNullOrWhiteSpace(status) && item != null)
                {
                    status = item.IsCapitalizable ? WorkflowStatus.Active : WorkflowStatus.Received;
                }

                if (!TryGetStatusPill(status, out string label, out Color bgColor, out Color textColor))
                    return;

                DrawBadge(e, label, bgColor, textColor);
                return;
            }

            // Kung Capitalizable column, i-draw ang capitalizable pill
            if (e.Column.FieldName == nameof(AuditorReviewItemDto.Capitalizable))
            {
                if (!TryGetCapitalizablePill(e.CellValue?.ToString(), out string label, out Color bgColor, out Color textColor))
                    return;

                DrawBadge(e, label, bgColor, textColor);
            }
        }

        // Ibalik ang label, background color, ug text color para sa status pill
        // Mubalik ug FALSE kung ang status wala sa listahan
        private static bool TryGetStatusPill(
    string status,
    out string label,
    out Color bgColor,
    out Color textColor)
        {
            label = null;
            bgColor = Color.Empty;
            textColor = Color.Empty;

            if (string.IsNullOrWhiteSpace(status))
            {
                label = "Unknown";
                bgColor = Color.LightGray;
                textColor = Color.Black;
                return true;
            }

            status = status.Trim();

            if (
                status.Equals(WorkflowStatus.Active, StringComparison.OrdinalIgnoreCase) ||
                status.Equals(WorkflowStatus.Approved, StringComparison.OrdinalIgnoreCase) ||
                status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                label = "Approved";
                bgColor = Color.FromArgb(220, 247, 220);
                textColor = Color.FromArgb(30, 120, 30);
                return true;
            }

            if (
                status.Equals(WorkflowStatus.Pending, StringComparison.OrdinalIgnoreCase) ||
                status.Equals("Pending Review", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("To be Approved", StringComparison.OrdinalIgnoreCase))
            {
                label = "To be Approved";
                bgColor = Color.FromArgb(255, 243, 200);
                textColor = Color.FromArgb(160, 100, 0);
                return true;
            }

            if (
                status.Equals(WorkflowStatus.Received, StringComparison.OrdinalIgnoreCase))
            {
                label = "Approved";
                bgColor = Color.FromArgb(220, 247, 220);
                textColor = Color.FromArgb(30, 120, 30);
                return true;
            }

            return false;
        }

        // Ibalik ang label, background color, ug text color para sa capitalizable pill
        // Mubalik ug FALSE kung ang value wala sa listahan
        private static bool TryGetCapitalizablePill(string value, out string label, out Color bgColor, out Color textColor)
        {
            label = null;
            bgColor = Color.Empty;
            textColor = Color.Empty;

            // "Yes" = Capitalizable: asul ang pill
            if (string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase))
            {
                label = PillCapitalizable;
                bgColor = Color.FromArgb(220, 235, 255);
                textColor = Color.FromArgb(30, 80, 180);
                return true;
            }

            // "No" = Expense: gray ang pill
            if (string.Equals(value, "No", StringComparison.OrdinalIgnoreCase))
            {
                label = PillNonCapitalizable;
                bgColor = Color.FromArgb(243, 244, 246);
                textColor = Color.FromArgb(75, 85, 99);
                return true;
            }

            return false;
        }

        // I-draw ang rounded pill/badge sa grid cell
        // Gigamit para sa Status ug Capitalizable columns
        private static void DrawBadge(RowCellCustomDrawEventArgs e, string label, Color bgColor, Color textColor)
        {
            // I-mark ang cell as handled para dili na i-draw ang default
            e.Handled = true;

            // I-fill ang cell background gamit ang default appearance
            e.Appearance.FillRectangle(e.Cache, e.Bounds);

            Graphics g = e.Graphics;

            // Padding sa sulod ug gawas sa badge
            int padX = 8, padY = 4;
            Rectangle cell = e.Bounds;

            // Kwentahon ang rectangle para sa badge mismo
            Rectangle badge = new Rectangle(
                cell.X + padX,
                cell.Y + padY,
                cell.Width - (padX * 2),
                cell.Height - (padY * 2));

            // I-draw ang rounded rectangle background
            using (GraphicsPath path = GetRoundedRect(badge, 10))
            using (SolidBrush brush = new SolidBrush(bgColor))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(brush, path);
            }

            // I-draw ang text sa sentro sa badge
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

        // Maghimo ug GraphicsPath nga adunay rounded corners
        // Gigamit para sa pill/badge drawing
        private static GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            // I-draw ang arc sa matag sulok sa rectangle
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.X + rect.Width - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.X + rect.Width - d, rect.Y + rect.Height - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        // Gi-trigger kung gi-click ang Lock button
        // Admin ra ang makaka-lock/unlock sa usa ka bulan
        private void BtnLock_Click(object sender, EventArgs e)
        {
            // Kinahanglan may pinili nga specific nga bulan
            if (!TryParseSelectedMonth(out DateTime selectedMonth))
            {
                XtraMessageBox.Show("Please select a specific month.",
                    "Lock/Unlock Month", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Admin ra ang makaka-lock/unlock; ipakita ang warning kung dili Admin
            if (!CanLockDirectly)
            {
                XtraMessageBox.Show(
                    "You do not have permission to lock or unlock a month.\n" +
                    "Use 'Request Unlock' to submit a request to an Admin.",
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if already locked
            var lockRecord = _repo.GetMonthLock(selectedMonth);
            bool isLocked = lockRecord != null && lockRecord.IsLocked;

            if (isLocked)
            {
                // UNLOCK OPERATION
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
            }
            else
            {
                // LOCK OPERATION
                // Prevent locking if the month is still in progress (current or future months)
                DateTime today = DateTime.Today;
                DateTime currentMonthStart = new DateTime(today.Year, today.Month, 1);

                if (selectedMonth >= currentMonthStart)
                {
                    XtraMessageBox.Show(
                        $"You cannot lock {selectedMonth:MMMM yyyy} because the month is still in progress.\n" +
                        $"You can only lock completed months (before {currentMonthStart:MMMM yyyy}).",
                        "Lock Month Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Mangutana sa user kung gusto ba gyud niya i-lock ang bulan
                var confirm = XtraMessageBox.Show(
                    $"Lock {selectedMonth:MMMM yyyy}?\n\nNo changes can be made to this month once locked.",
                    "Confirm Lock", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                try
                {
                    // I-save ang lock sa database
                    _repo.SaveMonthLock(new MonthLock
                    {
                        MonthYear = selectedMonth,
                        IsLocked = true,
                        LockedByUserId = CurrentUserId,
                        LockedAt = DateTime.Now
                    });

                    // Ipakita ang confirmation nga nalocked na ang bulan
                    XtraMessageBox.Show(
                        $"{selectedMonth:MMMM yyyy} has been locked.",
                        "Month Locked", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // I-refresh ang estado sa Lock/Unlock button
                    RefreshLockButtonState();
                }
                catch (Exception ex)
                {
                    // Ipakita ang error kung may problema sa pag-lock
                    XtraMessageBox.Show(
                        $"Failed to lock month.\n{ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Gi-triggered kung gi-click ang Request Unlock button
        // Kun Admin: direkta mag-unlock; kung dili Admin: mag-submit ug request
        private void BtnReqUnlock_Click(object sender, EventArgs e)
        {
            // Kinahanglan may pinili nga specific nga bulan
            if (!TryParseSelectedMonth(out DateTime selectedMonth))
            {
                XtraMessageBox.Show("Please select a specific month.",
                    "Unlock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kung Admin, pwede direkta mag-unlock
            if (CanLockDirectly)
            {
                // Mangutana sa user kung gusto ba gyud niya i-unlock ang bulan
                var confirm = XtraMessageBox.Show(
                    $"Unlock {selectedMonth:MMMM yyyy}?\n\nThis will allow changes to this month again.",
                    "Confirm Unlock", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                try
                {
                    // I-save ang unlock sa database (IsLocked = false)
                    _repo.SaveMonthLock(new MonthLock
                    {
                        MonthYear = selectedMonth,
                        IsLocked = false,
                        LockedByUserId = CurrentUserId,
                        LockedAt = DateTime.Now
                    });

                    // Ipakita ang confirmation nga naopen na pag-usab ang bulan
                    XtraMessageBox.Show(
                        $"{selectedMonth:MMMM yyyy} has been unlocked.",
                        "Month Unlocked", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // I-refresh ang estado sa Lock/Unlock button
                    RefreshLockButtonState();
                }
                catch (Exception ex)
                {
                    // Ipakita ang error kung may problema sa pag-unlock
                    XtraMessageBox.Show(
                        $"Failed to unlock month.\n{ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return;
            }

            // Kung dili Admin (Auditor), mag-submit ug unlock request
            // Ang Admin ray maka-aprobas niini
            var confirmReq = XtraMessageBox.Show(
                $"Submit an unlock request for {selectedMonth:MMMM yyyy}?\n\nAn Admin will review your request.",
                "Request Unlock", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmReq != DialogResult.Yes)
                return;

            try
            {
                // I-save ang unlock request sa database
                _repo.SaveUnlockRequest(new UnlockRequest
                {
                    MonthYear = selectedMonth,
                    RequestedByUserId = CurrentUserId,
                    RequestedAt = DateTime.Now,
                    Status = "Pending"
                });

                // Ipakita ang confirmation nga nasugmit na ang request
                XtraMessageBox.Show(
                    $"Your unlock request for {selectedMonth:MMMM yyyy} has been submitted.\n" +
                    "You will be notified once an Admin reviews it.",
                    "Request Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Ipakita ang error kung may problema sa pag-submit sa request
                XtraMessageBox.Show(
                    $"Failed to submit unlock request.\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // I-update ang visibility ug text sa Lock/Unlock buttons
        // Depende sa role sa user ug kung may pinili nga bulan
        private void RefreshLockButtonState()
        {
            // Icheck kung may pinili nga specific nga bulan (dili "All Months")
            bool monthSelected =
                CmbDate.SelectedIndex > 0 &&
                !string.Equals(
                    CmbDate.Text,
                    AllMonthsLabel,
                    StringComparison.OrdinalIgnoreCase);

            // Kung walay pinili nga bulan, itago ang duha ka button ug i-enable ang grid
            if (!monthSelected)
            {
                BtnLock.Visible = false;
                BtnReqUnlock.Visible = false;
                ReviewItemGC.Enabled = true;
                return;
            }

            // Check if the selected month is locked
            bool isLocked = false;
            if (TryParseSelectedMonth(out DateTime selectedMonth))
            {
                var lockRecord = _repo.GetMonthLock(selectedMonth);
                isLocked = lockRecord != null && lockRecord.IsLocked;
            }

            // Gray out (disable) the grid to avoid clicking it if the month is locked
            ReviewItemGC.Enabled = !isLocked;

            // Kung Admin, ipakita ang Lock button lang
            // Kay siya ray makaka-lock ug unlock directly
            if (CanLockDirectly)
            {
                BtnLock.Visible = true;
                BtnLock.Text = isLocked ? "Unlock Period" : "Lock Period";

                // Itago ang Request Unlock button para sa Admin
                BtnReqUnlock.Visible = false;
            }
            // Kung Auditor o non-Admin, ipakita ang Request Unlock button lang kon locked
            else
            {
                // Itago ang Lock button para sa non-Admin
                BtnLock.Visible = false;

                BtnReqUnlock.Visible = isLocked;
                BtnReqUnlock.Text = "Request Unlock";
            }
        }

        // I-parse ang napili nga bulan gikan sa combo box
        // Mubalik ug TRUE kung malampuson ang pag-parse, FALSE kung dili
        private bool TryParseSelectedMonth(out DateTime result)
        {
            return DateTime.TryParseExact(
                CmbDate.Text,
                "MMMM yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out result);
        }

        // Gi-trigger kung gi-click ang Edit button
        // I-abri ang Receiving Custodian form para sa editing
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // I-check kung Admin ang user; kung dili, dili pwede mag-edit
            if (!IsAdmin)
            {
                XtraMessageBox.Show(
                    "Only administrators can edit approved receiving reports.",
                    "Access Denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Kung walay napili nga item, undangon na
            if (_selectedItem == null)
                return;

            // Maghimo ug ReceivingCustodianUC ug i-load ang napili nga item
            ReceivingCustodianUC receivingUC =
                new ReceivingCustodianUC();

            receivingUC.LoadEditItem(_selectedItem);

            // Iopen ang edit form sa maximized window
            Form editForm = new Form();
            editForm.Text = "Edit Received Item";
            editForm.WindowState = FormWindowState.Maximized;

            // I-dock ang user control para ma fill ang form
            receivingUC.Dock = DockStyle.Fill;
            editForm.Controls.Add(receivingUC);

            // I-show ang form ug hulaton ang user nga mosira
            editForm.ShowDialog();

            // Human masira ang edit form, i-reload ang data para makita ang mga pagbag-o
            LoadReviewItems();
        }

        // Gi-trigger kung gi-click ang Remarks button
        // Ipakita ang remarks sa napili nga item sa message box
        private void BtnRemarks_Click(object sender, EventArgs e)
        {
            // Kung walay napili nga item, undangon na
            if (_selectedItem == null)
                return;

            // Ipakita ang remarks; kung walay sulod, ipakita ang "No remarks available"
            XtraMessageBox.Show(
                string.IsNullOrWhiteSpace(_selectedItem.Remarks)
                    ? "No remarks available."
                    : _selectedItem.Remarks,
                "Item Remarks",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}