using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Remarks { get; set; }
        public string AttachmentPath { get; set; }
        public List<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
        
    }
}
