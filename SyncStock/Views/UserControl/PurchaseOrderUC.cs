using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using SyncStock.Database;
using SyncStock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SyncStock.Views.UserControl
{
    public partial class PurchaseOrderUC : DevExpress.XtraEditors.XtraUserControl
    {
        private readonly Repository _repo = new Repository();
        private List<Departments> _departments = new List<Departments>();

        private int _singlePurchaseOrderId;
        private int _gpoPurchaseOrderId;

        public PurchaseOrderUC()
        {
            InitializeComponent();
            LoadDepartments();

            ReqDepartmentCB.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            gpoReqDepartmentCB.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        }

        private void LoadDepartments()
        {
            _departments = _repo.GetAllDepartments().ToList();

            ReqDepartmentCB.Properties.Items.Clear();
            gpoReqDepartmentCB.Properties.Items.Clear();

            foreach (var dept in _departments)
            {
                ReqDepartmentCB.Properties.Items.Add(dept.DepartmentName);
                gpoReqDepartmentCB.Properties.Items.Add(dept.DepartmentName);
            }

            if (_departments.Count > 0)
            {
                ReqDepartmentCB.SelectedIndex = 0;
                gpoReqDepartmentCB.SelectedIndex = 0;
            }
        }

        private int GetSelectedDepartmentId(ComboBoxEdit combo)
        {
            if (combo.SelectedIndex >= 0 && combo.SelectedIndex < _departments.Count)
                return _departments[combo.SelectedIndex].DepartmentID;

            return _departments.Count > 0 ? _departments[0].DepartmentID : 1;
        }

        private static void ConfigureOrderGrid(DevExpress.XtraGrid.Views.Grid.GridView gridView)
        {
            gridView.PopulateColumns();

            if (gridView.Columns["PurchaseOrderItemID"] != null)
                gridView.Columns["PurchaseOrderItemID"].Visible = false;
            if (gridView.Columns["PurchaseOrderID"] != null)
                gridView.Columns["PurchaseOrderID"].Visible = false;
            if (gridView.Columns["ItemID"] != null)
                gridView.Columns["ItemID"].Visible = false;
        }

        // Single purchase order
        private void AddToOrderBTN_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ItemNameTE.Text)
                    || string.IsNullOrWhiteSpace(UnitPriceTE.Text)
                    || QuantitySE.Value <= 0)
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
                    var order = new PurchaseOrders
                    {
                        InvoiceNumber = InvoiceNumTE.Text,
                        PONumber = poNumberTE.Text,
                        OrderDate = purchaseDate.DateTime,
                        DepartmentID = GetSelectedDepartmentId(ReqDepartmentCB),
                        Status = WorkflowStatus.Pending,
                        Priority = "Normal",
                        Remarks = RemarksTE.Text,
                        AttachmentPath = ""
                    };

                    _singlePurchaseOrderId = _repo.AddPurchaseOrder(order);
                }

                int itemId = _repo.AddItem(ItemNameTE.Text.Trim());

                var poItem = new PurchaseOrderItem
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

                XtraMessageBox.Show("Item added to order successfully!");
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
            ConfigureOrderGrid(ItemsInOrderGV);

            int totalItems = items.Sum(x => x.Quantity);
            decimal totalAmount = items.Sum(x => x.TotalPrice);

            TotalItemsLBL.Text = totalItems.ToString();
            TotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");
            ioTotalAmountLBL.Text = totalAmount.ToString("N2");
        }

        private void CalculateSinglePOTotal()
        {
            int quantity = (int)QuantitySE.Value;
            decimal.TryParse(UnitPriceTE.Text, out decimal price);
            TotalAmountLBL.Text = "₱" + (price * quantity).ToString("N2");
        }

        private void UnitPriceTE_EditValueChanged(object sender, EventArgs e) => CalculateSinglePOTotal();
        private void QuantitySE_ValueChanged(object sender, EventArgs e) => CalculateSinglePOTotal();

        // Group purchase order
        private void gpoAddToOrderBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(gpoAddItemToOrderItemNameTextEdit.Text)
                    || string.IsNullOrWhiteSpace(gpoAddItemToOrderUnitPriceTextEdit.Text)
                    || gpoAddItemToOrderQuantitySpinEdit.Value <= 0)
                {
                    XtraMessageBox.Show("Please check your item inputs.");
                    return;
                }

                if (!decimal.TryParse(gpoAddItemToOrderUnitPriceTextEdit.Text, out decimal unitPrice))
                {
                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                if (_gpoPurchaseOrderId == 0)
                {
                    var order = new PurchaseOrders
                    {
                        InvoiceNumber = gpoAddItemToOrderInvoiceNumberTextEdit.Text,
                        PONumber = gpoPurchaseOrderNumberTxtEdit.Text,
                        OrderDate = gpoPurchaseOrderDate.DateTime,
                        DepartmentID = GetSelectedDepartmentId(gpoReqDepartmentCB),
                        Status = WorkflowStatus.Pending,
                        Priority = "Normal",
                        Remarks = gpoRemarksTxtEdit.Text,
                        AttachmentPath = ""
                    };

                    _gpoPurchaseOrderId = _repo.AddPurchaseOrder(order);
                }

                int itemId = _repo.AddItem(gpoAddItemToOrderItemNameTextEdit.Text.Trim());

                var poItem = new PurchaseOrderItem
                {
                    PurchaseOrderID = _gpoPurchaseOrderId,
                    ItemID = itemId,
                    Quantity = (int)gpoAddItemToOrderQuantitySpinEdit.Value,
                    UnitPrice = unitPrice
                };

                _repo.AddPurchaseOrderItem(poItem);
                LoadGPOPurchaseOrderItems();

                gpoAddItemToOrderItemNameTextEdit.Text = "";
                gpoAddItemToOrderUnitPriceTextEdit.Text = "";
                gpoAddItemToOrderQuantitySpinEdit.Value = 1;

                XtraMessageBox.Show("Item added to group order successfully!");
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
            ConfigureOrderGrid(gpoItemsInOrderGV);

            decimal totalAmount = items.Sum(x => x.TotalPrice);
            gpoTotalAmountLbl.Text = "₱" + totalAmount.ToString("N2");
            gpoItemsInOrderTotalAmount.Text = "₱" + totalAmount.ToString("N2");
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
    }
}
