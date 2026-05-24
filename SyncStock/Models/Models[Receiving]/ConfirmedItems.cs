using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Models_Receiving_
{
    public class ConfirmedItems
    {
        public int ConfirmedItemID { get; set; } // SQL primary key identity
        public string PONumber { get; set; }
        public string ItemName { get; set; }
        public DateTime DateReceived { get; set; }
        public bool IsCapitalizable { get; set; }
        public int ExpectedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public string AttachmentPath { get; set; }
        public string Remarks { get; set; }
    }
}
