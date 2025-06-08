using MaidLinker.Data;
using MaidLinker.Data.Entites;
using MaidLinker.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static MaidLinker.Enums.SharedEnum;

namespace MaidLinker.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly INotificationService _notificationService;
        public string _loggedAspNetUserId;
        public int? _userProfileId;
        public const int _adminUserProfileId = 0;
        private readonly UserManager<IdentityUser> _userManager;


        public BaseController(ApplicationDbContext dbContext, INotificationService notificationService, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        //public int GetUserProfileId()
        //{
        //    string currentUserProfileId = GetAspNetUserId();

        //    if (_userProfileId == null)
        //    {
        //        if (currentUserProfileId != null)
        //            return _dbContext.UserProfiles.IgnoreQueryFilters().Where(w => w.UserId == currentUserProfileId).Select(x => x.Id).Single();
        //        return -1;
        //    }
        //    else
        //        return _userProfileId.Value;
        //}

        public string GetAspNetUserId()
        {
            if (_loggedAspNetUserId == null)
            {
                _loggedAspNetUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return _loggedAspNetUserId;
            }
            else
                return _loggedAspNetUserId;

        }

        //public string GetShortName()
        //{
        //    string fullName = _dbContext.UserProfiles.Where(w => w.Id == GetUserProfileId()).Select(s => s.FullName).Single();
        //    // Split the full name into individual words using space as the separator
        //    string[] nameParts = fullName.Split(' ');

        //    // The first element of the array will be the first name
        //    string firstName = nameParts[0];

        //    return firstName;
        //}

        //public string GetUserImage()
        //{
        //    var userId = GetUserProfileId();
        //    var userProfilePath = _dbContext.UserProfiles.Where(w => w.Id == userId).Select(a => a.ProfilePicturePath).FirstOrDefault();
        //    if (userProfilePath == null)
        //    {
        //        return "/users/images/Default-User-Profile.jpg";
        //    }
        //    return userProfilePath;
        //}

        //public void SetImagePathInCookies()
        //{
        //    var cookieOptions = new CookieOptions();
        //    cookieOptions.Expires = DateTime.Now.AddDays(1);
        //    cookieOptions.Path = "/";
        //    Response.Cookies.Append("userImagePath", GetUserImage(), cookieOptions);
        //}

        private  List<string> GetReceptionUserIds()
        {
            var receptionUsers =  _userManager.GetUsersInRoleAsync("Reception").Result;
            var receptionUserIds = receptionUsers.Select(u => u.Id).ToList();
            return receptionUserIds;
        }

        private List<string> GetAccountantUserIds()
        {
            var accountantUsers = _userManager.GetUsersInRoleAsync("Accountant").Result;
            var accountantUserIds = accountantUsers.Select(u => u.Id).ToList();
            return accountantUserIds;
        }

        private List<string> GetAllUsers()
        {
            var allUsers = _userManager.Users.ToList();
            var allUserIds = allUsers.Select(u => u.Id).ToList();
            return allUserIds;
        }

        public void PushNewNotification(NotificationTypeEnum type, AccountTypeEnum sendto, string? additionalData = null)
        {
            if (sendto == AccountTypeEnum.Reception)
            {
                var receptionUserIds = GetReceptionUserIds();
                foreach (var id in receptionUserIds)
                {
                    var notification = new Notification();
                    notification.CreatedByUserId = null;
                    notification.AssignedToUserId = id;
                    notification.CreationDate = DateTime.Now;

                    switch (type)
                    {
                        case NotificationTypeEnum.NewRequest:
                            notification.TitleAr = "طلب جديد";
                            notification.TitleEn = "New Order";
                            notification.MessageEn = $"You Have New Order With ID {additionalData}";
                            notification.MessageAr = $"لديك طلب جديد رقم الطلب {additionalData}";
                            break;
                    }

                    _dbContext.Notifications.Add(notification);
                    _dbContext.SaveChanges();

                    _notificationService.SendMessage("Notify", "You Have New notification" + additionalData);
                }
            }

            if (sendto == AccountTypeEnum.Accountant)
            {
                var accountantUserIds = GetAccountantUserIds();
                foreach (var id in accountantUserIds)
                {
                    var notification = new Notification();
                    notification.CreatedByUserId = null;
                    notification.AssignedToUserId = id;
                    notification.CreationDate = DateTime.Now;

                    switch (type)
                    {
                        case NotificationTypeEnum.TakeOverRequest:
                            notification.TitleAr = "طلب جديد";
                            notification.TitleEn = "New Order";
                            notification.MessageEn = $"You Have New Order Need to prepared and confirm With ID {additionalData}";
                            notification.MessageAr = $"لديك طلب يحتاج الى تجهيز والموافقه  رقم الطلب {additionalData}";
                            break;
                    }

                    _dbContext.Notifications.Add(notification);
                    _dbContext.SaveChanges();

                    _notificationService.SendMessage("Notify", "You Have New notification" + additionalData);
                }
            }

            if (sendto == AccountTypeEnum.All)
            {
                var allUserIds = GetAllUsers();
                foreach (var id in allUserIds)
                {
                    var notification = new Notification();
                    notification.CreatedByUserId = null;
                    notification.AssignedToUserId = id;
                    notification.CreationDate = DateTime.Now;
                    switch (type)
                    {
                        case NotificationTypeEnum.NewRequest:
                            notification.TitleAr = "طلب جديد";
                            notification.TitleEn = "New Order";
                            notification.MessageEn = $"You Have New Order With ID {additionalData}";
                            notification.MessageAr = $"لديكم طلب جديد رقم الطلب {additionalData}";
                            break;
                        case NotificationTypeEnum.Confirm:
                            notification.TitleAr = "طلب موافق عليه";
                            notification.TitleEn = "New Order";
                            notification.MessageEn = $"You Have New Order have been confirmed With ID {additionalData}";
                            notification.MessageAr = $"لديكم طلب تم الموافقه عليه رقم الطلب {additionalData}";
                            break;
                        case NotificationTypeEnum.Cancel:
                            notification.TitleAr = "طلب ملغي";
                            notification.TitleEn = "Order Cancelled";
                            notification.MessageEn = $"Order With ID {additionalData} has been cancelled.";
                            notification.MessageAr = $"تم إلغاء الطلب برقم {additionalData}.";
                            break;
                        case NotificationTypeEnum.Completed:
                            notification.TitleAr = "تم اكمال الطلب";
                            notification.TitleEn = "Order Completed";
                            notification.MessageEn = $"Order With ID {additionalData} has been Completed.";
                            notification.MessageAr = $"تم اكمال الطلب برقم {additionalData}.";
                            break;
                    }
                    _dbContext.Notifications.Add(notification);
                    _dbContext.SaveChanges();
                    _notificationService.SendMessage("Notify", "You Have New notification" + additionalData);
                }
            }

        }


        public string GetFileFullPath(IWebHostEnvironment _webHostEnvironment, string folderName, string fileName)
        {
            //Build the File Path.  
            return Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
        }
    }
}

