using System.ComponentModel.DataAnnotations;

namespace WhatsAppChat.Data.DataModel
{
    public class GroupMessages
    {
        [Key]
        public int Id { get; set; }
        public string? Message { get; set; }
        //Default CurrentTimestamp
        public DateTime? SendTime { get; set; }
        public int? IsRead {  get; set; }
        public int? IsDelivered { get; set; }
        public string? FilePath { get; set; }
        public int? SenderId { get; set; }
        public virtual Users? Sender { get; set; }
        public string? GroupId { get; set; }
        public virtual Groups? Group { get; set; }

    }
}
