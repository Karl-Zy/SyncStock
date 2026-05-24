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

        // FIX: Separate IDs for Single PO and Group PO
        private int _singlePurchaseOrderId = 0;
        private int _gpoPurchaseOrderId = 0;

        public PurchaseOrderUC()
        {
            InitializeComponent();
            LoadDepartments();
            LoadGPOPurchaseOrderItems();

            ReqDepartmentCB.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            gpoReqDepartmentCB.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        }

        private void LoadDepartments()
        {
            var departments = _repo.GetAllDepartments();

            ReqDepartmentCB.Properties.Items.Clear();
            gpoReqDepartmentCB.Properties.Items.Clear();

            foreach (var dept in departments)
            {
                ReqDepartmentCB.Properties.Items.Add(dept.DepartmentName);
                gpoReqDepartmentCB.Properties.Items.Add(dept.DepartmentName);
            }

            ReqDepartmentCB.SelectedIndex = 0;
            gpoReqDepartmentCB.SelectedIndex = 0;
        }

        // ==========================================
        // SINGLE PURCHASE ORDER LOGIC
        // ==========================================
        private void AddToOrderBTN_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ItemNameTE.Text) || string.IsNullOrWhiteSpace(UnitPriceTE.Text) || QuantitySE.Value <= 0)
                {
                    XtraMessageBox.Show("Please check your item inputs.");
                    return;
                }

                if (!decimal.TryParse(UnitPriceTE.Text, out decimal unitPrice))
                {
                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                if (_singlePurchaseOrderId == 0)
                {
                    PurchaseOrders order = new PurchaseOrders
                    {
                        InvoiceNumber = InvoiceNumTE.Text,
                        PONumber = poNumberTE.Text,
                        OrderDate = purchaseDate.DateTime,
                        DepartmentID = ReqDepartmentCB.SelectedIndex + 1,
                        Status = "Pending",
                        Priority = "Normal",
                        Remarks = RemarksTE.Text,
                        AttachmentPath = ""
                    };

                    _singlePurchaseOrderId = _repo.AddPurchaseOrder(order);
                }

                int itemId = _repo.AddItem(ItemNameTE.Text.Trim());

                PurchaseOrderItem poItem = new PurchaseOrderItem
                {
                    PurchaseOrderID = _singlePurchaseOrderId,
                    ItemID = itemId,
                    Quantity = (int)QuantitySE.Value,
                    UnitPrice = unitPrice
                };

                _repo.AddPurchaseOrderItem(poItem);

                LoadSinglePurchaseOrderItems();

                ItemNameTE.Text = "";
                UnitPriceTE.Text = "";
                QuantitySE.Value = 1;

                XtraMessageBox.Show("Item added to Single PO successfully!");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void LoadSinglePurchaseOrderItems()
        {
            var items = _repo.GetItemsByPurchaseOrder(_singlePurchaseOrderId).ToList();

            ItemsInOrderGC.DataSource = null;
            ItemsInOrderGC.DataSource = items;
            ItemsInOrderGV.PopulateColumns();

            if (ItemsInOrderGV.Columns["PurchaseOrderItemID"] != null)
            {
                ItemsInOrderGV.Columns["PurchaseOrderItemID"].Visible = false;
                ItemsInOrderGV.Columns["PurchaseOrderID"].Visible = false;
                ItemsInOrderGV.Columns["ItemID"].Visible = false;
            }

            int totalItems = items.Sum(x => x.Quantity);
            decimal totalAmount = items.Sum(x => x.TotalPrice);

            TotalItemsLBL.Text = totalItems.ToString();
            TotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");
        }

        private void CalculateSinglePOTotal()
        {
            int quantity = (int)QuantitySE.Value;
            decimal.TryParse(UnitPriceTE.Text, out decimal price);
            decimal total = price * quantity;

            TotalAmountLBL.Text = "₱" + total.ToString("N2");
        }

        private void UnitPriceTE_EditValueChanged(object sender, EventArgs e) => CalculateSinglePOTotal();
        private void QuantitySE_ValueChanged(object sender, EventArgs e) => CalculateSinglePOTotal();



        private void gpoAddToOrderBtn_Click(object sender, EventArgs e)
        {

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
            decimal total = price * quantity;

            gpoTotalAmountLbl.Text = "₱" + total.ToString("N2");
        }

        private void gpoAddItemToOrderUnitPriceTextEdit_EditValueChanged(object sender, EventArgs e) => CalculateGPOTotal();
        private void gpoAddItemToOrderQuantitySpinEdit_ValueChanged(object sender, EventArgs e) => CalculateGPOTotal();


      

        private void TotalItemsLBL_Click(object sender, EventArgs e) { }

        // ==========================================
        // GROUP PURCHASE ORDER (GPO) LOGIC
        // ==========================================
        private void gpoAddToOrderBtn_Click_1(object sender, EventArgs e)
        {
            try
            {
                // GET ALL CART ITEMS
                var cartItems = _repo.GetAllCartItems().ToList();

                // VALIDATION
                if (cartItems.Count == 0)
                {
                    XtraMessageBox.Show("Cart is empty.");
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

                // LOOP CART ITEMS
                foreach (var cart in cartItems)
                {
                    // INSERT ITEM
                    int itemId = _repo.AddItem(cart.ItemName);

                    // CREATE PURCHASE ORDER ITEM
                    PurchaseOrderItem poItem = new PurchaseOrderItem
                    {
                        PurchaseOrderID = _gpoPurchaseOrderId,
                        ItemID = itemId,
                        Quantity = cart.Quantity,
                        UnitPrice = cart.UnitPrice
                    };

                    // SAVE PURCHASE ORDER ITEM
                    _repo.AddPurchaseOrderItem(poItem);
                }

                // CLEAR CART TABLE
                _repo.ClearCart();

                // RESET CURRENT GPO ID
                _gpoPurchaseOrderId = 0;

                // CLEAR CART GRID
                gpoItemsInCartGC.DataSource = null;
                gpoItemsInCartGV.Columns.Clear();

                // CLEAR ORDER GRID
                gpoItemsInOrderGC.DataSource = null;
                gpoItemsInOrderGV.Columns.Clear();

                // RELOAD CART GRID
                gpoItemsInCartGC.DataSource = null;
                gpoItemsInCartGC.DataSource = _repo.GetAllCartItems().ToList();

                // RELOAD ORDER GRID
                LoadGPOPurchaseOrderItems();

                // CLEAR WHOLE FORM
                gpoPurchaseOrderNumberTxtEdit.Text = "";
                gpoAddItemToOrderInvoiceNumberTextEdit.Text = "";
                gpoRemarksTxtEdit.Text = "";

                gpoAddItemToOrderItemNameTextEdit.Text = "";
                gpoAddItemToOrderUnitPriceTextEdit.Text = "";
                gpoAddItemToOrderQuantitySpinEdit.Value = 1;

                gpoReqDepartmentCB.SelectedIndex = 0;

                XtraMessageBox.Show("Purchase Order saved successfully!");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void AddToCartBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDATION
                if (string.IsNullOrWhiteSpace(gpoAddItemToOrderItemNameTextEdit.Text) ||
                    string.IsNullOrWhiteSpace(gpoAddItemToOrderUnitPriceTextEdit.Text) ||
                    string.IsNullOrWhiteSpace(gpoAddItemToOrderInvoiceNumberTextEdit.Text) ||
                    string.IsNullOrWhiteSpace(gpoPurchaseOrderNumberTxtEdit.Text) ||
                    gpoAddItemToOrderQuantitySpinEdit.Value <= 0)
                {
                    XtraMessageBox.Show("Please fill in all required fields.");
                    return;
                }

                // UNIT PRICE VALIDATION
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
                    OrderDate = gpoPurchaseOrderDate.DateTime
                };

                // SAVE TO DATABASE
                _repo.AddCartItem(cart);

                // RELOAD GRID
                var cartItems = _repo.GetAllCartItems().ToList();

                gpoItemsInCartGC.DataSource = null;
                gpoItemsInCartGC.DataSource = cartItems;

                gpoItemsInCartGV.PopulateColumns();

                // HIDE COLUMNS
                if (gpoItemsInCartGV.Columns["CartItemID"] != null)
                {
                    gpoItemsInCartGV.Columns["CartItemID"].Visible = false;
                }

                if (gpoItemsInCartGV.Columns["CreatedAt"] != null)
                {
                    gpoItemsInCartGV.Columns["CreatedAt"].Visible = false;
                }

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
    }
}