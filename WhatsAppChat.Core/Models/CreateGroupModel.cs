using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WhatsAppChat.Core.Models
{
    public class CreateGroupModel
    {
        public IFormFile? GroupIcon {  get; set; }
        [Required]
        public string? GroupName { get; set; }
        public List<int>? GroupMembers { get; set; }
    }
}
