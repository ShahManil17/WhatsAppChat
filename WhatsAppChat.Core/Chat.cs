using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using WhatsAppChat.Core.Models;
using WhatsAppChat.Core.Repositories;
using WhatsAppChat.Data;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Core
{
	public class Chat : Hub
	{
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly INotifications _notifications;
		public Chat(ApplicationDbContext context, IHttpContextAccessor httpContext, INotifications notifications)
		{
			_context = context;
            _contextAccessor = httpContext;
			_notifications = notifications;
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
				if(user?.ConnectionId != null)
				{
					await Clients.Client(user.ConnectionId).SendAsync("newMessage", userId, _contextAccessor.HttpContext?.Request.Cookies["userId"],  message);
					if(user.FirebaseToken != null)
					{
						await _notifications.PushNotification(user.FirebaseToken, user.UserName, message);
					}
				}
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
                foreach (var item in userDelatils)
                {
                    if (item.t1.FirebaseToken != null && item.t1.Id != userId)
                    {
						string? FinalMessage = userName?.UserName + " : " + message;

                        await _notifications.PushNotification(item.t1.FirebaseToken, groupData.GroupName, FinalMessage);
					}
                }
            }
		}

		public async Task SendFile(object model, object urls)
		{
			try
			{
				string? stringModel = model.ToString();
				string? urlString = urls.ToString();
				if (stringModel != null)
				{
					tempResponseModel? data = JsonSerializer.Deserialize<tempResponseModel>(stringModel);
					UrlResponseModel? urlModel = JsonSerializer.Deserialize<UrlResponseModel>(urlString);
					if (data != null)
					{
						var user = _context.Users
							.Where(x => x.Id == data.receiverId)
							.FirstOrDefault();

						if (urlModel != null && urlModel.urls.Any())
						{
							int counter = 0;
							foreach (var item in urlModel.urls)
							{
								Communication communication = new Communication()
								{
									SendTime = DateTime.Now,
									IsRead = 0,
									IsDelivered = user?.ConnectionId == null ? 0 : 1,
									Message = "🎞 File",
									FilePath = item,
									FileType = urlModel.type?[counter],
									SenderId = data.senderId,
									ReceiverId = data.receiverId
								};
								await _context.Communication.AddAsync(communication);
								counter++;
							}
							await _context.SaveChangesAsync();
						}
						if (user?.ConnectionId != null)
						{
							Console.WriteLine(user.ConnectionId);
							await Clients.Client(user.ConnectionId).SendAsync("ReceiveFile", data.senderId, _contextAccessor.HttpContext?.Request.Cookies["userId"], urlModel);
                            if (user.FirebaseToken != null)
                            {
                                await _notifications.PushNotification(user.FirebaseToken, user.UserName, "🎞 File");
                            }
                        }
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			
		}

		public async Task SendFileToGroup(object model, object urls)
		{
			int userId = Convert.ToInt32(_contextAccessor.HttpContext?.Request.Cookies["userId"]);

			try
			{
				string? stringModel = model.ToString();
				string? urlString = urls.ToString();
				if (stringModel != null && urlString != null)
				{
					tempResponseModel? data = JsonSerializer.Deserialize<tempResponseModel>(stringModel);
					UrlResponseModel? urlModel = JsonSerializer.Deserialize<UrlResponseModel>(urlString);
					if (data != null)
					{
						// Add File To GroupMessages
						Groups? groupData = _context.Groups
							.Where(x => x.Id == data.groupId)
							.FirstOrDefault();
						var userName = _context.Users
										.Where(x => x.Id == userId)
										.FirstOrDefault();
						var userDelatils = _context.Users
							.Join(_context.GroupHasMembers, t1 => t1.Id, t2 => t2.UserId, (t1, t2) => new { t1, t2 })
							.Where(t => t.t2.GroupId == data.groupId)
							.ToList();
						if (urlModel != null && urlModel.urls.Any())
						{
							int counter = 0;
							foreach (var item in urlModel.urls)
							{
								Data.DataModel.GroupMessages groupMessage = new Data.DataModel.GroupMessages()
								{
									SendTime = DateTime.Now,
									IsRead = 0,
									IsDelivered = 0,
									Message = "🎞 File",
									FilePath = item,
									FileType = urlModel.type?[counter],
									SenderId = data.senderId,
									GroupId = data.groupId
								};
								_context.GroupMessages.Add(groupMessage);
								_context.SaveChanges();
								if (groupData != null)
								{
									foreach (var userItem in userDelatils)
									{
										if (userItem.t1.ConnectionId == null)
										{
											var unreadMessages = new GroupUnreads()
											{
												MessageId = groupMessage.Id,
												UserId = userItem.t1.Id,
												GroupId = data.groupId
											};
											_context.GroupUnreads.Add(unreadMessages);
											_context.SaveChanges();
										}
									}
								}
								counter++;
							}
							await Clients.Group(groupData.GroupName).SendAsync("ReceiveFromGroup", Convert.ToInt32(_contextAccessor.HttpContext?.Request.Cookies["userId"]), userName?.UserName, data.groupId, urlModel);
                            foreach (var item in userDelatils)
                            {
                                if (item.t1.FirebaseToken != null && item.t1.Id != userId)
                                {
                                    string? FinalMessage = userName?.UserName + " : " + "🎞 File";

                                    await _notifications.PushNotification(item.t1.FirebaseToken, groupData.GroupName, FinalMessage);
                                }
                            }
                        }
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

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
				data.FirebaseToken = null;
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
