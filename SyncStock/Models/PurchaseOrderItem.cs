using System;

namespace SyncStock.Models
{
    public class PurchaseOrderItem
    {
        public int PurchaseOrderItemID { get; set; }

        public int PurchaseOrderID { get; set; }

        public int ItemID { get; set; }

        public string ItemName { get; set; }

        // =========================================
        // PURCHASE ORDER DETAILS
        // =========================================

        public string PONumber { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string Remarks { get; set; }

        public string Priority { get; set; }

        public string POType { get; set; }

        public string OrderMode { get; set; }

        public string DepartmentName { get; set; }

        // =========================================
        // ITEM DETAILS
        // =========================================

        private int _quantity;

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Quantity must be greater than 0.");

                _quantity = value;
            }
        }

        private decimal _unitPrice;

        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Unit price cannot be negative.");

                _unitPrice = value;
            }
        }

        // =========================================
        // COMPUTED TOTAL
        // =========================================

        public decimal TotalPrice
        {
            get
            {
                return Quantity * UnitPrice;
            }
        }
    }
}