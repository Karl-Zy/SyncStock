using System;

namespace SyncStock.Models
{
    public class PendingOrderSummary
    {
        public int PurchaseOrderID { get; set; }

        public string InvoiceNumber { get; set; }

        public string PONumber { get; set; }

        public int DepartmentID { get; set; }

        public string DepartmentName { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        public string POType { get; set; }

        public string OrderMode { get; set; }

        public int TotalItems { get; set; }

        public decimal TotalAmount { get; set; }
    }
}