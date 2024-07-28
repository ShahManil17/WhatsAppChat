using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Core.Models
{
    public class RegisterModel
    {
        public IFormFile? ProfileImage { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        [StringLength(10)]
        public string? PhoneNo { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Country { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}
