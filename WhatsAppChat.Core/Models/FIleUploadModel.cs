using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Core.Models
{
	public class FIleUploadModel
	{
		public int? SenderId { get; set; }
		public int? ReceiverId { get; set; }
		public string? GroupId { get; set; }
		public List<IFormFile>? Files { get; set; }
	}
}
