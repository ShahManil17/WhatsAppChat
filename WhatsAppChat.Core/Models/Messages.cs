using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Core.Models
{
    public class Messages
    {
        public int? Id { get; set; }
        public string? Message { get; set; }
        public DateTime? SendTime { get; set; }
        public int? IsRead { get; set; }
        public int? IsDelivered { get; set; }
        public string? FilePath { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
    }
}
