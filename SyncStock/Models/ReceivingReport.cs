using System;

namespace SyncStock.Models
{
    public class ReceivingReport
    {
        public int ReceivingReportID { get; set; }
        public int PurchaseOrderItemID { get; set; }
        public DateTime DateReceived { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal ReceivedAmount { get; set; }
        public bool IsCapitalizable { get; set; }
        public string Remarks { get; set; }
        public string ProofOfDeliveryPath { get; set; }
        public DateTime AcceptedAt { get; set; }
        public string AuditorStatus { get; set; } = WorkflowStatus.PendingAudit;
    }

    /// <summary>Shared status values across receiving and auditor review.</summary>
    public static class WorkflowStatus
    {
        public const string PendingReceipt = "Pending";
        public const string AcceptedByCustodian = "Accepted";
        public const string PendingAudit = "Pending";
        public const string Active = "Active";
    }
}
