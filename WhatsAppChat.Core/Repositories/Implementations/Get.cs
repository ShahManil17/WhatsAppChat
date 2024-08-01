using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;
using WhatsAppChat.Core.Models;
using WhatsAppChat.Data;
using WhatsAppChat.Data.DataModel;


namespace WhatsAppChat.Core.Repositories.Implementations
{
	public class Get : IGet
    {
        private readonly ApplicationDbContext _context;
        public Get(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Users> getCurrentUser(int id)
        {
            return _context.Database.SqlQuery<Users>($"exec getUsersWith {id}").ToList().FirstOrDefault();
        }

        public async Task<List<UserDisplayModel>> getAllUsers(int id)
        {
            return _context.Database.SqlQuery<UserDisplayModel>($"exec getUsersExcept {id}").ToList();

		}

        public async Task<UserChat> GetUserChat(int senderId, int receiverId)
        {
            var data = _context.Database.SqlQuery<string>($"exec getUserChat {senderId}, {receiverId}").ToList();
            var ogData = JsonSerializer.Deserialize<List<UserChat>>(data.First());

			return ogData[0];
        }

        public async Task MarkAsRead(int senderId, int id)
        {
            var communication = _context.Communication
                .Where(x => x.ReceiverId == id && x.SenderId == senderId)
                .ToList();
            foreach (var item in communication)
            {
                var change = _context.Communication
                    .Where(x => x.Id == item.Id)
                    .FirstOrDefault();
                change.IsRead = 1;
                _context.Communication.Update(change);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UnreadModel?> GetCount(int senderId, int receiverId)
        {
            UnreadModel? data = _context.Database.SqlQuery<UnreadModel>($"exec getUnreadCount {senderId}, {receiverId}").ToList().First();
			return data;
        }

        public async Task<List<GroupDataModel>> getGroups(int id)
        {
            List<GroupDataModel> data = _context.Database.SqlQuery<GroupDataModel>($"exec getGroups {id}").ToList();
            return data;
        }

        public async Task<GroupChatModel?> GetGroupChat(string id)
        {
            try
            {
                var stringData = _context.Database.SqlQuery<string>($"exec getGroupChat {id}").ToList();
                var data = JsonSerializer.Deserialize<List<GroupChatModel>>(stringData.First());
                if(data != null)
                {
                    data[0].GroupIcon = data[0].GroupIcon?.Split("wwwroot/").Last().ToString();
				    return data[0];
                }
                else
                {
                    return new GroupChatModel();
                }


			}
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<GroupUnreadModel?> GetGroupUnread(int senderId, string groupId)
        {
			GroupUnreadModel? data = _context.Database.SqlQuery<GroupUnreadModel>($"exec getGroupUnreads {senderId}, {groupId}").ToList().First();
            return data;
		}

        public async Task<bool> DeleteFromGroupUnread(int senderId, string groupId)
        {
            var data = await _context.GroupUnreads
                .Where(x => x.UserId == senderId && x.GroupId == groupId)
                .ToListAsync();
            if(data.Any())
            {
                try
                {
                    _context.RemoveRange(data);
                    _context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> AddInGroupUnreads(int senderId, string groupId)
        {
            try
            {
				var unreadMessages = new GroupUnreads()
				{
					UserId = senderId,
					GroupId = groupId
				};
				_context.GroupUnreads.Add(unreadMessages);
				_context.SaveChanges();
                return true;
			}
            catch
            {
                return false;
            }
        }

		public async Task<List<GroupMembers>?> GetGroupMembers(string? groupId)
        {
            var data = _context.Database.SqlQuery<GroupMembers>($"exec getGroupMembers {groupId}").ToList();
			return data;
        }

		public async Task<bool> RemoveFromGroup(int? userId, string? groupId)
        {
            try
            {
                var deleteData = _context.GroupHasMembers
                    .Where(x => x.UserId == userId && x.GroupId == groupId)
                    .FirstOrDefault();
                if(deleteData != null)
                {
                    _context.Remove(deleteData);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
		public async Task<List<SearchResultModel>?> GetSearchResult(string name)
        {
            try
            {
                name = $"{name}%";
				var data = _context.Database.SqlQuery<SearchResultModel>($"exec getSearchResult {name}").ToList();
				return data;
			}
            catch
            {
                return null;
            }
        }
	}
}

