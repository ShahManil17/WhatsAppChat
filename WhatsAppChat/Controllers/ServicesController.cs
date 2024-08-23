using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using WhatsAppChat.Core.Models;
using WhatsAppChat.Core.Repositories;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Controllers
{
    [Route("[controller]")]
    public class ServicesController : Controller
    {
        private readonly IGet _get;
        private readonly IPost _post;
        private readonly IFileOperations _fileIO;
        private readonly INotifications _notifications;
        //private readonly IHubContext<Chat> _hubContext;
        //private readonly Chat _chat;
        BlobServiceClient _blobClient;
        BlobContainerClient _containerClient;
        public ServicesController(IGet get, IPost post, IFileOperations fileIO, INotifications Notifications, IConfiguration config/*, IHubContext<Chat> hubCintext, Chat chat*/)
        {
            _get = get;
            _post = post;
            _fileIO = fileIO;
            _notifications = Notifications;
            //_hubContext = hubCintext;
            //_chat = chat;
            _blobClient = new BlobServiceClient(config.GetValue<string>("AsureConnectionString:ConnectionString"));
            _containerClient = _blobClient.GetBlobContainerClient("sentfiles");
            _containerClient.CreateIfNotExistsAsync();
        }

        [HttpGet("getSingleUser")]
		public async Task<Users?> GetSingleUser(int id)
        {
            try
            {
                Users data = await _get.getCurrentUser(id);
				return data;

            }
            catch
            {
                return null;
			}
        }

		[HttpGet("getUserChat")]
        public async Task<UserChat> getUserChat(int senderId, int receiverId)
        {
            var data = await _get.GetUserChat(senderId, receiverId);
            data.ProfileImage = data.ProfileImage.Split("wwwroot/").Last().ToString();
            return data;
        }

        [HttpGet("markAsRead")]
        public async Task markAsRead(int senderId, int receiverId)
        {
            await _get.MarkAsRead(senderId, receiverId);
        }

        [HttpGet("getAllUsers")]
        public async Task<List<UserDisplayModel>> GetAllUsers(int id)
        {
            var data = await _get.getAllUsers(id);
            return data;
        }

        [HttpPost("CreateGroup")]
        public async Task<IActionResult> CreateGroup(CreateGroupModel model)
        {
            if(ModelState.IsValid)
            {
                model.GroupMembers?.Add(Convert.ToInt32(Request.Cookies["userId"]));
                await _post.CreateGroup(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("getGroupChat")]
        public async Task<GroupChatModel> GetGroupChat(string id)
        {
            var data = await _get.GetGroupChat(id);
            return data;
        }

        [HttpGet("deleteFromUnreads")]
        public async Task<bool> DeleteFromUnreads(int SenderId, string GroupId)
        {
            try
            {
                var success = await _get.DeleteFromGroupUnread(SenderId, GroupId);
                if(success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        [HttpGet("AddInGroupUnreads")]
        public async Task<bool> AddInGroupUnreads(int senderId, string groupId)
        {
            bool success = await _get.AddInGroupUnreads(senderId, groupId);
            if(success)
            {
                return true;
            }
            return false;
        }

        [HttpPost("editProfile")]
        public async Task<IActionResult> EditProfile(RegisterModel model)
        {
            var success =await _post.EditProfile(model);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("getGroupMembers")]
        public async Task<List<GroupMembers>?> GetGroupMembers(string? groupId)
        {
            var data = await _get.GetGroupMembers(groupId);
            return data;
        }

        [HttpGet("RemoveFromGroup")]
        public async Task<IActionResult> RemoveFromGroup(int? userId, string? groupId)
        {
            var success = await _get.RemoveFromGroup(userId, groupId);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("AddToGroup")]
        public async Task<IActionResult> AddToGroup (AddGroupModel model)
        {
            var success = await _post.AddToGroup(model);
			return RedirectToAction("Index", "Home");
		}

        [HttpGet("getSearchResult")]
        public async Task<List<SearchResultModel>?> GetSearchResult(string name)
        {
            List<SearchResultModel> data = await _get.GetSearchResult(name);
            return data;
        }

		[HttpPost("SendFile")]
        public async Task<ApiObjectModel?> SendFileToUser(FIleUploadModel model)
        {
            try
            {
                ApiObjectModel apiModel = new ApiObjectModel()
                {
                    Urls = new UrlResponseModel()
                    {
                        urls = await _fileIO.UploadFileAsync(model),
                        type = new List<string>()
					},
                    Upload = model
                };
                return apiModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("setFirebaseToken")]
        public async Task<bool> SetFirebaseToken(int id, string Token)
        {
            return await _notifications.SetFirebaseToken(id, Token);
        }
    }
}
