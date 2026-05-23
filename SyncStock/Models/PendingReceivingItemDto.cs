namespace SyncStock.Models
{
    /// <summary>PO line awaiting receiving custodian confirmation.</summary>
    public class PendingReceivingItemDto
    {
        public int PurchaseOrderItemID { get; set; }
        public int PurchaseOrderID { get; set; }
        public string PONumber { get; set; }
        public string ItemName { get; set; }
        public string Department { get; set; }
        public int ExpectedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ExpectedAmount => ExpectedQuantity * UnitPrice;
    }
}
