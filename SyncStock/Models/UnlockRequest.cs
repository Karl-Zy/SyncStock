using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models
{
    public class UnlockRequest
    {
        public int Id { get; set; }
        public DateTime MonthYear { get; set; }
        public string RequestedByUserId { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; }         // "Pending", "Approved", "Rejected"
    }
}
