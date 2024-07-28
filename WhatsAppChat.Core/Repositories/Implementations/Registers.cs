using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using WhatsAppChat.Core.Models;
using WhatsAppChat.Data;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Core.Repositories.Implementations
{
    public class Registers : IRegisters
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        public Registers(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            _context = context;
            _configuration = configuration;
            _httpContext = httpContext;
        }
        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            try
            {
                var uniqueFileName = Guid.NewGuid().ToString();
                string? ImagePath = null;
                if(model.ProfileImage == null)
                {
                    ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets/defaultImage.jpg  ");
                }
                else
                {
                    ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName + model.ProfileImage?.FileName);
                }
                using (var stream = System.IO.File.Create(ImagePath))
                {
                    if(model.ProfileImage != null)
                    {
                        await model.ProfileImage.CopyToAsync(stream);
                    }
                }
                byte[] inputBytes = Encoding.ASCII.GetBytes(model.Password);
                byte[] passwordHash = MD5.HashData(inputBytes);
                var insertData = new Users()
                {
                    ProfileImage = ImagePath,
                    UserName = model.UserName,
                    PhoneNumber = model.PhoneNo,
                    Email = model.Email,
                    Country = model.Country,
                    Password = Convert.ToHexString(passwordHash),
                    IsDeleted = 0,
                    CreatedAt = DateTime.Now
                };
                Console.WriteLine("Success");
                _context.Users.Add(insertData);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> LoginAsync(LoginModel model)
        {
            try
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(model.Password);
                byte[] passwordHash = MD5.HashData(inputBytes);
                var user = _context.Users
                    .Where(x => x.UserName == model.UserName && x.Password == Convert.ToHexString(passwordHash))
                    .FirstOrDefault();
                if (user == null)
                {
                    return false;
                }
                string token = Token.generateAccessToken(user, _configuration, _context);
                _httpContext.HttpContext.Response.Cookies.Append("Token", token);
                _httpContext.HttpContext.Response.Cookies.Append("userId", user.Id.ToString());
                string refreshToken = Token.generateRefreshToken();
                try
                {
                    var tokenData = new RefreshTokens
                    {
                        UserId = user.Id,
                        RefreshToken = refreshToken,
                        ExpireTime = DateTime.Now.AddMinutes(60)
                    };
                    _context.RefreshTokens.Add(tokenData);
                    _context.SaveChanges();
                }
                catch
                {
                    return false;
                }

                Console.WriteLine("Success");

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
