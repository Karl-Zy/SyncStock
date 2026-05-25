using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Reports
{
    public class ReconcilationItem
    {
        public string PONumber { get; set; }
        public string DepartmentName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Priority { get; set; }
        public string POStatus { get; set; }

        // Purchase order side
        public string ItemName { get; set; }
        public int OrderedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OrderedAmount { get; set; }

        // Received order side
        public DateTime? DateReceived { get; set; }
        public int ExpectedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public string ReceivingStatus { get; set; }
        public string Remarks { get; set; }

        // Attachment (image)
        public byte[] AttachmentData { get; set; }
        public string AttachmentFileName { get; set; }
    }
}
