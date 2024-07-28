using System.ComponentModel.DataAnnotations;

namespace WhatsAppChat.Data.DataModel
{
    public class RefreshTokens
    {
        [Key]
        public int Id { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpireTime { get; set; }
        public int? UserId { get; set; }
        public virtual Users? User { get; set; }
    }
}
