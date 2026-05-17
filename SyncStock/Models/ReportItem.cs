using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models
{
    public  class ReportItem : PurchaseOrders
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal Amount {  get; set; }
    }
}
