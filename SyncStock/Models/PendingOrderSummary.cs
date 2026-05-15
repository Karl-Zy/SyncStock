using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models
{
    public class PendingOrderSummary : PurchaseOrders
    {
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }

    }
}
