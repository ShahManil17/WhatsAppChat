using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Data.DataModel
{
    public class Groups
    {
        [Key]
        public string? Id { get; set; }
        public string? GroupIcon { get; set; }
        public string? GroupName { get; set; }
        //Default CurrentTimeStamp
        public DateTime? CreatedAt { get; set; }

    }
}
