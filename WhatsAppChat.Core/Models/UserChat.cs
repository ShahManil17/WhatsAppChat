using System.ComponentModel.DataAnnotations.Schema;

namespace WhatsAppChat.Core.Models
{
	public class UserChat
    {
        public int? Id { get; set; }
        public string? ProfileImage { get; set; }
        public string? UserName { get; set; }
        public DateTime? LogoutTime { get; set; }
		[NotMapped]
        public List<Messages>? Message {  get; set; }
    }
}
