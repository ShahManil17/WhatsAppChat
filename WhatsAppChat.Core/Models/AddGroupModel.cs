using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Core.Models
{
	public class AddGroupModel
	{
		public string? GroupId { get; set; }
		public List<int>? UserId { get; set; }
	}
}
