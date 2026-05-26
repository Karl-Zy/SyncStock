using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models
{
    public class CartItems 
    {
        public int CartItemID { get; set; }
        public int UserID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public string InvoiceNumber { get; set; }
        public string PONumber { get; set; }
        public DateTime OrderDate { get; set; }
        [Browsable(false)]
        public DateTime CreatedAt { get; set; }
        public string CartType { get; set; }
    }
}
