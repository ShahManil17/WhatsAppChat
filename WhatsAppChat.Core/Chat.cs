using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using WhatsAppChat.Core.Models;
using WhatsAppChat.Data;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Core
{
	public class Chat : Hub
	{
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _contextAccessor;
		public Chat(ApplicationDbContext context, IHttpContextAccessor httpContext)
		{
			_context = context;
            _contextAccessor = httpContext;
		}
		public override Task OnConnectedAsync()
		{
            Console.WriteLine("Connection Id is : " + Context.ConnectionId);

			#region For One To One Chat
			// Update Connection Id
			Users? data = _context.Users
				.Where(x => x.Id == Convert.ToInt32(_contextAccessor.HttpContext.Request.Cookies["userId"]))
				.FirstOrDefault();
			if (data != null)
			{
				data.ConnectionId = Context.ConnectionId;
				data.LogoutTime = null;
				_context.Users.Update(data);
				_context.SaveChanges();
			}
			
			// Mark every message as delivered
			List<Communication> communicationData = _context.Communication
				.Where(x => x.ReceiverId == Convert.ToInt32(_contextAccessor.HttpContext.Request.Cookies["userId"]))
				.ToList();
			if(communicationData.Any())
			{
				foreach (var item in communicationData)
				{
					Communication? changeCommunication = _context.Communication
						.Where(x => x.Id == item.Id)
						.FirstOrDefault();
					if(changeCommunication!=null)
					{
						changeCommunication.IsDelivered = 1;
						_context.Communication.Update(changeCommunication);
						_context.SaveChanges();
					}
				}
			}
			#endregion

			var userInGroups = _context.Groups
				.Join(_context.GroupHasMembers, t1 => t1.Id, t2 => t2.GroupId, (t1, t2) => new { t1, t2 })
				.Where(t => t.t2.UserId == Convert.ToInt32(_contextAccessor.HttpContext.Request.Cookies["userId"])) 
				.Select(t => t.t1.GroupName)
				.ToList();
			if(userInGroups.Any())
			{
                foreach (var item in userInGroups)
                {
					Groups.AddToGroupAsync(Context.ConnectionId, item);
                }
			}

            return base.OnConnectedAsync();
		}

		public async Task SendMessage(string userId, string message)
		{
			// Add messages in the database
			var user = _context.Users
				.Where(x => x.Id == Convert.ToInt32(userId))
				.FirstOrDefault();
			Console.WriteLine("User Connection Id : "+user?.ConnectionId);
			if(user!= null)
			{
				var communicationObj = new Communication()
				{
					Message = message,
					SendTime = DateTime.Now, 
					IsRead = 0,
					IsDelivered = user.ConnectionId == null ? 0 : 1,
					SenderId = Convert.ToInt32(_contextAccessor.HttpContext?.Request.Cookies["userId"]),
					ReceiverId = Convert.ToInt32(userId)
				};
				_context.Communication.Add(communicationObj);
				_context.SaveChanges();
			}
			if(user.ConnectionId != null)
			{
				await Clients.Client(user.ConnectionId).SendAsync("newMessage", userId, _contextAccessor.HttpContext?.Request.Cookies["userId"],  message);
			}
		}

		public async Task SendToGroup(string groupId, string message)
		{
			int userId = Convert.ToInt32(_contextAccessor.HttpContext?.Request.Cookies["userId"]);
			// Add Message To GroupMessages
			Groups? groupData = _context.Groups
				.Where(x => x.Id == groupId)
				.FirstOrDefault();
			var groupMessage = new Data.DataModel.GroupMessages()
			{
				Message = message,
				SendTime = DateTime.Now,
				IsRead = 0,
				IsDelivered = 0,
				FilePath = null,
				SenderId = Convert.ToInt32(_contextAccessor.HttpContext?.Request.Cookies["userId"]),
				GroupId = groupId
			};
			_context.GroupMessages.Add(groupMessage);
			_context.SaveChanges();
			if (groupData != null)
			{
				var userName = _context.Users
					.Where(x => x.Id == userId)
					.FirstOrDefault();
				var userDelatils = _context.Users
					.Join(_context.GroupHasMembers, t1 => t1.Id, t2 => t2.UserId, (t1, t2) => new { t1, t2 })
					.Where(t => t.t2.GroupId == groupId)
					.ToList();
                foreach (var item in userDelatils)
                {
                    if(item.t1.ConnectionId == null)
					{
						var unreadMessages = new GroupUnreads()
						{
							MessageId = groupMessage.Id,
							UserId = item.t1.Id,
							GroupId = groupId
						};
						_context.GroupUnreads.Add(unreadMessages);
						_context.SaveChanges();
					}
                }
                await Clients.Group(groupData.GroupName).SendAsync("SendToGroup", Convert.ToInt32(_contextAccessor.HttpContext?.Request.Cookies["userId"]), userName?.UserName, groupId, message);
			}
		}

		//public async Task SendFile(FIleUploadModel model, List<string>? Urls)
		//{
		//	var user = _context.Users
		//		.Where(x => x.Id == model.ReceiverId)
		//		.FirstOrDefault();

		//	if (Urls != null && Urls.Any())
		//	{
		//		foreach (var item in Urls)
		//		{
		//			Communication communication = new Communication()
		//			{
		//				SendTime = DateTime.Now,
		//				IsRead = 0,
		//				IsDelivered = user?.ConnectionId == null ? 0 : 1,
		//				FilePath = item,
		//				SenderId = model.SenderId,
		//				ReceiverId = model.ReceiverId
		//			};
		//			await _context.Communication.AddAsync(communication);
		//		}
		//		await _context.SaveChangesAsync();

		//	}
		//	if (user?.ConnectionId != null)
		//	{
		//		await Clients.Client(user.ConnectionId).SendAsync("ReceiveFile", model.SenderId, _contextAccessor.HttpContext?.Request.Cookies["userId"], Urls);
		//	}
		//}

		//public async Task SendFileToGroup(FIleUploadModel model, List<string>? Urls)
		//{
		//	int userId = Convert.ToInt32(_contextAccessor.HttpContext?.Request.Cookies["userId"]);

		//	// Add File To GroupMessages
		//	Groups? groupData = _context.Groups
		//		.Where(x => x.Id == model.GroupId)
		//		.FirstOrDefault();
		//	var userName = _context.Users
		//					.Where(x => x.Id == userId)
		//					.FirstOrDefault();
		//	var userDelatils = _context.Users
		//		.Join(_context.GroupHasMembers, t1 => t1.Id, t2 => t2.UserId, (t1, t2) => new { t1, t2 })
		//		.Where(t => t.t2.GroupId == model.GroupId)
		//		.ToList();
		//	if (Urls != null && Urls.Any())
		//	{
  //              foreach (var item in Urls)
  //              {
		//			Data.DataModel.GroupMessages groupMessage = new Data.DataModel.GroupMessages()
		//			{
		//				SendTime = DateTime.Now,
		//				IsRead = 0,
		//				IsDelivered = 0,
		//				FilePath = item,
		//				SenderId = model.SenderId,
		//				GroupId = model.GroupId
		//			};
		//			_context.GroupMessages.Add(groupMessage);
		//			_context.SaveChanges();
		//			if (groupData != null)
		//			{
		//				foreach (var userItem in userDelatils)
		//				{
		//					if (userItem.t1.ConnectionId == null)
		//					{
		//						var unreadMessages = new GroupUnreads()
		//						{
		//							MessageId = groupMessage.Id,
		//							UserId = userItem.t1.Id,
		//							GroupId = model.GroupId
		//						};
		//						_context.GroupUnreads.Add(unreadMessages);
		//						_context.SaveChanges();
		//					}
		//				}
		//			}
		//		}
		//		await Clients.Group(groupData.GroupName).SendAsync("SendFileToGroup", Convert.ToInt32(_contextAccessor.HttpContext?.Request.Cookies["userId"]), userName?.UserName, model.GroupId, Urls);
		//	}
		//}

		public override Task OnDisconnectedAsync(Exception? exception)
		{
            Console.WriteLine("Connection Id is : " + Context.ConnectionId);

			#region For One To One
			Users? data = _context.Users
                .Where(x => x.Id == Convert.ToInt32(_contextAccessor.HttpContext.Request.Cookies["userId"]))
                .FirstOrDefault();
			if(data != null)
			{
				data.ConnectionId = null;
				data.LogoutTime = DateTime.Now;
				_context.Users.Update(data);
				_context.SaveChanges();
			}
			#endregion

			var userInGroups = _context.Groups
				.Join(_context.GroupHasMembers, t1 => t1.Id, t2 => t2.GroupId, (t1, t2) => new { t1, t2 })
				.Where(t => t.t2.UserId == Convert.ToInt32(_contextAccessor.HttpContext.Request.Cookies["userId"]))
				.Select(t => t.t1.GroupName)
				.ToList();
			if (userInGroups.Any())
			{
				foreach (var item in userInGroups)
				{
					Groups.RemoveFromGroupAsync(Context.ConnectionId, item);
				}
			}

			return base.OnDisconnectedAsync(exception);
		}
	}
}
