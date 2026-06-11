using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Reports
{
    public class Reconciliation
    {
        public int ConfirmedItemID { get; set; }
        public string PONumber { get; set; }
        public string ItemName { get; set; }
        public string InvoiceNumber { get; set; }
        public bool IsCapitalizable { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal OrderedAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal UnitPrice { get; set; }
        public byte[] AttachmentData { get; set; }
        public string POType { get; set; }
        public string OrderMode { get; set; }
        [Browsable(false)]
        public string AttachmentFileName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DateReceived { get; set; }
    }
}
