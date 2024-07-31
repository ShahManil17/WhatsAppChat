using System.ComponentModel.DataAnnotations;

namespace WhatsAppChat.Data.DataModel
{
    public class Communication
    {
        [Key]
        public int Id { get; set; }
        public string? Message { get; set; }
        //Default CurrentTimeStamp
        public DateTime? SendTime { get; set; }
        //Default 0
        public int? IsRead { get; set; }
        //Default 0
        public int? IsDelivered { get; set; }
        public string? FilePath { get; set; }
        public string? FileType { get; set; }
        public int? SenderId { get; set; }
        public virtual Users? Sender { get; set; }
        public int? ReceiverId { get; set; }
        public virtual Users? Receiver { get; set; }
    }
}
