using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Reports
{
    public class Reconciliation
    {
        public string PONumber { get; set; }
        public string ItemName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DateReceived { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal OrderedAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal UnitPrice { get; set; }
        public byte[] AttachmentData { get; set; }
        public string AttachmentFileName { get; set; }
    }
}
