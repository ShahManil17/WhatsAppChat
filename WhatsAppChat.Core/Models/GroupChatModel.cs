using System.ComponentModel.DataAnnotations.Schema;

namespace WhatsAppChat.Core.Models
{
	public class GroupChatModel
	{
		public string? Id { get; set; }
		public string? GroupIcon { get; set; }
		public string? GroupName { get; set; }
		//[NotMapped]
		public List<GroupMessages>? Message { get; set; }
		[NotMapped]
		public List<GroupMembers>? GroupMembers { get; set; }
	}
}
