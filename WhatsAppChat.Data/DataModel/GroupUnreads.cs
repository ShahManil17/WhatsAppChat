using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Data.DataModel
{
	public class GroupUnreads
	{
		[Key]
		public int Id { get; set; }
		public int? MessageId { get; set; }
		public virtual GroupMessages? Message { get; set; }
		public int? UserId { get; set; }
		public virtual Users? User { get; set; }
		public string? GroupId { get; set; }
		public virtual Groups? Group {  get; set; }
	}
}
