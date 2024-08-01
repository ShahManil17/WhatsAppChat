using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Core.Models
{
	public class SearchResultModel
	{
		public int? UserId { get; set; }
		public string? ProfileImage {  get; set; }
		public string? Name { get; set; }
		//public string? GroupId { get; set; }
	}
}
