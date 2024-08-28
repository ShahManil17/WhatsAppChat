using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using WhatsAppChat.Core.Models;
using WhatsAppChat.Data;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Core.Repositories.Implementations
{
    public class Post : IPost
    {
		private readonly ApplicationDbContext _context; 
		private readonly IHttpContextAccessor _contextAccessor;
		public Post(ApplicationDbContext context, IHttpContextAccessor httpContext)
		{
			_context = context;
			_contextAccessor = httpContext;
		}
		public async Task<bool> CreateGroup(CreateGroupModel model)
        {
            try
            {
                #region In Groups
                var uniqueFileName = Guid.NewGuid().ToString();
                string? ImagePath = null;
                if (model.GroupIcon == null)
                {
                    ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets/defaultGroupIcon.png");
                }
                else
                {
                    ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName + model.GroupIcon.FileName);
                }
                using (var stream = File.Create(ImagePath))
                {
                    if (model.GroupIcon != null)
                    {
                        await model.GroupIcon.CopyToAsync(stream);
                    }
                }
                Groups? newGroup = new Groups()
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupIcon = ImagePath,
                    GroupName = model.GroupName,
                    CreatedAt = DateTime.Now
                };
                await _context.Groups.AddAsync(newGroup);
                await _context.SaveChangesAsync();
                #endregion

                #region In Group Has Members
                if (model.GroupMembers != null)
                {
                    foreach (var item in model.GroupMembers)
                    {
                        GroupHasMembers? groupHasMembers = new GroupHasMembers()
                        {
                            GroupId = newGroup.Id,
                            UserId = item
                        };
                        await _context.GroupHasMembers.AddAsync(groupHasMembers);
                    }
                    await _context.SaveChangesAsync();
                }
                #endregion


                return true;
            }
            catch
            {
                return false;
            }
        }

		public async Task<bool> EditProfile(RegisterModel model)
		{
			try
			{
                Users? userData = _context.Users
                    .Where(x => x.Id == Convert.ToInt32(_contextAccessor.HttpContext.Request.Cookies["userId"]))
                    .FirstOrDefault();
                if(userData != null)
                {
                    if(model.ProfileImage != null)
                    {
						var uniqueFileName = Guid.NewGuid().ToString();
						File.Delete(userData.ProfileImage);
						var ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName + model.ProfileImage?.FileName);
						using (var stream = File.Create(ImagePath))
						{
							await model.ProfileImage.CopyToAsync(stream);
						}
                        userData.ProfileImage = ImagePath;
					}
                    //userData.UserName = model.UserName;
                    userData.PhoneNumber = model.PhoneNo;
                    userData.Email = model.Email;

                    _context.Users.Update(userData);
                    await _context.SaveChangesAsync();
                }
				return true;
			}
			catch
			{
                return false;
			}
		}

        public async Task<bool> AddToGroup(AddGroupModel model)
        {
            try
            {
                foreach (var item in model.UserId)
                {
                    var insertData = new GroupHasMembers()
                    {
                        GroupId = model.GroupId,
                        UserId = item
                    };
                    _context.GroupHasMembers.Add(insertData);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
	}
}
