using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Models_Receiving_
{
    public class PendingIncomingItem
    {
        public string PONumber { get; set; }
        public string Department { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateOrdered { get; set; }
        public string Status { get; set; }
    }
}
