using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Core.Models
{
	public class UserDisplayModel
	{
		public int Id { get; set; }
		public string? ProfileImage { get; set; }
		public string? UserName { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Email { get; set; }
		public string? Country { get; set; }
		public string? Password { get; set; }
		public string? ConnectionId { get; set; }
		// Default 0
		public int? IsDeleted { get; set; }
		// Default CurrentTimeStamp
		public DateTime? CreatedAt { get; set; }
		public DateTime? LogoutTime { get; set; }
		public string? LastMessage { get; set; }
	}
}
