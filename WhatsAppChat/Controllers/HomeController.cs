using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WhatsAppChat.Core.Models;
using WhatsAppChat.Core.Repositories;
using WhatsAppChat.Data.DataModel;
using WhatsAppChat.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WhatsAppChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRegisters _registers;
        private readonly IGet _get;

        public HomeController(ILogger<HomeController> logger, IRegisters registers, IGet get)
        {
            _logger = logger;
            _registers = registers;
            _get = get;
        }

        public async Task<IActionResult> Index()
        {
            Users? data = null;
            if (Request.Cookies["userId"] != null)
            {
                // Current User Details
                data = await _get.getCurrentUser(Convert.ToInt32(Request.Cookies["userId"]));
                ViewBag.CurrentUser = data;
                ViewBag.CurrentUser.ProfileImage = data.ProfileImage?.Split("wwwroot/").Last().ToString();

				#region User others than current User
				var allData = await _get.getAllUsers(Convert.ToInt32(Request.Cookies["userId"]));
				ViewBag.AllUsers = allData;
                for(int i = 0; i<ViewBag.AllUsers.Count; i++)
                {
                    ViewBag.AllUsers[i].ProfileImage = allData[i].ProfileImage?.Split("wwwroot/").Last().ToString();
				}
				#endregion

				#region Unread Messages
				List<UnreadModel>? unreadCount = new List<UnreadModel>();
                foreach (var item in allData)
                {
                    UnreadModel count = await _get.GetCount(item.Id, Convert.ToInt32(Request.Cookies["userId"]));
                    unreadCount.Add(count);
                }
                ViewBag.unreadCount = unreadCount;
				#endregion

				#region Group Working
				var groupData = await _get.getGroups(Convert.ToInt32(Request.Cookies["userId"]));
                int counter = 0;
                foreach (var item in groupData)
                {
                    groupData[counter].GroupIcon = item.GroupIcon?.Split("wwwroot/").Last().ToString();
                    counter++;
                }
                ViewBag.groupData = groupData;

                List<GroupUnreadModel>? GorupUnreadList = new List<GroupUnreadModel>();
                if(groupData.Any())
                {
				    foreach (var item in groupData)
                    {
                        var groupUnreadData = await _get.GetGroupUnread(Convert.ToInt32(Request.Cookies["userId"]), item.Id);
                        GorupUnreadList.Add(groupUnreadData);
                    }
                }
                ViewBag.GroupUnreadList = GorupUnreadList;
                #endregion
            }
			return View();
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if(await _registers.LoginAsync(model))
            {
                return RedirectToAction("Index");
            }
            ViewBag.LoginError = "Incorrect User Name Or Password!";
            return View();
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            _logger.LogInformation("Log Test In Reg. Post");
            if(!ModelState.IsValid)
            {
                return View();
            }
            if(model.ProfileImage?.Length >= 307200)
            {
                ViewBag.ProfileError = "Profile Photo Must be less than 300KB";
                return View();
            }
            bool success = await _registers.RegisterAsync(model);
            if (success)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
