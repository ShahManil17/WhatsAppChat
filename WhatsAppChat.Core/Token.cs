using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WhatsAppChat.Data;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Core
{
    public class Token
    {
        private static IConfiguration _configuration;
        private static ApplicationDbContext _context;
        public Token(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public static string generateAccessToken(Users userData, IConfiguration _configuration, ApplicationDbContext _context)
        {
            Console.WriteLine(_configuration["Jwt:Key"]);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var clm = new List<Claim>();
            clm.Add(new Claim("UserName", userData.UserName));

            var Sectoken = new JwtSecurityToken(
              _configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
              claims: clm,
              expires: DateTime.Now.AddMinutes(5),
              signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(Sectoken).ToString();
        }

        public static string generateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
