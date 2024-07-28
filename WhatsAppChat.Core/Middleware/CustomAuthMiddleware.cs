using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppChat.Data.DataModel;
using WhatsAppChat.Data;

namespace WhatsAppChat.Core.Middleware
{
    public class CustomAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public CustomAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
        public Task Invoke(HttpContext httpContext, ApplicationDbContext _context)
        {
            var req = httpContext.Request.Method;
            var header = httpContext.Request.Headers;
            string ogPath = httpContext.Request.Path.ToString();
            string path = httpContext.Request.Path.ToString().Split("/").Last();
            if (path != "Login" && path != "Register")
            {
                if (httpContext.Request.Cookies["Token"] != null)
                {
                    var token = httpContext.Request.Cookies["Token"];

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
                    };
                    string? userName = null;
                    try
                    {
                        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                        httpContext.Request.Headers["Authorization"] = "Bearer " + token;
                    }
                    catch
                    {
                        int userId = Convert.ToInt32(httpContext.Request.Cookies["userId"]);
                        DateTime? refreshExpiry = _context.RefreshTokens
                            .Where(x => x.UserId == userId)
                            .OrderBy(x => x.ExpireTime)
                            .Select(x => x.ExpireTime)
                            .LastOrDefault();
                        if (DateTime.Now > refreshExpiry)
                        {
                            httpContext.Response.Redirect("/Login");
                        }
                        else
                        {
                            Users? userData = _context.Users
                                .Where(x => x.Id == userId)
                                .FirstOrDefault();

                            token = Token.generateAccessToken(userData, _configuration, _context);
                            string refreshToken = Token.generateRefreshToken();

                            var tokenData = new RefreshTokens
                            {
                                UserId = userData.Id,
                                RefreshToken = refreshToken,
                                ExpireTime = DateTime.Now.AddMinutes(10)
                            };
                            _context.RefreshTokens.Update(tokenData);
                            httpContext.Request.Headers["Authorization"] = "Bearer " + token;
                        }
                    }
                }
                else
                {
                    httpContext.Response.Redirect("/Login");
                }
            }
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustomAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomAuthMiddleware>();
        }
    }
}
