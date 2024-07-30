using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Core.Models
{
	public class tempResponseModel
	{
		public int? senderId { get; set; }
		public int? receiverId { get; set; }
		public string? groupId { get; set; }
	}
}
