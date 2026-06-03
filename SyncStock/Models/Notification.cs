using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string NotificationType { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
