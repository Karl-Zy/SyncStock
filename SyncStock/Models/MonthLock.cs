using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models
{
    public class MonthLock
    {
        public int Id { get; set; }
        public DateTime MonthYear { get; set; }   // always the 1st of the month
        public bool IsLocked { get; set; }
        public string LockedByUserId { get; set; }
        public DateTime LockedAt { get; set; }
    }

}
