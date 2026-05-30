using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using SyncStock.Database;
using SyncStock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

// namespace nga naglangkob sa purchase order user control
namespace SyncStock.Views.UserControl
{
    // klase nga mao ang purchase order user control, nag-extend sa XtraUserControl
    public partial class PurchaseOrderUC : DevExpress.XtraEditors.XtraUserControl
    {
        // gi-instansya ang repository para sa database operations
        private readonly Repository _repo = new Repository();

        // lista sa mga departamento nga gamiton sa dropdown
        private List<Departments> _departments = new List<Departments>();

        // variable para sa id sa opo purchase order
        private int _opoPurchaseOrderId = 0;

        // variable para sa id sa gpo purchase order
        private int _gpoPurchaseOrderId = 0;

        // constructor sa user control, gipatuman pag-load sa form
        public PurchaseOrderUC()
        {
            // gi-initialize ang mga components sa form
            InitializeComponent();

            // gi-load ang lista sa mga departamento
            LoadDepartments();

            // gi-load ang mga opo purchase order items gikan sa database
            LoadOPOPurchaseOrderItems();

            // gi-load ang mga gpo purchase order items gikan sa database
            LoadGPOPurchaseOrderItems();

            // gi-disable ang text editing sa opo department combobox, pwede lang mag-select
            opoReqDepartmentCB.Properties.TextEditStyle =
                TextEditStyles.DisableTextEditor;

            // gi-disable ang text editing sa gpo department combobox, pwede lang mag-select
            gpoReqDepartmentCB.Properties.TextEditStyle =
                TextEditStyles.DisableTextEditor;

            // gi-set ang opo cart grid nga dili ma-edit
            opoItemsInCartGV.OptionsBehavior.Editable = false;

            // gi-set ang opo cart grid nga read only
            opoItemsInCartGV.OptionsBehavior.ReadOnly = true;

            // gi-set ang opo order grid nga dili ma-edit
            opoItemsInOrderGV.OptionsBehavior.Editable = false;

            // gi-set ang opo order grid nga read only
            opoItemsInOrderGV.OptionsBehavior.ReadOnly = true;



            // gi-set ang gpo cart grid nga dili ma-edit
            gpoItemsInCartGV.OptionsBehavior.Editable = false;

            // gi-set ang gpo cart grid nga read only
            gpoItemsInCartGV.OptionsBehavior.ReadOnly = true;

            // gi-set ang gpo order grid nga dili ma-edit
            gpoItemsInOrderGV.OptionsBehavior.Editable = false;

            // gi-set ang gpo order grid nga read only
            gpoItemsInOrderGV.OptionsBehavior.ReadOnly = true;



            // gi-disable ang highlighted cell appearance sa opo cart grid
            opoItemsInCartGV.OptionsSelection.EnableAppearanceFocusedCell = false;

            // gi-disable ang highlighted cell appearance sa opo order grid
            opoItemsInOrderGV.OptionsSelection.EnableAppearanceFocusedCell = false;

            // gi-set ang focus rectangle style sa opo cart grid para sa tibuok row
            opoItemsInCartGV.FocusRectStyle =
                DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;

            // gi-set ang focus rectangle style sa opo order grid para sa tibuok row
            opoItemsInOrderGV.FocusRectStyle =
                DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
        }

        // method para i-load ang mga departamento gikan sa database papunta sa dropdowns
        private void LoadDepartments()
        {
            // gi-fetch ang tanan nga departamento ug gi-convert sa list
            _departments = _repo.GetAllDepartments().ToList();

            // gi-clear ang opo department combobox bago mag-populate
            opoReqDepartmentCB.Properties.Items.Clear();

            // gi-clear ang gpo department combobox bago mag-populate
            gpoReqDepartmentCB.Properties.Items.Clear();

            // gi-loop ang matag departamento ug gi-add sa duha ka dropdowns
            foreach (var dept in _departments)
            {
                // gi-add ang ngalan sa departamento sa opo dropdown
                opoReqDepartmentCB.Properties.Items.Add(dept.DepartmentName);

                // gi-add ang ngalan sa departamento sa gpo dropdown
                gpoReqDepartmentCB.Properties.Items.Add(dept.DepartmentName);
            }

            // gi-set ang default na pinili sa opo dropdown sa unang item
            opoReqDepartmentCB.SelectedIndex = 0;

            // gi-set ang default na pinili sa gpo dropdown sa unang item
            gpoReqDepartmentCB.SelectedIndex = 0;
        }

        // ==========================================
        // one purchase order (opo) logic
        // ==========================================

        // event handler pag-click sa button nga mag-save sa cart items isip opo order
        private void opoAddToOrderBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // gi-fetch ang tanan nga cart items nga may tipo nga opo
                var cartItems = _repo.GetCartItemsByType("OPO").ToList();

                // gi-check kung walay sulod ang cart, kung wala ipakita ang mensahe
                if (!cartItems.Any())
                {
                    XtraMessageBox.Show("Cart is empty. Please add items first.");
                    return;
                }

                // gi-create ang bag-ong purchase order object gamit ang mga input sa form
                PurchaseOrders order = new PurchaseOrders
                {
                    // gi-kuha ang invoice number gikan sa text field
                    InvoiceNumber = opoInvoiceNumTE.Text.Trim(),

                    // gi-kuha ang po number gikan sa text field
                    PONumber = opopoNumberTE.Text.Trim(),

                    // gi-kuha ang petsa sa order gikan sa date picker
                    OrderDate = opopurchaseDate.DateTime,

                    // gi-kuha ang department id base sa pinili nga departamento
                    DepartmentID = _departments
                        .First(x => x.DepartmentName == opoReqDepartmentCB.Text)
                        .DepartmentID,

                    // gi-set ang status sa pending
                    Status = "Pending",

                    // gi-check kung asap ang departamento, kung oo priority asap, kung dili normal
                    Priority = opoReqDepartmentCB.Text == "ASAP Department"
                        ? "ASAP Department"
                        : "Normal",

                    // gi-kuha ang remarks gikan sa text field
                    Remarks = opoRemarksTE.Text,

                    // gi-set ang attachment path nga walay sulod sa una
                    AttachmentPath = "",

                    // gi-set ang po type isip opo
                    POType = "OPO",

                    // gi-set ang order mode isip grouped
                    OrderMode = "Grouped"
                };

                // gi-save ang purchase order sa database ug gi-store ang bag-ong id
                _opoPurchaseOrderId = _repo.AddPurchaseOrder(order);

                // gi-loop ang matag cart item para i-save isip purchase order items
                foreach (var cart in cartItems.ToList())
                {
                    // gi-add ang item sa items table ug gi-kuha ang item id
                    int itemId = _repo.AddItem(cart.ItemName);

                    // gi-create ang purchase order item object
                    PurchaseOrderItem poItem = new PurchaseOrderItem
                    {
                        // gi-link kini sa purchase order id
                        PurchaseOrderID = _opoPurchaseOrderId,

                        // gi-link kini sa item id
                        ItemID = itemId,

                        // gi-kuha ang quantity gikan sa cart
                        Quantity = cart.Quantity,

                        // gi-kuha ang unit price gikan sa cart
                        UnitPrice = cart.UnitPrice
                    };

                    // gi-save ang purchase order item sa database
                    _repo.AddPurchaseOrderItem(poItem);
                }

                // gi-reload ang order grid gikan sa database
                LoadOPOPurchaseOrderItems();

                // gi-clear ang opo cart sa database
                _repo.ClearCartByType("OPO");

                // gi-clear ang data source sa cart grid
                opoItemsInCartGC.DataSource = null;

                // gi-clear ang mga column sa cart grid
                opoItemsInCartGV.Columns.Clear();

                // gi-reload ang cart grid nga empty na
                opoItemsInCartGC.DataSource = _repo.GetCartItemsByType("OPO").ToList();

                // gi-reset ang total items label sa zero
                opoTotalItemsInCartLBL.Text = "0";

                // gi-reset ang total amount label sa zero pesos
                opoTotalAmountLBL.Text = "₱0.00";

                // gi-clear ang invoice number field
                opoInvoiceNumTE.Text = "";

                // gi-clear ang po number field
                opopoNumberTE.Text = "";

                // gi-clear ang remarks field
                opoRemarksTE.Text = "";

                // gi-clear ang item name field
                opoItemNameTE.Text = "";

                // gi-clear ang unit price field
                opoUnitPriceTE.Text = "";

                // gi-reset ang quantity spinner sa 1
                opoQuantitySE.Value = 1;

                // gi-reset ang total items in cart label sa zero
                opoTotalItemsInCartLBL.Text = "0";

                // gi-set ang selected department sa unang item
                opoReqDepartmentCB.SelectedIndex = 0;

                // gi-clear ang purchase date picker
                opopurchaseDate.EditValue = null;

                // gi-clear ang add item date picker
                opoAddItemDateCB.EditValue = null;

                // gi-ipakita ang success message
                XtraMessageBox.Show("OPO saved successfully!");
            }
            catch (Exception ex)
            {
                // gi-ipakita ang error message kung adunay nahitabo nga problema
                XtraMessageBox.Show(ex.Message);
            }
        }

        // event handler pag-click sa button nga mag-add sa item sa opo cart
        private void opoAddToCartBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // gi-check kung naa ba ang po number ug invoice number, kung wala ipakita mensahe
                if (string.IsNullOrWhiteSpace(opopoNumberTE.Text) ||
                    string.IsNullOrWhiteSpace(opoInvoiceNumTE.Text))
                {
                    XtraMessageBox.Show("Please fill in PO Number and Invoice Number before adding items.");
                    return;
                }

                // gi-check kung naa ba ang item name, unit price, ug quantity, kung wala ipakita mensahe
                if (string.IsNullOrWhiteSpace(opoItemNameTE.Text) ||
                    string.IsNullOrWhiteSpace(opoUnitPriceTE.Text) ||
                    opoQuantitySE.Value <= 0)
                {
                    XtraMessageBox.Show("Please fill in all required item fields.");
                    return;
                }

                // gi-parse ang unit price, kung dili numero ipakita mensahe
                if (!decimal.TryParse(opoUnitPriceTE.Text, out decimal unitPrice))
                {
                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                // gi-create ang cart item object gamit ang mga input sa form
                CartItems cart = new CartItems
                {
                    // gi-kuha ang item name
                    ItemName = opoItemNameTE.Text.Trim(),

                    // gi-kuha ang quantity gikan sa spinner
                    Quantity = (int)opoQuantitySE.Value,

                    // gi-assign ang gi-parse nga unit price
                    UnitPrice = unitPrice,

                    // gi-kuha ang invoice number
                    InvoiceNumber = opoInvoiceNumTE.Text.Trim(),

                    // gi-kuha ang po number
                    PONumber = opopoNumberTE.Text.Trim(),

                    // gi-kuha ang order date
                    OrderDate = opopurchaseDate.DateTime,

                    // gi-set ang cart type isip opo
                    CartType = "OPO"
                };

                // gi-save ang cart item sa database
                _repo.AddCartItem(cart);

                // gi-fetch ang updated nga lista sa opo cart items
                var cartItems = _repo.GetCartItemsByType("OPO").ToList();

                // gi-clear ang data source sa cart grid
                opoItemsInCartGC.DataSource = null;

                // gi-assign ang bag-ong lista sa cart grid
                opoItemsInCartGC.DataSource = cartItems;

                // gi-populate ang mga column sa grid base sa data
                opoItemsInCartGV.PopulateColumns();

                // gi-hide ang internal columns sa cart grid gamit ang field name
                HideInternalColumns(opoItemsInCartGV);

                // gi-calculate ang total quantity sa tanan nga cart items
                int totalItems = cartItems.Sum(x => x.Quantity);

                // gi-calculate ang total amount sa tanan nga cart items
                decimal totalAmount = cartItems.Sum(x => x.TotalPrice);

                // gi-update ang total items label
                opoTotalItemsInCartLBL.Text = totalItems.ToString();

                // gi-update ang total amount label nga may peso sign
                opoTotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");

                // gi-clear ang item name field pagkahuman mag-add
                opoItemNameTE.Text = "";

                // gi-clear ang unit price field
                opoUnitPriceTE.Text = "";

                // gi-reset ang quantity spinner sa 1
                opoQuantitySE.Value = 1;

                // gi-ipakita ang success message
                XtraMessageBox.Show("Added to cart successfully!");
            }
            catch (Exception ex)
            {
                // gi-ipakita ang error message kung adunay problema
                XtraMessageBox.Show(ex.Message);
            }
        }

        // event handler pag-click sa button nga mag-add sa single item direkta isip opo order
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // gi-check kung naa ba ang po number ug invoice number
                if (string.IsNullOrWhiteSpace(opopoNumberTE.Text) ||
                    string.IsNullOrWhiteSpace(opoInvoiceNumTE.Text))
                {
                    XtraMessageBox.Show("Please fill in PO Number and Invoice Number.");
                    return;
                }

                // gi-check kung naa ba ang item name, unit price, ug quantity
                if (string.IsNullOrWhiteSpace(opoItemNameTE.Text) ||
                    string.IsNullOrWhiteSpace(opoUnitPriceTE.Text) ||
                    opoQuantitySE.Value <= 0)
                {
                    XtraMessageBox.Show("Please fill in all required item fields.");
                    return;
                }

                // gi-parse ang unit price, kung dili valid numero ipakita mensahe
                if (!decimal.TryParse(opoUnitPriceTE.Text, out decimal unitPrice))
                {
                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                // gi-create ang purchase order object para sa single item
                PurchaseOrders order = new PurchaseOrders
                {
                    // gi-kuha ang invoice number
                    InvoiceNumber = opoInvoiceNumTE.Text.Trim(),

                    // gi-kuha ang po number
                    PONumber = opopoNumberTE.Text.Trim(),

                    // gi-kuha ang order date
                    OrderDate = opopurchaseDate.DateTime,

                    // gi-kuha ang department id base sa pinili nga departamento
                    DepartmentID = _departments
                        .First(x => x.DepartmentName == opoReqDepartmentCB.Text)
                        .DepartmentID,

                    // gi-set ang status sa pending
                    Status = "Pending",

                    // gi-check kung asap ang departamento para i-set ang priority
                    Priority = opoReqDepartmentCB.Text == "ASAP Department" ? "ASAP Department" : "Normal",

                    // gi-kuha ang remarks
                    Remarks = opoRemarksTE.Text,

                    // gi-set ang attachment path nga walay sulod
                    AttachmentPath = "",

                    // gi-set ang po type isip opo
                    POType = "OPO",

                    // gi-set ang order mode isip single
                    OrderMode = "Single"
                };

                // gi-save ang purchase order sa database ug gi-kuha ang bag-ong id
                int purchaseOrderId = _repo.AddPurchaseOrder(order);

                // gi-add ang item sa items table ug gi-kuha ang item id
                int itemId = _repo.AddItem(opoItemNameTE.Text.Trim());

                // gi-create ang purchase order item object
                var poItem = new PurchaseOrderItem
                {
                    // gi-link sa purchase order
                    PurchaseOrderID = purchaseOrderId,

                    // gi-link sa item
                    ItemID = itemId,

                    // gi-kuha ang quantity gikan sa spinner
                    Quantity = (int)opoQuantitySE.Value,

                    // gi-assign ang unit price
                    UnitPrice = unitPrice
                };

                // gi-save ang purchase order item sa database
                _repo.AddPurchaseOrderItem(poItem);

                // gi-reload ang order grid gikan sa database
                LoadOPOPurchaseOrderItems();

                // gi-clear ang item name field
                opoItemNameTE.Text = "";

                // gi-clear ang unit price field
                opoUnitPriceTE.Text = "";

                // gi-reset ang quantity spinner sa 1
                opoQuantitySE.Value = 1;

                // gi-set ang department dropdown sa unang item
                opoReqDepartmentCB.SelectedIndex = 0;

                // gi-clear ang purchase date picker
                opopurchaseDate.EditValue = null;

                // gi-clear ang remarks field
                opoRemarksTE.Text = "";

                // gi-clear ang add item date picker
                opoAddItemDateCB.EditValue = null;

                // gi-clear ang invoice number field
                opoInvoiceNumTE.Text = "";

                // gi-clear ang po number field
                opopoNumberTE.Text = "";

                // gi-ipakita ang success message
                XtraMessageBox.Show("Single order added successfully!");
            }
            catch (Exception ex)
            {
                // gi-ipakita ang error message kung adunay problema
                XtraMessageBox.Show(ex.Message);
            }
        }

        // method para i-load ang tanan nga opo purchase order items gikan sa database
        private void LoadOPOPurchaseOrderItems()
        {
            // gi-fetch ang tanan nga opo items ug gi-convert sa list
            var items = _repo.GetAllOPOPurchaseOrderItems().ToList();

            // gi-clear ang data source sa order grid
            opoItemsInOrderGC.DataSource = null;

            // gi-assign ang bag-ong lista sa order grid
            opoItemsInOrderGC.DataSource = items;

            // gi-populate ang mga column base sa data
            opoItemsInOrderGV.PopulateColumns();

            // gi-rename ang po number column
            if (opoItemsInOrderGV.Columns["PONumber"] != null)
                opoItemsInOrderGV.Columns["PONumber"].Caption = "Purchase Order Number";

            // gi-rename ang order date column
            if (opoItemsInOrderGV.Columns["OrderDate"] != null)
                opoItemsInOrderGV.Columns["OrderDate"].Caption = "Purchase Order Date";

            // gi-rename ang invoice number column
            if (opoItemsInOrderGV.Columns["InvoiceNumber"] != null)
                opoItemsInOrderGV.Columns["InvoiceNumber"].Caption = "Invoice Number";

            // gi-rename ang item name column
            if (opoItemsInOrderGV.Columns["ItemName"] != null)
                opoItemsInOrderGV.Columns["ItemName"].Caption = "Item Name";

            // gi-rename ang unit price column
            if (opoItemsInOrderGV.Columns["UnitPrice"] != null)
                opoItemsInOrderGV.Columns["UnitPrice"].Caption = "Unit Price";

            // gi-rename ang total price column
            if (opoItemsInOrderGV.Columns["TotalPrice"] != null)
                opoItemsInOrderGV.Columns["TotalPrice"].Caption = "Total Price";

            // gi-rename ang remarks column
            if (opoItemsInOrderGV.Columns["Remarks"] != null)
                opoItemsInOrderGV.Columns["Remarks"].Caption = "Remarks";

            // gi-set ang unit price column format isip currency nga may 2 decimal places
            if (opoItemsInOrderGV.Columns["UnitPrice"] != null)
            {
                opoItemsInOrderGV.Columns["UnitPrice"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                opoItemsInOrderGV.Columns["UnitPrice"].DisplayFormat.FormatString = "c2";
            }

            // gi-set ang total price column format isip currency nga may 2 decimal places
            if (opoItemsInOrderGV.Columns["TotalPrice"] != null)
            {
                opoItemsInOrderGV.Columns["TotalPrice"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                opoItemsInOrderGV.Columns["TotalPrice"].DisplayFormat.FormatString = "c2";
            }

            // gi-set ang order date column format para ipakita ang buwan, adlaw, ug tuig
            if (opoItemsInOrderGV.Columns["OrderDate"] != null)
            {
                opoItemsInOrderGV.Columns["OrderDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                opoItemsInOrderGV.Columns["OrderDate"].DisplayFormat.FormatString = "MMMM dd, yyyy";
            }

            // IMPORTANTE: gi-hide ang internal columns gamit ang field name BEFORE mag-BestFit
            // para dili ma-include ang hidden columns sa auto-fit calculation
            HideInternalColumns(opoItemsInOrderGV);

            // gi-snapshot ang VisibleColumns sa usa ka list BEFORE mag-iterate
            // para malikayan ang "Collection was modified" error nga mahitabo
            // kung ang DevExpress nag-modify sa columns collection sa panahon sa enumeration
            var opoVisibleCols = opoItemsInOrderGV.VisibleColumns
                .Cast<DevExpress.XtraGrid.Columns.GridColumn>()
                .ToList();

            // gi-auto-fit ang lapad sa matag VISIBLE column lang base sa sulod
            foreach (var col in opoVisibleCols)
            {
                col.BestFit();
            }

            // gi-refresh ang grid para mawala ang empty spaces
            opoItemsInOrderGV.LayoutChanged();
            opoItemsInOrderGV.RefreshData();

            // gi-count ang total nga bilang sa items
            int totalItems = items.Count;

            // gi-calculate ang total amount sa tanan nga items
            decimal totalAmount = items.Sum(x => x.TotalPrice);

            // gi-update ang total items label
            opoTotalItemsLBL.Text = totalItems.ToString();

            // gi-update ang total amount label nga may peso sign
            opoioTotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");
        }

        // method para i-calculate ang opo total amount base sa quantity ug unit price
        private void CalculateOPOTotal()
        {
            // gi-kuha ang quantity gikan sa spinner
            int quantity = (int)opoQuantitySE.Value;

            // gi-parse ang unit price, kung dili valid zero ang gamiton
            decimal.TryParse(opoUnitPriceTE.Text, out decimal price);

            // gi-update ang total amount label gamit ang quantity times price
            opoTotalAmountLBL.Text = "₱" + (price * quantity).ToString("N2");
        }

        // event handler pag-usab sa quantity spinner, gi-recalculate ang total
        private void opoQuantitySE_ValueChanged_1(object sender, EventArgs e) => CalculateOPOTotal();

        // event handler pag-usab sa unit price field, gi-recalculate ang total
        private void opoUnitPriceTE_EditValueChanged_1(object sender, EventArgs e) => CalculateOPOTotal();

        // event handler pag-click sa delete button sa opo cart
        private void opoDeleteCartITemBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // gi-kuha ang napili nga row sa opo cart grid
                var row = opoItemsInCartGV.GetFocusedRow();

                // kung walay napili nga row, ipakita mensahe
                if (row == null)
                {
                    XtraMessageBox.Show("Please select an item to delete.");
                    return;
                }

                // gi-cast ang row isip cart item object
                var cartItem = row as CartItems;

                // kung dili cart item ang napili, undangan
                if (cartItem == null) return;

                // gi-ipakita ang confirmation dialog antes mag-delete
                var confirm = XtraMessageBox.Show(
                    $"Are you sure you want to remove \"{cartItem.ItemName}\" from the cart?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                // kung dili yes ang gi-click, undangan
                if (confirm != DialogResult.Yes) return;

                // gi-delete ang cart item sa database gamit ang id niini
                _repo.DeleteCartItem(cartItem.CartItemID);

                // gi-fetch ang updated nga lista sa opo cart items
                var cartItems = _repo.GetCartItemsByType("OPO").ToList();

                // gi-clear ang data source sa cart grid
                opoItemsInCartGC.DataSource = null;

                // gi-assign ang updated lista sa cart grid
                opoItemsInCartGC.DataSource = cartItems;

                // gi-populate ang mga column
                opoItemsInCartGV.PopulateColumns();

                // gi-hide ang internal columns sa cart grid gamit ang field name
                HideInternalColumns(opoItemsInCartGV);

                // gi-calculate ang updated total items
                int totalItems = cartItems.Sum(x => x.Quantity);

                // gi-calculate ang updated total amount
                decimal totalAmount = cartItems.Sum(x => x.TotalPrice);

                // gi-update ang total items label
                opoTotalItemsInCartLBL.Text = totalItems.ToString();

                // gi-update ang total amount label
                opoTotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");

                // gi-ipakita ang success message
                XtraMessageBox.Show("Item removed from cart.");
            }
            catch (Exception ex)
            {
                // gi-ipakita ang error message kung adunay problema
                XtraMessageBox.Show(ex.Message);
            }
        }

        // event handler pag-click sa edit button sa opo cart
        private void opoEditCartItemBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // gi-kuha ang napili nga row sa opo cart grid
                var row = opoItemsInCartGV.GetFocusedRow();

                // kung walay napili nga row, ipakita mensahe
                if (row == null)
                {
                    XtraMessageBox.Show("Please select an item to edit.");
                    return;
                }

                // gi-cast ang row isip cart item object
                var cartItem = row as CartItems;

                // kung dili cart item ang napili, undangan
                if (cartItem == null) return;

                // gi-load ang item name sa text field para ma-edit
                opoItemNameTE.Text = cartItem.ItemName;

                // gi-load ang unit price sa text field
                opoUnitPriceTE.Text = cartItem.UnitPrice.ToString("N2");

                // gi-load ang quantity sa spinner
                opoQuantitySE.Value = cartItem.Quantity;

                // gi-load ang invoice number sa text field
                opoInvoiceNumTE.Text = cartItem.InvoiceNumber;

                // gi-load ang po number sa text field
                opopoNumberTE.Text = cartItem.PONumber;

                // gi-load ang order date sa date picker
                opopurchaseDate.DateTime = cartItem.OrderDate;

                // gi-delete ang cart item sa database para ma-replace sa bag-ong version
                _repo.DeleteCartItem(cartItem.CartItemID);

                // gi-fetch ang updated nga lista sa opo cart items
                var cartItems = _repo.GetCartItemsByType("OPO").ToList();

                // gi-clear ang data source sa cart grid
                opoItemsInCartGC.DataSource = null;

                // gi-assign ang updated lista sa cart grid
                opoItemsInCartGC.DataSource = cartItems;

                // gi-populate ang mga column
                opoItemsInCartGV.PopulateColumns();

                // gi-hide ang internal columns sa cart grid gamit ang field name
                HideInternalColumns(opoItemsInCartGV);

                // gi-calculate ang updated total items
                int totalItems = cartItems.Sum(x => x.Quantity);

                // gi-calculate ang updated total amount
                decimal totalAmount = cartItems.Sum(x => x.TotalPrice);

                // gi-update ang total items label
                opoTotalItemsInCartLBL.Text = totalItems.ToString();

                // gi-update ang total amount label
                opoTotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");

                // gi-ipakita ang mensahe nga na-load na ang item para ma-edit
                XtraMessageBox.Show("Item loaded for editing. Modify the fields and click \"Add to Cart\" to save changes.");
            }
            catch (Exception ex)
            {
                // gi-ipakita ang error message kung adunay problema
                XtraMessageBox.Show(ex.Message);
            }
        }

        // ==========================================
        // group purchase order (gpo) logic
        // ==========================================

        // event handler pag-click sa button nga mag-add sa item sa gpo cart
        private void AddToCartBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // gi-check kung naa ba ang po number ug invoice number
                if (string.IsNullOrWhiteSpace(gpoPurchaseOrderNumberTxtEdit.Text) ||
                    string.IsNullOrWhiteSpace(gpoAddItemToOrderInvoiceNumberTextEdit.Text))
                {
                    XtraMessageBox.Show("Please fill in PO Number and Invoice Number before adding items.");
                    return;
                }

                // gi-check kung naa ba ang item name, unit price, ug quantity
                if (string.IsNullOrWhiteSpace(gpoAddItemToOrderItemNameTextEdit.Text) ||
                    string.IsNullOrWhiteSpace(gpoAddItemToOrderUnitPriceTextEdit.Text) ||
                    gpoAddItemToOrderQuantitySpinEdit.Value <= 0)
                {
                    XtraMessageBox.Show("Please fill in all required item fields.");
                    return;
                }

                // gi-parse ang unit price, kung dili valid numero ipakita mensahe
                if (!decimal.TryParse(gpoAddItemToOrderUnitPriceTextEdit.Text, out decimal unitPrice))
                {
                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                // gi-create ang gpo cart item object
                CartItems cart = new CartItems
                {
                    // gi-kuha ang item name
                    ItemName = gpoAddItemToOrderItemNameTextEdit.Text.Trim(),

                    // gi-kuha ang quantity
                    Quantity = (int)gpoAddItemToOrderQuantitySpinEdit.Value,

                    // gi-assign ang unit price
                    UnitPrice = unitPrice,

                    // gi-kuha ang invoice number
                    InvoiceNumber = gpoAddItemToOrderInvoiceNumberTextEdit.Text.Trim(),

                    // gi-kuha ang po number
                    PONumber = gpoPurchaseOrderNumberTxtEdit.Text.Trim(),

                    // gi-kuha ang order date
                    OrderDate = gpoPurchaseOrderDate.DateTime,

                    // gi-set ang cart type isip gpo
                    CartType = "GPO"
                };

                // gi-save ang cart item sa database
                _repo.AddCartItem(cart);

                // gi-fetch ang updated nga lista sa gpo cart items
                var cartItems = _repo.GetCartItemsByType("GPO").ToList();

                // gi-clear ang data source sa gpo cart grid
                gpoItemsInCartGC.DataSource = null;

                // gi-assign ang updated lista
                gpoItemsInCartGC.DataSource = cartItems;

                // gi-populate ang mga column
                gpoItemsInCartGV.PopulateColumns();

                // gi-hide ang internal columns sa gpo cart grid gamit ang field name
                HideInternalColumns(gpoItemsInCartGV);

                // gi-calculate ang total quantity sa gpo cart
                int totalItems = cartItems.Sum(x => x.Quantity);

                // gi-calculate ang total amount sa gpo cart
                decimal totalAmount = cartItems.Sum(x => x.TotalPrice);

                // gi-update ang gpo total items label
                gpoItemsInCartTotalItemsLBL.Text = totalItems.ToString();

                // gi-update ang gpo total amount label
                gpoItemsInCartTotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");

                // gi-clear ang item name field
                gpoAddItemToOrderItemNameTextEdit.Text = "";

                // gi-clear ang unit price field
                gpoAddItemToOrderUnitPriceTextEdit.Text = "";

                // gi-reset ang quantity spinner sa 1
                gpoAddItemToOrderQuantitySpinEdit.Value = 1;

                // gi-ipakita ang success message
                XtraMessageBox.Show("Added to cart successfully!");
            }
            catch (Exception ex)
            {
                // gi-ipakita ang error message kung adunay problema
                XtraMessageBox.Show(ex.Message);
            }
        }

        // event handler pag-click sa button nga mag-save sa gpo cart isip order
        private void gpoAddToOrderBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // gi-fetch ang tanan nga gpo cart items
                var cartItems = _repo.GetCartItemsByType("GPO").ToList();

                // kung walay sulod ang cart, ipakita mensahe
                if (!cartItems.Any())
                {
                    XtraMessageBox.Show("Cart is empty. Please add items to cart first.");
                    return;
                }

                // gi-kuha ang unang item sa cart para gamiton ang header info niini
                var first = cartItems.First();

                // gi-create ang gpo purchase order object
                PurchaseOrders order = new PurchaseOrders
                {
                    // gi-kuha ang invoice number gikan sa unang cart item
                    InvoiceNumber = first.InvoiceNumber,

                    // gi-kuha ang po number gikan sa unang cart item
                    PONumber = first.PONumber,

                    // gi-kuha ang order date gikan sa unang cart item
                    OrderDate = first.OrderDate,

                    // gi-kuha ang department id base sa pinili nga departamento
                    DepartmentID = _departments
                        .First(x => x.DepartmentName == gpoReqDepartmentCB.Text)
                        .DepartmentID,

                    // gi-set ang status sa pending gamit ang workflow status constant
                    Status = WorkflowStatus.Pending,

                    // gi-check kung asap ang departamento para i-set ang priority
                    Priority = gpoReqDepartmentCB.Text == "ASAP Department" ? "ASAP Department" : "Normal",

                    // gi-kuha ang remarks
                    Remarks = gpoRemarksTxtEdit.Text,

                    // gi-set ang attachment path nga walay sulod
                    AttachmentPath = "",

                    // gi-set ang po type isip gpo
                    POType = "GPO",

                    // gi-set ang order mode isip grouped
                    OrderMode = "Grouped"
                };

                // gi-save ang purchase order sa database ug gi-store ang bag-ong id
                _gpoPurchaseOrderId = _repo.AddPurchaseOrder(order);

                // gi-loop ang matag cart item para i-save isip purchase order items
                foreach (var cart in cartItems.ToList())
                {
                    // gi-add ang item sa items table ug gi-kuha ang item id
                    int itemId = _repo.AddItem(cart.ItemName);

                    // gi-create ang purchase order item object
                    PurchaseOrderItem poItem = new PurchaseOrderItem
                    {
                        // gi-link sa purchase order
                        PurchaseOrderID = _gpoPurchaseOrderId,

                        // gi-link sa item
                        ItemID = itemId,

                        // gi-kuha ang quantity
                        Quantity = cart.Quantity,

                        // gi-kuha ang unit price
                        UnitPrice = cart.UnitPrice
                    };

                    // gi-save ang purchase order item sa database
                    _repo.AddPurchaseOrderItem(poItem);
                }

                // gi-reload ang gpo order grid gikan sa database
                LoadGPOPurchaseOrderItems();

                // gi-clear ang gpo cart sa database
                _repo.ClearCartByType("GPO");

                // gi-clear ang gpo cart grid data source
                gpoItemsInCartGC.DataSource = null;

                // gi-clear ang mga column sa gpo cart grid
                gpoItemsInCartGV.Columns.Clear();

                // gi-reload ang gpo cart grid nga empty na
                gpoItemsInCartGC.DataSource = _repo.GetCartItemsByType("GPO").ToList();

                // gi-clear ang po number field
                gpoPurchaseOrderNumberTxtEdit.Text = "";

                // gi-clear ang invoice number field
                gpoAddItemToOrderInvoiceNumberTextEdit.Text = "";

                // gi-clear ang remarks field
                gpoRemarksTxtEdit.Text = "";

                // gi-clear ang item name field
                gpoAddItemToOrderItemNameTextEdit.Text = "";

                // gi-clear ang unit price field
                gpoAddItemToOrderUnitPriceTextEdit.Text = "";

                // gi-reset ang quantity spinner sa 1
                gpoAddItemToOrderQuantitySpinEdit.Value = 1;

                // gi-set ang department dropdown sa unang item
                gpoReqDepartmentCB.SelectedIndex = 0;

                // gi-reset ang total items label sa zero
                gpoItemsInCartTotalItemsLBL.Text = "0";

                // gi-reset ang total amount label sa zero pesos
                gpoItemsInCartTotalAmountLBL.Text = "₱0.00";

                // gi-ipakita ang success message
                XtraMessageBox.Show("GPO saved successfully!");
            }
            catch (Exception ex)
            {
                // gi-ipakita ang error message kung adunay problema
                XtraMessageBox.Show(ex.Message);
            }
        }

        // event handler pag-click sa button nga mag-add sa single item direkta isip gpo order
        private void gpoAddSingleOrderBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // gi-check kung naa ba ang po number ug invoice number
                if (string.IsNullOrWhiteSpace(gpoPurchaseOrderNumberTxtEdit.Text) ||
                    string.IsNullOrWhiteSpace(gpoAddItemToOrderInvoiceNumberTextEdit.Text))
                {
                    XtraMessageBox.Show("Please fill in PO Number and Invoice Number.");
                    return;
                }

                // gi-check kung naa ba ang item name, unit price, ug quantity
                if (string.IsNullOrWhiteSpace(gpoAddItemToOrderItemNameTextEdit.Text) ||
                    string.IsNullOrWhiteSpace(gpoAddItemToOrderUnitPriceTextEdit.Text) ||
                    gpoAddItemToOrderQuantitySpinEdit.Value <= 0)
                {
                    XtraMessageBox.Show("Please fill in all required item fields.");
                    return;
                }

                // gi-parse ang unit price, kung dili valid ipakita mensahe
                if (!decimal.TryParse(gpoAddItemToOrderUnitPriceTextEdit.Text, out decimal unitPrice))
                {
                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                // gi-create ang gpo purchase order object para sa single item
                PurchaseOrders order = new PurchaseOrders
                {
                    // gi-kuha ang invoice number
                    InvoiceNumber = gpoAddItemToOrderInvoiceNumberTextEdit.Text.Trim(),

                    // gi-kuha ang po number
                    PONumber = gpoPurchaseOrderNumberTxtEdit.Text.Trim(),

                    // gi-kuha ang order date
                    OrderDate = gpoPurchaseOrderDate.DateTime,

                    // gi-kuha ang department id
                    DepartmentID = _departments
                        .First(x => x.DepartmentName == gpoReqDepartmentCB.Text)
                        .DepartmentID,

                    // gi-set ang status sa pending
                    Status = "Pending",

                    // gi-set ang priority base sa pinili nga departamento
                    Priority = gpoReqDepartmentCB.Text == "ASAP Department" ? "ASAP Department" : "Normal",

                    // gi-kuha ang remarks
                    Remarks = gpoRemarksTxtEdit.Text,

                    // gi-set ang attachment path nga walay sulod
                    AttachmentPath = "",

                    // gi-set ang po type isip gpo
                    POType = "GPO",

                    // gi-set ang order mode isip single
                    OrderMode = "Single"
                };

                // gi-save ang purchase order sa database
                _gpoPurchaseOrderId = _repo.AddPurchaseOrder(order);

                // gi-add ang item sa items table ug gi-kuha ang item id
                int itemId = _repo.AddItem(gpoAddItemToOrderItemNameTextEdit.Text.Trim());

                // gi-create ang purchase order item object
                PurchaseOrderItem poItem = new PurchaseOrderItem
                {
                    // gi-link sa purchase order
                    PurchaseOrderID = _gpoPurchaseOrderId,

                    // gi-link sa item
                    ItemID = itemId,

                    // gi-kuha ang quantity
                    Quantity = (int)gpoAddItemToOrderQuantitySpinEdit.Value,

                    // gi-assign ang unit price
                    UnitPrice = unitPrice
                };

                // gi-save ang purchase order item sa database
                _repo.AddPurchaseOrderItem(poItem);

                // gi-reload ang gpo order grid gikan sa database
                LoadGPOPurchaseOrderItems();

                // gi-reset ang gpo purchase order id sa zero
                _gpoPurchaseOrderId = 0;

                // gi-clear ang invoice number field
                gpoAddItemToOrderInvoiceNumberTextEdit.Text = "";

                // gi-set ang department dropdown sa unang item
                gpoReqDepartmentCB.SelectedIndex = 0;

                // gi-clear ang item name field
                gpoAddItemToOrderItemNameTextEdit.Text = "";

                // gi-clear ang unit price field
                gpoAddItemToOrderUnitPriceTextEdit.Text = "";

                // gi-reset ang quantity spinner sa 1
                gpoAddItemToOrderQuantitySpinEdit.Value = 1;

                // gi-clear ang po number field
                gpoPurchaseOrderNumberTxtEdit.Text = "";

                // gi-clear ang remarks field
                gpoRemarksTxtEdit.Text = "";

                // gi-clear ang add item date picker
                gpoDateAddItemToOrder.EditValue = null;

                // gi-clear ang purchase date picker
                gpoPurchaseOrderDate.EditValue = null;

                // gi-ipakita ang success message
                XtraMessageBox.Show("Single order added successfully!");
            }
            catch (Exception ex)
            {
                // gi-ipakita ang error message kung adunay problema
                XtraMessageBox.Show(ex.Message);
            }
        }

        // method para i-load ang tanan nga gpo purchase order items gikan sa database
        private void LoadGPOPurchaseOrderItems()
        {
            // gi-fetch ang tanan nga gpo items ug gi-convert sa list
            var items = _repo.GetAllGPOPurchaseOrderItems().ToList();

            // gi-clear ang data source sa gpo order grid
            gpoItemsInOrderGC.DataSource = null;

            // gi-assign ang updated lista sa gpo order grid
            gpoItemsInOrderGC.DataSource = items;

            // gi-populate ang mga column base sa data
            gpoItemsInOrderGV.PopulateColumns();

            // gi-rename ang po number column
            if (gpoItemsInOrderGV.Columns["PONumber"] != null)
                gpoItemsInOrderGV.Columns["PONumber"].Caption = "Purchase Order Number";

            // gi-rename ang order date column
            if (gpoItemsInOrderGV.Columns["OrderDate"] != null)
                gpoItemsInOrderGV.Columns["OrderDate"].Caption = "Purchase Order Date";

            // gi-rename ang invoice number column
            if (gpoItemsInOrderGV.Columns["InvoiceNumber"] != null)
                gpoItemsInOrderGV.Columns["InvoiceNumber"].Caption = "Invoice Number";

            // gi-rename ang item name column
            if (gpoItemsInOrderGV.Columns["ItemName"] != null)
                gpoItemsInOrderGV.Columns["ItemName"].Caption = "Item Name";

            // gi-rename ang unit price column
            if (gpoItemsInOrderGV.Columns["UnitPrice"] != null)
                gpoItemsInOrderGV.Columns["UnitPrice"].Caption = "Unit Price";

            // gi-rename ang total price column
            if (gpoItemsInOrderGV.Columns["TotalPrice"] != null)
                gpoItemsInOrderGV.Columns["TotalPrice"].Caption = "Total Price";

            // gi-rename ang remarks column
            if (gpoItemsInOrderGV.Columns["Remarks"] != null)
                gpoItemsInOrderGV.Columns["Remarks"].Caption = "Remarks";

            // gi-set ang unit price column format isip currency
            if (gpoItemsInOrderGV.Columns["UnitPrice"] != null)
            {
                gpoItemsInOrderGV.Columns["UnitPrice"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gpoItemsInOrderGV.Columns["UnitPrice"].DisplayFormat.FormatString = "c2";
            }

            // gi-set ang total price column format isip currency
            if (gpoItemsInOrderGV.Columns["TotalPrice"] != null)
            {
                gpoItemsInOrderGV.Columns["TotalPrice"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gpoItemsInOrderGV.Columns["TotalPrice"].DisplayFormat.FormatString = "c2";
            }

            // gi-set ang order date column format para ipakita ang buwan, adlaw, ug tuig
            if (gpoItemsInOrderGV.Columns["OrderDate"] != null)
            {
                gpoItemsInOrderGV.Columns["OrderDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                gpoItemsInOrderGV.Columns["OrderDate"].DisplayFormat.FormatString = "MMMM dd, yyyy";
            }

            // IMPORTANTE: gi-hide ang internal columns gamit ang field name BEFORE mag-BestFit
            // para dili ma-include ang hidden columns sa auto-fit calculation
            HideInternalColumns(gpoItemsInOrderGV);

            // gi-snapshot ang VisibleColumns sa usa ka list BEFORE mag-iterate
            // para malikayan ang "Collection was modified" error nga mahitabo
            // kung ang DevExpress nag-modify sa columns collection sa panahon sa enumeration
            var gpoVisibleCols = gpoItemsInOrderGV.VisibleColumns
                .Cast<DevExpress.XtraGrid.Columns.GridColumn>()
                .ToList();

            // gi-auto-fit ang lapad sa matag VISIBLE column lang base sa sulod
            foreach (var col in gpoVisibleCols)
            {
                col.BestFit();
            }

            // gi-refresh ang layout
            gpoItemsInOrderGV.LayoutChanged();
            gpoItemsInOrderGV.RefreshData();

            // gi-count ang total nga bilang sa items
            int totalItems = items.Count;

            // gi-calculate ang total amount sa tanan nga gpo items
            decimal totalAmount = items.Sum(x => x.TotalPrice);

            // gi-update ang gpo total items label
            gpoTotalAmountTotalItems.Text = totalItems.ToString();

            // gi-update ang gpo total amount label nga may peso sign
            gpoTotalAmountLbl.Text = "₱" + totalAmount.ToString("N2");
        }

        // method para i-calculate ang gpo total amount base sa quantity ug unit price
        private void CalculateGPOTotal()
        {
            // gi-kuha ang quantity gikan sa gpo spinner
            int quantity = (int)gpoAddItemToOrderQuantitySpinEdit.Value;

            // gi-parse ang unit price, kung dili valid zero ang gamiton
            decimal.TryParse(gpoAddItemToOrderUnitPriceTextEdit.Text, out decimal price);

            // gi-update ang gpo total amount label
            gpoTotalAmountLbl.Text = "₱" + (price * quantity).ToString("N2");
        }

        // event handler pag-usab sa gpo unit price field, gi-recalculate ang total
        private void gpoAddItemToOrderUnitPriceTextEdit_EditValueChanged(object sender, EventArgs e) => CalculateGPOTotal();

        // event handler pag-usab sa gpo quantity spinner, gi-recalculate ang total
        private void gpoAddItemToOrderQuantitySpinEdit_ValueChanged(object sender, EventArgs e) => CalculateGPOTotal();

        // event handler pag-click sa delete button sa gpo cart
        private void gpoDeleteCartITemBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // gi-kuha ang napili nga row sa gpo cart grid
                var row = gpoItemsInCartGV.GetFocusedRow();

                // kung walay napili nga row, ipakita mensahe
                if (row == null)
                {
                    XtraMessageBox.Show("Please select an item to delete.");
                    return;
                }

                // gi-cast ang row isip cart item object
                var cartItem = row as CartItems;

                // kung dili cart item ang napili, undangan
                if (cartItem == null) return;

                // gi-ipakita ang confirmation dialog antes mag-delete
                var confirm = XtraMessageBox.Show(
                    $"Are you sure you want to remove \"{cartItem.ItemName}\" from the cart?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                // kung dili yes ang gi-click, undangan
                if (confirm != DialogResult.Yes) return;

                // gi-delete ang cart item sa database
                _repo.DeleteCartItem(cartItem.CartItemID);

                // gi-fetch ang updated nga lista sa gpo cart items
                var cartItems = _repo.GetCartItemsByType("GPO").ToList();

                // gi-clear ang data source sa gpo cart grid
                gpoItemsInCartGC.DataSource = null;

                // gi-assign ang updated lista
                gpoItemsInCartGC.DataSource = cartItems;

                // gi-populate ang mga column
                gpoItemsInCartGV.PopulateColumns();

                // gi-hide ang internal columns sa gpo cart grid gamit ang field name
                HideInternalColumns(gpoItemsInCartGV);

                // gi-calculate ang updated total quantity
                int totalItems = cartItems.Sum(x => x.Quantity);

                // gi-calculate ang updated total amount
                decimal totalAmount = cartItems.Sum(x => x.TotalPrice);

                // gi-update ang gpo total items label
                gpoItemsInCartTotalItemsLBL.Text = totalItems.ToString();

                // gi-update ang gpo total amount label
                gpoItemsInCartTotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");

                // gi-ipakita ang success message
                XtraMessageBox.Show("Item removed from cart.");
            }
            catch (Exception ex)
            {
                // gi-ipakita ang error message kung adunay problema
                XtraMessageBox.Show(ex.Message);
            }
        }

        // event handler pag-click sa edit button sa gpo cart
        private void gpoEditCartItemBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // gi-kuha ang napili nga row sa gpo cart grid
                var row = gpoItemsInCartGV.GetFocusedRow();

                // kung walay napili nga row, ipakita mensahe
                if (row == null)
                {
                    XtraMessageBox.Show("Please select an item to edit.");
                    return;
                }

                // gi-cast ang row isip cart item object
                var cartItem = row as CartItems;

                // kung dili cart item ang napili, undangan
                if (cartItem == null) return;

                // gi-load ang item name sa text field para ma-edit
                gpoAddItemToOrderItemNameTextEdit.Text = cartItem.ItemName;

                // gi-load ang unit price sa text field
                gpoAddItemToOrderUnitPriceTextEdit.Text = cartItem.UnitPrice.ToString("N2");

                // gi-load ang quantity sa spinner
                gpoAddItemToOrderQuantitySpinEdit.Value = cartItem.Quantity;

                // gi-load ang invoice number sa text field
                gpoAddItemToOrderInvoiceNumberTextEdit.Text = cartItem.InvoiceNumber;

                // gi-load ang po number sa text field
                gpoPurchaseOrderNumberTxtEdit.Text = cartItem.PONumber;

                // gi-load ang order date sa date picker
                gpoPurchaseOrderDate.DateTime = cartItem.OrderDate;

                // gi-delete ang cart item sa database para ma-replace sa bag-ong version
                _repo.DeleteCartItem(cartItem.CartItemID);

                // gi-fetch ang updated nga lista sa gpo cart items
                var cartItems = _repo.GetCartItemsByType("GPO").ToList();

                // gi-clear ang data source sa gpo cart grid
                gpoItemsInCartGC.DataSource = null;

                // gi-assign ang updated lista
                gpoItemsInCartGC.DataSource = cartItems;

                // gi-populate ang mga column
                gpoItemsInCartGV.PopulateColumns();

                // gi-hide ang internal columns sa gpo cart grid gamit ang field name
                HideInternalColumns(gpoItemsInCartGV);

                // gi-calculate ang updated total quantity
                int totalItems = cartItems.Sum(x => x.Quantity);

                // gi-calculate ang updated total amount
                decimal totalAmount = cartItems.Sum(x => x.TotalPrice);

                // gi-update ang gpo total items label
                gpoItemsInCartTotalItemsLBL.Text = totalItems.ToString();

                // gi-update ang gpo total amount label
                gpoItemsInCartTotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");

                // gi-ipakita ang mensahe nga na-load na ang item para ma-edit
                XtraMessageBox.Show("Item loaded for editing. Modify the fields and click \"Add to Cart\" to save changes.");
            }
            catch (Exception ex)
            {
                // gi-ipakita ang error message kung adunay problema
                XtraMessageBox.Show(ex.Message);
            }
        }

        // method para i-hide ang mga internal/id columns sa bisan unsang grid view
        // gi-check base sa field name (dili caption) para mapugngan ang DevExpress
        // auto-caption mismatch nga naghimo sa columns nga makita gihapon
        private void HideInternalColumns(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            // lista sa mga field name sa internal columns nga dili ipakita sa user
            // gi-check ang field name direkta kay ang DevExpress nag-auto-generate
            // ug nag-space insert sa captions (e.g. PurchaseOrderItemID -> "Purchase Order Item ID")
            // mao nga dili pwede gamiton ang caption para mag-check
            string[] hiddenFieldNames =
            {
                "CartItemID",
                "PurchaseOrderItemID",
                "PurchaseOrderID",
                "ItemID"
            };

            // gi-snapshot ang columns sa usa ka list BEFORE mag-iterate
            // para malikayan ang "Collection was modified; enumeration operation may not execute"
            // nga error nga mahitabo kung ang DevExpress nag-modify sa collection
            // sa panahon nga gi-loop kini (labi na pag gi-set ang Visible = false)
            var allColumns = view.Columns
                .Cast<DevExpress.XtraGrid.Columns.GridColumn>()
                .ToList();

            // gi-loop ang tanan nga columns gamit ang snapshot (dili ang live collection)
            foreach (var col in allColumns)
            {
                // gi-check kung ang field name sa column naa sa lista sa hidden columns                                                                                                                                                                                                                                                                                                                                    
                if (hiddenFieldNames.Contains(col.FieldName))
                {
                    // gi-hide ang column para dili makita sa user
                    col.Visible = false;

                    // gi-remove sa customization form para dili ma-restore sa user
                    col.OptionsColumn.ShowInCustomizationForm = false;
                }
            }

            // NOTA: wala na'y LayoutChanged/RefreshData dinhi para malikayan ang
            // "Collection was modified" error. Ang caller (Load methods) mao na
            // ang mag-refresh sa grid pagkahuman sa tanan nga column operations.
        }

        // empty event handler, walay gibuhat
        private void TotalItemsLBL_Click(object sender, EventArgs e) { }

        // empty event handler, walay gibuhat
        private void textEdit5_EditValueChanged(object sender, EventArgs e) { }
    }
}