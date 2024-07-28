using WhatsAppChat.Core.Models;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Core.Repositories
{
	public interface IGet
    {
        public Task<Users> getCurrentUser(int id);
        public Task<List<UserDisplayModel>> getAllUsers(int id);
        public Task<UserChat> GetUserChat(int senderId, int receiverId);
        public Task MarkAsRead(int senderId, int id);
        public Task<UnreadModel> GetCount(int senderId, int receiverId);
        public Task<List<GroupDataModel>> getGroups(int id);
        public Task<GroupChatModel> GetGroupChat(string id);
		public Task<GroupUnreadModel> GetGroupUnread(int senderId, string groupId);
        public Task<bool> DeleteFromGroupUnread(int senderId, string groupId);
        public Task<bool> AddInGroupUnreads(int senderId, string groupId);
        public Task<List<GroupMembers>?> GetGroupMembers(string? groupId);
        public Task<bool> RemoveFromGroup(int? userId, string? groupId);
	}
}
