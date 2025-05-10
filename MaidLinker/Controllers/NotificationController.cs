using MaidLinker.Data;
using MaidLinker.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Data;
namespace MaidLinker.Controllers
{

    [Route("Notification")]
    [Authorize(Roles = "Administrator,ServiceProvider,Beneficiary")]
    public class NotificationController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationController(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext, INotificationService notificationService) : base(dbContext, notificationService)
        {
            _userManager = userManager;
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("GetNotificationsCount")]
        public ActionResult GetNotificationsCount()
        {
            var currentUserId = GetUserProfileId();

            if (currentUserId < 0)
                return Json(0);

            var count = _dbContext.Notifications.Count(a => a.AssignedToUserId == currentUserId && !a.IsRead);

            return Json(count);
        }

        [HttpGet]
        [Route("GetNotifications")]
        public ActionResult GetNotifications()
        {
            var currentUserId = GetUserProfileId();
            var maxNumberOfNotifications = _dbContext.Notifications.Count(a => a.AssignedToUserId == currentUserId && !a.IsRead);
            if (maxNumberOfNotifications < 10 || maxNumberOfNotifications > 15)
                maxNumberOfNotifications = 15;

            var model = _dbContext.Notifications.Where(a => a.AssignedToUserId == currentUserId)
                                                .OrderByDescending(a => a.CreationDate)
                                                .Take(maxNumberOfNotifications).ToList();
            // Pass the data to the view
            return PartialView("NotificationsList", model);

        }

        [HttpGet]
        [Route("MarkAllNotificationsReaded")]
        public ActionResult MarkAllNotificationsReaded()
        {
            var currentUserId = GetUserProfileId();

            var model = _dbContext.Notifications.Where(a => a.AssignedToUserId == currentUserId).OrderByDescending(a => a.CreationDate).ToList();
            foreach (var item in model)
            {
                item.IsRead = true;
            }
            _dbContext.SaveChanges();
            return Ok();
        }

    }
}
