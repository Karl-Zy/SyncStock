using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models
{
    public class ItemImages
    {
        public int ImageID { get; set; }
        public int PurchaseOrderID { get; set; }
        public string ImagePath { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
