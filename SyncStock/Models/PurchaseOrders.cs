using System;
using System.ComponentModel;

namespace SyncStock.Models
{
    public class PurchaseOrders
    {
        public int PurchaseOrderID { get; set; }

        public string InvoiceNumber { get; set; }

        public string PONumber { get; set; }

        public int DepartmentID { get; set; }

        public string DepartmentName { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        [Browsable(false)]
        public string Remarks { get; set; }

        [Browsable(false)]
        public string AttachmentPath { get; set; }

        // NEW - Payment Attachment
        [Browsable(false)]
        public string PaymentAttachmentPath { get; set; }

        [Browsable(false)]
        public string PaymentAttachmentFileName { get; set; }

        public string POType { get; set; }

        public string OrderMode { get; set; }

        public int TotalItems { get; set; }

        public decimal TotalAmount { get; set; }
    }
}