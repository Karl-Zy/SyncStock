using DevExpress.XtraEditors;
using SyncStock.Database;
using SyncStock.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;

namespace SyncStock.Views.UserControl
{
    public partial class PurchaseOrderUC : DevExpress.XtraEditors.XtraUserControl
    {
        private Repository _repo = new Repository();

        private int _opoPurchaseOrderId = 0;
        private int _gpoPurchaseOrderId = 0;

        public PurchaseOrderUC()
        {
            InitializeComponent();
            LoadDepartments();
            LoadOPOPurchaseOrderItems();
            LoadGPOPurchaseOrderItems();

            opoReqDepartmentCB.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            gpoReqDepartmentCB.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        }

        private void LoadDepartments()
        {
            var departments = _repo.GetAllDepartments();

            opoReqDepartmentCB.Properties.Items.Clear();
            gpoReqDepartmentCB.Properties.Items.Clear();

            foreach (var dept in departments)
            {
                opoReqDepartmentCB.Properties.Items.Add(dept.DepartmentName);
                gpoReqDepartmentCB.Properties.Items.Add(dept.DepartmentName);
            }

            opoReqDepartmentCB.SelectedIndex = 0;
            gpoReqDepartmentCB.SelectedIndex = 0;
        }

        // ==========================================
        // ONE PURCHASE ORDER (OPO) LOGIC
        // ==========================================

        // ADD ITEM DIRECTLY TO OPO
        private void opoAddToOrderBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // GET OPO CART ITEMS
                var cartItems =
                    _repo.GetCartItemsByType("OPO").ToList();

                // VALIDATION
                if (!cartItems.Any())
                {
                    XtraMessageBox.Show(
                        "Cart is empty. Please add items first.");
                    return;
                }

                // CREATE PURCHASE ORDER
                PurchaseOrders order = new PurchaseOrders
                {
                    InvoiceNumber =
                        opoInvoiceNumTE.Text.Trim(),

                    PONumber =
                        opopoNumberTE.Text.Trim(),

                    OrderDate =
                        opopurchaseDate.DateTime,

                    DepartmentID =
                        opoReqDepartmentCB.SelectedIndex + 1,

                    Status = "Pending",
                    Priority = "Normal",

                    Remarks = opoRemarksTE.Text,

                    AttachmentPath = ""
                };

                // SAVE PURCHASE ORDER
                _opoPurchaseOrderId =
                    _repo.AddPurchaseOrder(order);

                // SAVE CART ITEMS
                foreach (var cart in cartItems)
                {
                    int itemId =
                        _repo.AddItem(cart.ItemName);

                    PurchaseOrderItem poItem =
                        new PurchaseOrderItem
                        {
                            PurchaseOrderID =
                                _opoPurchaseOrderId,

                            ItemID = itemId,

                            Quantity = cart.Quantity,

                            UnitPrice = cart.UnitPrice
                        };

                    _repo.AddPurchaseOrderItem(poItem);
                }

                // LOAD ORDER GRID
                LoadOPOPurchaseOrderItems();

                // CLEAR OPO CART
                _repo.ClearCartByType("OPO");

                // CLEAR CART GRID
                opoItemsInCartGC.DataSource = null;
                opoItemsInCartGV.Columns.Clear();

                // RELOAD EMPTY CART
                opoItemsInCartGC.DataSource =
                    _repo.GetCartItemsByType("OPO").ToList();

                // RESET TOTALS
                opoTotalItemsInCartLBL.Text = "0";
                opoTotalAmountLBL.Text = "₱0.00";

                // CLEAR FORM
                opoInvoiceNumTE.Text = "";
                opopoNumberTE.Text = "";
                opoRemarksTE.Text = "";

                opoItemNameTE.Text = "";
                opoUnitPriceTE.Text = "";
                opoQuantitySE.Value = 1;

                opoReqDepartmentCB.SelectedIndex = 0;

                opopurchaseDate.EditValue = null;
                opoAddItemDateCB.EditValue = null;

                XtraMessageBox.Show(
                    "OPO saved successfully!");

                // RESET ID
                _opoPurchaseOrderId = 0;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        // SAVE / FINALIZE OPO
        private void opoAddToCartBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDATION - HEADER
                if (string.IsNullOrWhiteSpace(opopoNumberTE.Text) ||
                    string.IsNullOrWhiteSpace(opoInvoiceNumTE.Text))
                {
                    XtraMessageBox.Show(
                        "Please fill in PO Number and Invoice Number before adding items.");
                    return;
                }

                // VALIDATION - ITEM
                if (string.IsNullOrWhiteSpace(opoItemNameTE.Text) ||
                    string.IsNullOrWhiteSpace(opoUnitPriceTE.Text) ||
                    opoQuantitySE.Value <= 0)
                {
                    XtraMessageBox.Show(
                        "Please fill in all required item fields.");
                    return;
                }

                // VALIDATE PRICE
                if (!decimal.TryParse(
                    opoUnitPriceTE.Text,
                    out decimal unitPrice))
                {

                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                // CREATE CART OBJECT
                CartItems cart = new CartItems
                {
                    ItemName = opoItemNameTE.Text.Trim(),
                    Quantity = (int)opoQuantitySE.Value,
                    UnitPrice = unitPrice,
                    InvoiceNumber = opoInvoiceNumTE.Text.Trim(),
                    PONumber = opopoNumberTE.Text.Trim(),
                    OrderDate = opopurchaseDate.DateTime,
                    CartType = "OPO"
                };

                // SAVE TO DATABASE
                _repo.AddCartItem(cart);

                // RELOAD CART GRID
                var cartItems = _repo.GetCartItemsByType("OPO").ToList();

                opoItemsInCartGC.DataSource = null;
                opoItemsInCartGC.DataSource = cartItems;

                opoItemsInCartGV.PopulateColumns();

                // HIDE IDS
                if (opoItemsInCartGV.Columns["CartItemID"] != null)
                    opoItemsInCartGV.Columns["CartItemID"].Visible = false;

                if (opoItemsInCartGV.Columns["CreatedAt"] != null)
                    opoItemsInCartGV.Columns["CreatedAt"].Visible = false;

                // TOTALS
                int totalItems = cartItems.Sum(x => x.Quantity);

                decimal totalAmount = cartItems.Sum(x => x.TotalPrice);

                opoTotalItemsInCartLBL.Text =
                    totalItems.ToString();

                opoTotalAmountLBL.Text =
                    "₱" + totalAmount.ToString("N2");

                // CLEAR ITEM INPUTS
                opoItemNameTE.Text = "";
                opoUnitPriceTE.Text = "";
                opoQuantitySE.Value = 1;

                XtraMessageBox.Show(
                    "Added to cart successfully!");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void LoadOPOPurchaseOrderItems()
        {
            var items = _repo.GetItemsByPurchaseOrder(_opoPurchaseOrderId).ToList();

            opoItemsInOrderGC.DataSource = null;
            opoItemsInOrderGC.DataSource = items;
            opoItemsInOrderGV.PopulateColumns();

            // HIDE IDS
            if (opoItemsInOrderGV.Columns["PurchaseOrderItemID"] != null)
            {
                opoItemsInOrderGV.Columns["PurchaseOrderItemID"].Visible = false;
                opoItemsInOrderGV.Columns["PurchaseOrderID"].Visible = false;
                opoItemsInOrderGV.Columns["ItemID"].Visible = false;
            }

            // TOTALS
            int totalItems = items.Sum(x => x.Quantity);
            decimal totalAmount = items.Sum(x => x.TotalPrice);

            opoTotalItemsInCartLBL.Text = totalItems.ToString();
            opoTotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");
            opoioTotalAmountLBL.Text = totalAmount.ToString("N2");
            opoTotalItemsLBL.Text = totalItems.ToString();
        }

        private void CalculateOPOTotal()
        {
            int quantity = (int)opoQuantitySE.Value;
            decimal.TryParse(opoUnitPriceTE.Text, out decimal price);
            opoTotalAmountLBL.Text = "₱" + (price * quantity).ToString("N2");
        }

        private void opoQuantitySE_ValueChanged_1(object sender, EventArgs e) => CalculateOPOTotal();
        private void opoUnitPriceTE_EditValueChanged_1(object sender, EventArgs e) => CalculateOPOTotal();


        // ==========================================
        // GROUP PURCHASE ORDER (GPO) LOGIC
        // ==========================================

        // ADD ITEM TO CART
        private void AddToCartBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDATION - HEADER FIELDS
                if (string.IsNullOrWhiteSpace(gpoPurchaseOrderNumberTxtEdit.Text) ||
                    string.IsNullOrWhiteSpace(gpoAddItemToOrderInvoiceNumberTextEdit.Text))
                {
                    XtraMessageBox.Show("Please fill in PO Number and Invoice Number before adding items.");
                    return;
                }

                // VALIDATION - ITEM FIELDS
                if (string.IsNullOrWhiteSpace(gpoAddItemToOrderItemNameTextEdit.Text) ||
                    string.IsNullOrWhiteSpace(gpoAddItemToOrderUnitPriceTextEdit.Text) ||
                    gpoAddItemToOrderQuantitySpinEdit.Value <= 0)
                {
                    XtraMessageBox.Show("Please fill in all required item fields.");
                    return;
                }

                if (!decimal.TryParse(gpoAddItemToOrderUnitPriceTextEdit.Text, out decimal unitPrice))
                {
                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                // CREATE CART OBJECT
                CartItems cart = new CartItems
                {
                    ItemName = gpoAddItemToOrderItemNameTextEdit.Text.Trim(),
                    Quantity = (int)gpoAddItemToOrderQuantitySpinEdit.Value,
                    UnitPrice = unitPrice,
                    InvoiceNumber = gpoAddItemToOrderInvoiceNumberTextEdit.Text.Trim(),
                    PONumber = gpoPurchaseOrderNumberTxtEdit.Text.Trim(),
                    OrderDate = gpoPurchaseOrderDate.DateTime,
                    CartType = "GPO"
                };

                // SAVE TO DATABASE
                _repo.AddCartItem(cart);

                // RELOAD CART GRID
                var cartItems = _repo.GetCartItemsByType("GPO").ToList();

                gpoItemsInCartGC.DataSource = null;
                gpoItemsInCartGC.DataSource = cartItems;
                gpoItemsInCartGV.PopulateColumns();

                // HIDE COLUMNS
                if (gpoItemsInCartGV.Columns["CartItemID"] != null)
                    gpoItemsInCartGV.Columns["CartItemID"].Visible = false;

                if (gpoItemsInCartGV.Columns["CreatedAt"] != null)
                    gpoItemsInCartGV.Columns["CreatedAt"].Visible = false;

                // TOTALS
                int totalItems = cartItems.Sum(x => x.Quantity);
                decimal totalAmount = cartItems.Sum(x => x.TotalPrice);

                gpoItemsInCartTotalItemsLBL.Text = totalItems.ToString();
                gpoItemsInCartTotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");

                // CLEAR ITEM INPUTS
                gpoAddItemToOrderItemNameTextEdit.Text = "";
                gpoAddItemToOrderUnitPriceTextEdit.Text = "";
                gpoAddItemToOrderQuantitySpinEdit.Value = 1;

                XtraMessageBox.Show("Added to cart successfully!");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        // SAVE CART ITEMS AS GPO
        private void gpoAddToOrderBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // GET ALL CART ITEMS
                var cartItems = _repo.GetCartItemsByType("GPO").ToList();

                // VALIDATION
                if (cartItems.Count() == 0)
                {
                    XtraMessageBox.Show("Cart is empty. Please add items to cart first.");
                    return;
                }

                // CREATE PURCHASE ORDER
                PurchaseOrders order = new PurchaseOrders
                {
                    InvoiceNumber = gpoAddItemToOrderInvoiceNumberTextEdit.Text.Trim(),
                    PONumber = gpoPurchaseOrderNumberTxtEdit.Text.Trim(),
                    OrderDate = gpoPurchaseOrderDate.DateTime,
                    DepartmentID = gpoReqDepartmentCB.SelectedIndex + 1,
                    Status = "Pending",
                    Priority = "Normal",
                    Remarks = gpoRemarksTxtEdit.Text,
                    AttachmentPath = ""
                };

                // SAVE PURCHASE ORDER
                _gpoPurchaseOrderId = _repo.AddPurchaseOrder(order);

                // LOOP CART ITEMS AND SAVE
                foreach (var cart in cartItems)
                {
                    int itemId = _repo.AddItem(cart.ItemName);

                    PurchaseOrderItem poItem = new PurchaseOrderItem
                    {
                        PurchaseOrderID = _gpoPurchaseOrderId,
                        ItemID = itemId,
                        Quantity = cart.Quantity,
                        UnitPrice = cart.UnitPrice
                    };

                    _repo.AddPurchaseOrderItem(poItem);
                }

                LoadGPOPurchaseOrderItems();

                // CLEAR CART TABLE
                _repo.ClearCartByType("GPO");



                // CLEAR GRIDS
                gpoItemsInCartGC.DataSource = null;
                gpoItemsInCartGV.Columns.Clear();

               

                // RELOAD GRIDS
                gpoItemsInCartGC.DataSource =
    _repo.GetCartItemsByType("GPO").ToList();

                // RESET CURRENT GPO ID
                _gpoPurchaseOrderId = 0;


                // CLEAR FORM
                gpoPurchaseOrderNumberTxtEdit.Text = "";
                gpoAddItemToOrderInvoiceNumberTextEdit.Text = "";
                gpoRemarksTxtEdit.Text = "";
                gpoAddItemToOrderItemNameTextEdit.Text = "";
                gpoAddItemToOrderUnitPriceTextEdit.Text = "";
                gpoAddItemToOrderQuantitySpinEdit.Value = 1;
                gpoReqDepartmentCB.SelectedIndex = 0;

                // RESET TOTALS
                gpoItemsInCartTotalItemsLBL.Text = "0";
                gpoItemsInCartTotalAmountLBL.Text = "₱0.00";
                gpoTotalAmountLbl.Text = "₱0.00";

                XtraMessageBox.Show("GPO saved successfully!");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void LoadGPOPurchaseOrderItems()
        {
            var items = _repo.GetItemsByPurchaseOrder(_gpoPurchaseOrderId).ToList();

            gpoItemsInOrderGC.DataSource = null;
            gpoItemsInOrderGC.DataSource = items;
            gpoItemsInOrderGV.PopulateColumns();

            if (gpoItemsInOrderGV.Columns["PurchaseOrderItemID"] != null)
            {
                gpoItemsInOrderGV.Columns["PurchaseOrderItemID"].Visible = false;
                gpoItemsInOrderGV.Columns["PurchaseOrderID"].Visible = false;
                gpoItemsInOrderGV.Columns["ItemID"].Visible = false;
            }

            decimal totalAmount = items.Sum(x => x.TotalPrice);
            gpoTotalAmountLbl.Text = "₱" + totalAmount.ToString("N2");
        }

        private void CalculateGPOTotal()
        {
            int quantity = (int)gpoAddItemToOrderQuantitySpinEdit.Value;
            decimal.TryParse(gpoAddItemToOrderUnitPriceTextEdit.Text, out decimal price);
            gpoTotalAmountLbl.Text = "₱" + (price * quantity).ToString("N2");
        }

        private void gpoAddItemToOrderUnitPriceTextEdit_EditValueChanged(object sender, EventArgs e) => CalculateGPOTotal();
        private void gpoAddItemToOrderQuantitySpinEdit_ValueChanged(object sender, EventArgs e) => CalculateGPOTotal();

        private void TotalItemsLBL_Click(object sender, EventArgs e) { }
        private void textEdit5_EditValueChanged(object sender, EventArgs e) { }

        private void gpoAddSingleOrderBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDATION - HEADER FIELDS
                if (string.IsNullOrWhiteSpace(gpoPurchaseOrderNumberTxtEdit.Text) ||
                    string.IsNullOrWhiteSpace(gpoAddItemToOrderInvoiceNumberTextEdit.Text))
                {
                    XtraMessageBox.Show("Please fill in PO Number and Invoice Number.");
                    return;
                }

                // VALIDATION - ITEM FIELDS
                if (string.IsNullOrWhiteSpace(gpoAddItemToOrderItemNameTextEdit.Text) ||
                    string.IsNullOrWhiteSpace(gpoAddItemToOrderUnitPriceTextEdit.Text) ||
                    gpoAddItemToOrderQuantitySpinEdit.Value <= 0)
                {
                    XtraMessageBox.Show("Please fill in all required item fields.");
                    return;
                }

                // VALIDATE PRICE
                if (!decimal.TryParse(gpoAddItemToOrderUnitPriceTextEdit.Text, out decimal unitPrice))
                {
                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                // CREATE PURCHASE ORDER
                PurchaseOrders order = new PurchaseOrders
                {
                    InvoiceNumber = gpoAddItemToOrderInvoiceNumberTextEdit.Text.Trim(),
                    PONumber = gpoPurchaseOrderNumberTxtEdit.Text.Trim(),
                    OrderDate = gpoPurchaseOrderDate.DateTime,
                    DepartmentID = gpoReqDepartmentCB.SelectedIndex + 1,
                    Status = "Pending",
                    Priority = "Normal",
                    Remarks = gpoRemarksTxtEdit.Text,
                    AttachmentPath = ""
                };

                // SAVE PURCHASE ORDER
                _gpoPurchaseOrderId = _repo.AddPurchaseOrder(order);

                // SAVE ITEM
                int itemId = _repo.AddItem(gpoAddItemToOrderItemNameTextEdit.Text.Trim());

                PurchaseOrderItem poItem = new PurchaseOrderItem
                {
                    PurchaseOrderID = _gpoPurchaseOrderId,
                    ItemID = itemId,
                    Quantity = (int)gpoAddItemToOrderQuantitySpinEdit.Value,
                    UnitPrice = unitPrice
                };

                _repo.AddPurchaseOrderItem(poItem);

                // LOAD ORDER GRID
                LoadGPOPurchaseOrderItems();

                // CLEAR ITEM INPUTS ONLY
                gpoAddItemToOrderInvoiceNumberTextEdit.Text = "";
                gpoReqDepartmentCB.SelectedIndex = 0;
                gpoAddItemToOrderItemNameTextEdit.Text = "";
                gpoAddItemToOrderUnitPriceTextEdit.Text = "";
                gpoAddItemToOrderQuantitySpinEdit.Value = 1;
                gpoPurchaseOrderNumberTxtEdit.Text = "";
                gpoRemarksTxtEdit.Text = "";
                gpoDateAddItemToOrder.EditValue = null;
                gpoPurchaseOrderDate.EditValue = null;

                XtraMessageBox.Show("Single order added successfully!");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDATION - HEADER
                if (string.IsNullOrWhiteSpace(opopoNumberTE.Text) ||
                    string.IsNullOrWhiteSpace(opoInvoiceNumTE.Text))
                {
                    XtraMessageBox.Show(
                        "Please fill in PO Number and Invoice Number.");
                    return;
                }

                // VALIDATION - ITEM
                if (string.IsNullOrWhiteSpace(opoItemNameTE.Text) ||
                    string.IsNullOrWhiteSpace(opoUnitPriceTE.Text) ||
                    opoQuantitySE.Value <= 0)
                {
                    XtraMessageBox.Show(
                        "Please fill in all required item fields.");
                    return;
                }

                // VALIDATE PRICE
                if (!decimal.TryParse(
                    opoUnitPriceTE.Text,
                    out decimal unitPrice))
                {
                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                // CREATE PURCHASE ORDER
                PurchaseOrders order = new PurchaseOrders
                {
                    InvoiceNumber = opoInvoiceNumTE.Text.Trim(),
                    PONumber = opopoNumberTE.Text.Trim(),
                    OrderDate = opopurchaseDate.DateTime,
                    DepartmentID =
                        opoReqDepartmentCB.SelectedIndex + 1,
                    Status = "Pending",
                    Priority = "Normal",
                    Remarks = opoRemarksTE.Text,
                    AttachmentPath = ""
                };

                // SAVE PURCHASE ORDER
                int purchaseOrderId =
                    _repo.AddPurchaseOrder(order);

                // SAVE ITEM
                int itemId =
                    _repo.AddItem(opoItemNameTE.Text.Trim());

                PurchaseOrderItem poItem =
                    new PurchaseOrderItem
                    {
                        PurchaseOrderID = purchaseOrderId,
                        ItemID = itemId,
                        Quantity = (int)opoQuantitySE.Value,
                        UnitPrice = unitPrice
                    };

                _repo.AddPurchaseOrderItem(poItem);

                // DISPLAY ITEM
                var items =
                    _repo.GetItemsByPurchaseOrder(
                        purchaseOrderId).ToList();

                opoItemsInOrderGC.DataSource = null;
                opoItemsInOrderGC.DataSource = items;

                opoItemsInOrderGV.PopulateColumns();

                // HIDE IDS
                if (opoItemsInOrderGV.Columns[
                    "PurchaseOrderItemID"] != null)
                {
                    opoItemsInOrderGV.Columns[
                        "PurchaseOrderItemID"].Visible = false;

                    opoItemsInOrderGV.Columns[
                        "PurchaseOrderID"].Visible = false;

                    opoItemsInOrderGV.Columns[
                        "ItemID"].Visible = false;
                }

                // CLEAR ITEM INPUTS
                opoItemNameTE.Text = "";
                opoUnitPriceTE.Text = "";
                opoQuantitySE.Value = 1;
                opoReqDepartmentCB.SelectedIndex = 0;
                opopurchaseDate.EditValue = null;
                opoRemarksTE.Text = "";
                opoAddItemDateCB.EditValue = null;
                opoInvoiceNumTE.Text = "";
                opopoNumberTE.Text = "";

                XtraMessageBox.Show(
                    "Single order added successfully!");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }
    }
}