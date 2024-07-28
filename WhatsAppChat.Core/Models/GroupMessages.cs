using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Core.Models
{
	public class GroupMessages
	{
		public int? Id { get; set; }
		public string? Message { get; set; }
		public int? IsRead { get; set; }
		public int? IsDelivered { get; set; }
		public int? SenderId { get; set; }
		public DateTime? SendTime { get; set; }
		public string? GroupId { get; set; }
		public string? UserName { get; set; }
	}
}
