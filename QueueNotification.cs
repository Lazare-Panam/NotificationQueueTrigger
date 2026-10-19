using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationQueueTrigger
{
    public class QueueNotification
    {
        public string email { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
}
