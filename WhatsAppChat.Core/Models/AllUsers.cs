using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Core.Models
{
    public class AllUsers
    {
        public Users? CurrentUser { get; set; }
        public List<Users>? ListUsers { get; set; }
    }
}
