using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppChat.Data;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Core.Repositories.Implementations
{
    public class Notifications : INotifications
    {
        private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public Notifications(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
			_webHostEnvironment = webHostEnvironment;
		}
        public async Task<bool> SetFirebaseToken(int id, string Token)
        {
            try
            {
                Users? userData = await _context.Users
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();
                if(userData == null)
                {
                    return false;
                }
                else
                {
                    userData.FirebaseToken = Token;
                    _context.Update(userData);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> PushNotification(string Token, string Title, string Body)
        {
            try
            {

                string tempPath = Path.Combine(_webHostEnvironment.WebRootPath + "\\FirebaseAuth.json");
                FirebaseApp app;
                try
                {
                    app = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(tempPath)
                    }, "WhatsApp");
                }
                catch
                {
                    app = FirebaseApp.GetInstance("WhatsApp");
                }
                try
                {
                    var fcm = FirebaseMessaging.GetMessaging(app);
					Body = Body.Length > 20 ? Body[..40] + " ..." : Body;
					Message message = new Message()
                    {
                        Notification = new Notification
                        {
                            Title = Title,
                            Body = Body
                        },
                        Token = Token
                    };
                    var msg = new List<Message>()
                    {
                        message
                    };
                    var result = await fcm.SendEachAsync(msg);
                    Console.WriteLine(result.SuccessCount);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
