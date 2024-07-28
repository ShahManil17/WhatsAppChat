using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Core.Models
{
    public class GroupDataModel
    {
        public string? Id { get; set; }
        public string? GroupIcon { get; set; }
        public string? GroupName { get; set; }
        public string? LastMessage { get; set; }
    }
}
