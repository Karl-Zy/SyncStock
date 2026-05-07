using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Item
{
    public abstract class Asset : BaseItem
    {
        public string Category { get; set; }

        public override decimal CalculateTotalPrice()
        {
            return Quantity * Price;
        }
    }
}
