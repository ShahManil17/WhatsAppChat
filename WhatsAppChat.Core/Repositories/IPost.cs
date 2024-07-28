using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppChat.Core.Models;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Core.Repositories
{
    public interface IPost
    {
        public Task<bool> CreateGroup(CreateGroupModel model);
        public Task<bool> EditProfile(RegisterModel model);
		public Task<bool> AddToGroup(AddGroupModel model);
	}
}
