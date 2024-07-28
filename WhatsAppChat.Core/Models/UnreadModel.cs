using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Core.Models
{
	public class UnreadModel
	{
		public int? Count { get; set; }
		public int? SenderId { get; set; }
	}
}
