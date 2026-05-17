using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Models_Report_
{
    public class InventorySummaryReport
    {
        public string POnumber { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateReceived { get; set; }
        public string Capitalizable { get; set; }
        public string Status { get; set; }
        public string Department { get; set; }
    }
}
