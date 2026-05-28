using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models
{
    public class PurchaseOrderBrief
    {
        public string PONumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remarks { get; set; }
        public string OrderType { get; set; }
        public string OrderMode { get; set; }

    }
}
