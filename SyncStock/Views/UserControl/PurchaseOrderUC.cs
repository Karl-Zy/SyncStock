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
            LoadAllItems();

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


        // ==========================================
        // GROUP PURCHASE ORDER (GPO) LOGIC
        // ==========================================
        private void gpoAddToOrderBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(gpoAddItemToOrderItemNameTextEdit.Text) || string.IsNullOrWhiteSpace(gpoAddItemToOrderUnitPriceTextEdit.Text) || gpoAddItemToOrderQuantitySpinEdit.Value <= 0)
                {
                    XtraMessageBox.Show("Please check your GPO item inputs.");
                    return;
                }

                if (!decimal.TryParse(gpoAddItemToOrderUnitPriceTextEdit.Text, out decimal unitPrice))
                {
                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                if (_gpoPurchaseOrderId == 0)
                {
                    PurchaseOrders order = new PurchaseOrders
                    {
                        InvoiceNumber = gpoAddItemToOrderInvoiceNumberTextEdit.Text,
                        PONumber = gpoPurchaseOrderNumberTxtEdit.Text,
                        OrderDate = gpoPurchaseOrderDate.DateTime,
                        DepartmentID = gpoReqDepartmentCB.SelectedIndex + 1,
                        Status = "Pending",
                        Priority = "Normal",
                        Remarks = gpoRemarksTxtEdit.Text,
                        AttachmentPath = ""
                    };

                    _gpoPurchaseOrderId = _repo.AddPurchaseOrder(order);
                }

                int itemId = _repo.AddItem(gpoAddItemToOrderItemNameTextEdit.Text.Trim());

                PurchaseOrderItem poItem = new PurchaseOrderItem
                {
                    PurchaseOrderID = _gpoPurchaseOrderId,
                    ItemID = itemId,
                    Quantity = (int)gpoAddItemToOrderQuantitySpinEdit.Value,
                    UnitPrice = unitPrice
                };

                _repo.AddPurchaseOrderItem(poItem);

                // FIX: Update the GPO Grid and refresh the global view if needed
                LoadGPOPurchaseOrderItems();
                LoadAllItems();

                gpoAddItemToOrderItemNameTextEdit.Text = "";
                gpoAddItemToOrderUnitPriceTextEdit.Text = "";
                gpoAddItemToOrderQuantitySpinEdit.Value = 1;

                XtraMessageBox.Show("Item added to Group PO successfully!");
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
            decimal total = price * quantity;

            gpoTotalAmountLbl.Text = "₱" + total.ToString("N2");
        }

        private void gpoAddItemToOrderUnitPriceTextEdit_EditValueChanged(object sender, EventArgs e) => CalculateGPOTotal();
        private void gpoAddItemToOrderQuantitySpinEdit_ValueChanged(object sender, EventArgs e) => CalculateGPOTotal();


        // ==========================================
        // GLOBAL/ALL ITEMS VIEW LOGIC
        // ==========================================
        private void LoadAllItems()
        {
            // This loads ALL items in the database across every single purchase order
            var items = _repo.GetAllPurchaseOrderItems().ToList();

            ItemsInOrderGC.DataSource = null;
            ItemsInOrderGC.DataSource = items;

            gpoItemsInOrderGC.DataSource = null;
            gpoItemsInOrderGC.DataSource = items;

            ItemsInOrderGV.PopulateColumns();
            gpoItemsInOrderGV.PopulateColumns();

            int totalItems = items.Sum(x => x.Quantity);
            decimal totalAmount = items.Sum(x => x.TotalPrice);

            TotalItemsLBL.Text = totalItems.ToString();
            gpoItemsInOrderTotalAmount.Text = "₱" + totalAmount.ToString("N2");
            ioTotalAmountLBL.Text = totalAmount.ToString("N2");
        }

        private void TotalItemsLBL_Click(object sender, EventArgs e) { }
    }
}