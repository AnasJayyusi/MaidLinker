using MaidLinker.Data;
using MaidLinker.Data.Entites;
using MaidLinker.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static MaidLinker.Data.SharedEnum;

namespace MaidLinker.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly INotificationService _notificationService;
        public string _loggedAspNetUserId;
        public int? _userProfileId;
        public const int _adminUserProfileId = 0;

        public BaseController(ApplicationDbContext dbContext, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
        }

        public int GetUserProfileId()
        {
            string currentUserProfileId = GetAspNetUserId();
            if (_userProfileId == null)
            {
                if (currentUserProfileId != null)
                    return _dbContext.UserProfiles.IgnoreQueryFilters().Where(w => w.UserId == currentUserProfileId).Select(x => x.Id).Single();
                return -1;
            }
            else
                return _userProfileId.Value;
        }

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

        public string GetShortName()
        {
            string fullName = _dbContext.UserProfiles.Where(w => w.Id == GetUserProfileId()).Select(s => s.FullName).Single();
            // Split the full name into individual words using space as the separator
            string[] nameParts = fullName.Split(' ');

            // The first element of the array will be the first name
            string firstName = nameParts[0];

            return firstName;
        }

        public string GetUserImage()
        {
            var userId = GetUserProfileId();
            var userProfilePath = _dbContext.UserProfiles.Where(w => w.Id == userId).Select(a => a.ProfilePicturePath).FirstOrDefault();
            if (userProfilePath == null)
            {
                return "/users/images/Default-User-Profile.jpg";
            }
            return userProfilePath;
        }

        public void SetImagePathInCookies()
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(1);
            cookieOptions.Path = "/";
            Response.Cookies.Append("userImagePath", GetUserImage(), cookieOptions);
        }


        public void PushNewNotification(NotificationTypeEnum type, int fromUserId, int toUserId, string? additionalData = null)
        {
            var notification = new Notification();
            notification.CreatedByUserId = fromUserId;
            notification.AssignedToUserId = toUserId;
            notification.CreationDate = DateTime.Now;

            switch (type)
            {
                case NotificationTypeEnum.NewOrder:
                    notification.TitleAr = "طلب جديد";
                    notification.TitleEn = "New Order";
                    notification.MessageEn = $"You Have New Order With ID {additionalData}";
                    notification.MessageAr = $"لديك طلب جديد رقم الطلب {additionalData}";
                    break;
                case NotificationTypeEnum.ApprovedOrder:
                    notification.TitleAr = "قبول طلب";
                    notification.TitleEn = "Approved Order";
                    notification.MessageEn = $"Order With ID {additionalData} Approved";
                    notification.MessageAr = $"تم قبول طلب رقم {additionalData}";
                    break;
                case NotificationTypeEnum.RejectOrder:
                    notification.TitleAr = "رفض طلب";
                    notification.TitleEn = "Reject Order";
                    notification.MessageEn = $"Order With ID {additionalData} Rejected";
                    notification.MessageAr = $"تم رفض طلب رقم {additionalData}"; 
                    break;
                case NotificationTypeEnum.SendProfileToReview:
                    notification.TitleAr = "طلب مراجعة جديد";
                    notification.TitleEn = "New Review Request";
                    notification.MessageEn = $"You have new requst review from {additionalData}";
                    notification.MessageAr = $" تم ارسال طلب مراجعه جديد من {additionalData}";
                    break;

                case NotificationTypeEnum.RejectProfile:
                    notification.TitleAr = "رفض";
                    notification.TitleEn = "Reject";
                    notification.MessageEn = $"Your Profile Rejected";
                    notification.MessageAr = $"تم رفض صفحتك الشخصية";
                    break;

                case NotificationTypeEnum.SendNewService:
                    notification.TitleAr = "خدمه جديده";
                    notification.TitleEn = "New Service";
                    notification.MessageEn = $"You have new add service request review from {additionalData}";
                    notification.MessageAr = $" تم ارسال طلب اضافة خدمه جديد من {additionalData}";
                    break;
                case NotificationTypeEnum.ApprovedNewService:
                    notification.TitleAr = "موافقه";
                    notification.TitleEn = "Approved";
                    notification.MessageEn = $"Your new service request has been approved";
                    notification.MessageAr = $"تم الموفقه على خدمتك الجديده";
                    break;
                case NotificationTypeEnum.RejectNewService:
                    notification.TitleAr = "رفض";
                    notification.TitleEn = "Reject";
                    notification.MessageEn = $"Your new service was reject";
                    notification.MessageAr = $"تم رفض خدمتك الجديده";
                    break;
                case NotificationTypeEnum.ActivateUserProfile:
                    notification.TitleAr = "تفعيل";
                    notification.TitleEn = "Activate";
                    notification.MessageEn = $"Your Profile has been Activated";
                    notification.MessageAr = $"لقد تم تفعيل ملفك الشخصي";
                    break;
                case NotificationTypeEnum.DeactivateUserProfile:
                    notification.TitleAr = "إلغاء التنشيط";
                    notification.TitleEn = "Deactivate";
                    notification.MessageEn = $"Your Profile has been Deactivated";
                    notification.MessageAr = $"لقد تم إلغاء تنشيط ملفك الشخصي";
                    break;

            }

            _dbContext.Notifications.Add(notification);
            _dbContext.SaveChanges();

            _notificationService.SendMessage("Notify", "You Have New notification" + additionalData);

        }


        public string GetFileFullPath(IWebHostEnvironment _webHostEnvironment, string folderName, string fileName)
        {
            //Build the File Path.  
            return Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
        }
    }
}

