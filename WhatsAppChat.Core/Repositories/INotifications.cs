using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Core.Repositories
{
    public interface INotifications
    {
        public Task<bool> SetFirebaseToken(int id, string Token);
        public Task<bool> PushNotification(string Token, string Title, string Body);
    }
}