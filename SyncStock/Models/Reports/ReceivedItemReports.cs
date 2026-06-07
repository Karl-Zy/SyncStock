using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Reports
{
    public class ReceivedItemReports
    {
        public int ConfirmedItemID { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string ItemName { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal ReceivedAmount { get; set; }
        public string Remarks { get; set; }
        public string POType { get; set; }
        public string OrderMode { get; set; }
        public DateTime DateReceived { get; set; }
    }
}
