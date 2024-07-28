using WhatsAppChat.Core.Models;

namespace WhatsAppChat.Core.Repositories
{
    public interface IRegisters
    {
        public Task<bool> RegisterAsync(RegisterModel model);
        public Task<bool> LoginAsync(LoginModel model);
    }
}
