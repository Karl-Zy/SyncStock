using System;

namespace SyncStock.Models
{
    /// <summary>
    /// Read model for auditor review — one row per line item accepted by the receiving custodian.
    /// </summary>
    public class AuditorReviewItemDto
    {
        public int PurchaseOrderItemID { get; set; }
        public string PONumber { get; set; }
        public string ItemName { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DateReceived { get; set; }
        public string Capitalizable { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public string POType { get; set; }
        public string OrderMode { get; set; }

        public string Remarks { get; set; }

        public int ExpectedQuantity { get; set; }

        public decimal ExpectedAmount { get; set; }

        public int ReceivedQuantity { get; set; }

        public decimal ReceivedAmount { get; set; }

        public int ConfirmedItemID { get; set; }

        public string Priority { get; set; }

        public bool IsCapitalizable { get; set; }

    }
}
