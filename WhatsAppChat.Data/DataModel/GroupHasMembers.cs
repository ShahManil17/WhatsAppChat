using System.ComponentModel.DataAnnotations;

namespace WhatsAppChat.Data.DataModel
{
    public class GroupHasMembers
    {
        [Key]
        public int Id { get; set; }
        public string? GroupId { get; set; }
        public virtual Groups? Group { get; set; }
        public int? UserId { get; set; }
        public virtual Users? User { get; set; }
    }
}
