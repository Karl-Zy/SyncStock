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
       
        private int _purchaseOrderId = 0;
        public PurchaseOrderUC()
        {
            InitializeComponent();
            LoadDepartments();

            LoadAllItems();

            
            ReqDepartmentCB.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        }

        //private void AutoSetDate()
        //{
        //    purchaseDate.EditValue = DateTime.Now;
        //    purchaseDate.Properties.DisplayFormat.FormatString = "yyyy-MM-dd";

        //    AddItemDateCB.EditValue = DateTime.Now;
        //    AddItemDateCB.Properties.DisplayFormat.FormatString = "yyyy-MM-dd";
        //}

        //private void AutoSetPONumber()
        //{
        //    string datePart = DateTime.Now.ToString("yyyyMMdd");
        //    int count = _repo.GetPurchaseOrderCount() + 1;  
        //    string sequence = count.ToString("D3");

        //    poNumberTE.Text = $"PO-{datePart}-{sequence}";
        //}

        //private void AutoSetInvoiceNumber()
        //{
        //    var invoiceNumber = $"INV-{DateTime.Now:yyyyMMddHHss}";
        //    InvoiceNumTE.Text = invoiceNumber;
        //}

        private void LoadDepartments()
        {
            var departments = _repo.GetAllDepartments();

            ReqDepartmentCB.Properties.Items.Clear();

            foreach (var dept in departments)
            {
                ReqDepartmentCB.Properties.Items.Add(dept.DepartmentName);
            }

            ReqDepartmentCB.SelectedIndex = 0;
        }

        private void AddToOrderBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDATION
                if (string.IsNullOrWhiteSpace(ItemNameTE.Text))
                {
                    XtraMessageBox.Show("Please enter item name.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(UnitPriceTE.Text))
                {
                    XtraMessageBox.Show("Please enter unit price.");
                    return;
                }

                if (QuantitySE.Value <= 0)
                {
                    XtraMessageBox.Show("Quantity must be greater than zero.");
                    return;
                }

                decimal unitPrice;

                if (!decimal.TryParse(UnitPriceTE.Text, out unitPrice))
                {
                    XtraMessageBox.Show("Invalid unit price.");
                    return;
                }

                // CREATE PURCHASE ORDER ONLY ONCE
                if (_purchaseOrderId == 0)
                {
                    PurchaseOrders order = new PurchaseOrders
                    {
                        InvoiceNumber = InvoiceNumTE.Text,
                        PONumber = poNumberTE.Text,
                        OrderDate = purchaseDate.DateTime,
                        DepartmentID = ReqDepartmentCB.SelectedIndex + 1,
                        Status = WorkflowStatus.Pending,
                        Priority = "Normal",
                        Remarks = RemarksTE.Text,
                        AttachmentPath = ""
                    };

                    _purchaseOrderId = _repo.AddPurchaseOrder(order);
                }

                // SAVE ITEM
                int itemId = _repo.AddItem(ItemNameTE.Text.Trim());

                // SAVE PURCHASE ORDER ITEM
                PurchaseOrderItem poItem = new PurchaseOrderItem
                {
                    PurchaseOrderID = _purchaseOrderId,
                    ItemID = itemId,
                    Quantity = (int)QuantitySE.Value,
                    UnitPrice = unitPrice
                };

                _repo.AddPurchaseOrderItem(poItem);

                // RELOAD GRID FROM DATABASE
                LoadPurchaseOrderItems();

                // CLEAR FIELDS
                ItemNameTE.Text = "";
                UnitPriceTE.Text = "";
                QuantitySE.Value = 1;

                XtraMessageBox.Show("Item added successfully!");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


           
            
        }

        private void LoadPurchaseOrderItems()
        {
            var items = _repo.GetItemsByPurchaseOrder(_purchaseOrderId).ToList();

            ItemsInOrderGC.DataSource = null;
            ItemsInOrderGC.DataSource = items;

            ItemsInOrderGV.PopulateColumns();

            // HIDE IDS
            ItemsInOrderGV.Columns["PurchaseOrderItemID"].Visible = false;
            ItemsInOrderGV.Columns["PurchaseOrderID"].Visible = false;
            ItemsInOrderGV.Columns["ItemID"].Visible = false;

            // TOTAL QUANTITY
            int totalItems = items.Sum(x => x.Quantity);

            // TOTAL AMOUNT
            decimal totalAmount = items.Sum(x => x.TotalPrice);

            // DISPLAY
            TotalItemsLBL.Text = totalItems.ToString();
            TotalAmountLBL.Text = "₱" + totalAmount.ToString("N2");
        }

        private void UnitPriceTE_EditValueChanged(object sender, EventArgs e)
        {
            CalculateItemTotal();
        }

        private void QuantitySE_ValueChanged(object sender, EventArgs e)
        {
            CalculateItemTotal();
        }

        private void CalculateItemTotal()
        {
            decimal price = 0;
            int quantity = (int)QuantitySE.Value;

            decimal.TryParse(UnitPriceTE.Text, out price);

            decimal total = price * quantity;


            TotalAmountLBL.Text ="₱" + total.ToString("N2");
        }

        private void LoadAllItems()
        {
            var items = _repo.GetAllPurchaseOrderItems().ToList();

            ItemsInOrderGC.DataSource = null;
            ItemsInOrderGC.DataSource = items;

            ItemsInOrderGV.PopulateColumns();

            int totalItems = items.Sum(x => x.Quantity);

            // TOTAL AMOUNT
            decimal totalAmount = items.Sum(x => x.TotalPrice);

            // DISPLAY TOTALS
            TotalItemsLBL.Text = totalItems.ToString();

            ioTotalAmountLBL.Text = "" + totalAmount.ToString("N2");
            
        }

        private void TotalItemsLBL_Click(object sender, EventArgs e)
        {

        }

       
    }
}
