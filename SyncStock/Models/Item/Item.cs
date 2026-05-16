using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Item
{
    public class Item : BaseItem
    {
        public override string GetItemType() => "General";
    }
}
