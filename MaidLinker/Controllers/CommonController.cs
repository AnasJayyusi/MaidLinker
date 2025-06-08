using MaidLinker.Data;
using MaidLinker.Data.Entites;
using MaidLinker.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static MaidLinker.Enums.SharedEnum;
namespace MaidLinker.Controllers
{

    [Route("Common")]
    public class CommonController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        public CommonController(ApplicationDbContext dbContext, INotificationService notificationService, UserManager<IdentityUser> userManager) : base(dbContext, notificationService, userManager)
        {
        }

        #region Shared DropDownList

        [HttpGet]
        [Route("GetCountriesDDL")]
        public ActionResult GetCountriesDDL()
        {
            // Retrieve the data for the dropdown list
            var dropdownData = _dbContext.Countries.ToList();

            // Pass the data to the view
            return Json(dropdownData);
        }

        [HttpGet]
        [Route("GetNationalitiesDDL")]
        public ActionResult GetNationalitiesDDL()
        {
            // Retrieve the data for the dropdown list
            var dropdownData = _dbContext.Nationalities.ToList();

            // Pass the data to the view
            return Json(dropdownData);
        }

        [HttpGet]
        [Route("GetLanguagesDDL")]
        public ActionResult GetLanguagesDDL()
        {
            // Retrieve the data for the dropdown list
            var dropdownData = _dbContext.Languages.ToList();

            // Pass the data to the view
            return Json(dropdownData);
        }

        [HttpGet]
        [Route("GetAgeDDL")]
        public ActionResult GetAgeDDL()
        {
            var dropdownData = new List<object>
                             {
                                 new { Id = "1", Title = "18-25" },
                                 new { Id = "2", Title = "26-35" },
                                 new { Id = "3", Title = "36-45" },
                                 new { Id = "4", Title = "46-50" },
                                 new { Id = "5", Title = "50+" }
                             };

            return Json(dropdownData);
        }

        [HttpGet]
        [Route("GetYearsExperiencesDDL")]
        public ActionResult GetYearsExperiencesDDL()
        {
            var culture = Thread.CurrentThread.CurrentCulture.Name;
            bool isArabic = culture.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            var dropdownData = new List<object>
              {
                  new { Id = "0",   Title = isArabic ? "0 - 1 سنة"      : "0 - 1 years" },
                  new { Id = "1",   Title = isArabic ? "2 - 3 سنوات"    : "2 - 3 years" },
                  new { Id = "2",   Title = isArabic ? "4 - 5 سنوات"    : "4 - 5 years" },
                  new { Id = "3",  Title = isArabic ? "6 - 10 سنوات"   : "6 - 10 years" },
                  new { Id = "4",   Title = isArabic ? "أكثر من 10 سنوات" : "10+ years" }
              };

            return Json(dropdownData);
        }

        [Route("GetMaritalStatus")]
        public ActionResult GetMaritalStatusDDL()
        {
            var culture = Thread.CurrentThread.CurrentCulture.Name;

            var dropdownData = new List<object>
                             {
                                 new { Id = (int)MaritalStatus.Single,   Title = culture.StartsWith("ar") ? "عزباء" : "Single" },
                                 new { Id = (int)MaritalStatus.Married,  Title = culture.StartsWith("ar") ? "متزوجة" : "Married" },
                                 new { Id = (int)MaritalStatus.Divorced, Title = culture.StartsWith("ar") ? "مطلقة" : "Divorced" },
                                 new { Id = (int)MaritalStatus.Widowed,  Title = culture.StartsWith("ar") ? "أرملة" : "Widowed" }
                             };

            return Json(dropdownData);
        }

        [HttpGet]
        [Route("GetAccountTypesDDL")]
        public ActionResult GetAccountTypesDDL()
        {
            // Retrieve the data for the dropdown list
            var dropdownData = _dbContext.AccountTypes.ToList();

            // Pass the data to the view
            return Json(dropdownData);
        }

        [HttpGet]
        [Route("GetPractitionerTypesDDL")]
        public ActionResult GetPractitionerTypesDDL()
        {
            // Retrieve the data for the dropdown list
            var dropdownData = _dbContext.PractitionerTypes.Where(w => w.IsActive).ToList();

            // Pass the data to the view
            return Json(dropdownData);
        }

        [HttpPost]
        [Route("SendRequest/{maidId}/{name}/{phone}")]
        public async Task<IActionResult> SendRequest(int maidId, string name, string phone)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
            {
                return BadRequest("Name and phone are required.");
            }

            var maid = await _dbContext.Maids.FindAsync(maidId);
            if (maid == null)
            {
                return NotFound("Maid not found.");
            }

            var request = new Request
            {
                MaidId = maidId,
                Name = name,
                Phone = phone,
                RequestDate = DateTime.Now,
                Status = RequestStatus.New
            };

            _dbContext.Requests.Add(request);
            await _dbContext.SaveChangesAsync();
            var newRequestId = request.Id;  // <-- here you get the new request ID
            PushNewNotification(NotificationTypeEnum.NewRequest,AccountTypeEnum.All , $"#{newRequestId.ToString()}");


            return Ok();
        }
        #endregion

        #region Other Methods

        [HttpGet]
        [Route("GetContactInfo")]
        public JsonResult GetContactInfo()
        {

            // Assuming you have a DbContext named _context and SettingValue column exists
            var keys = new[] { "PhoneNumber", "WhatsAppNumber", "AddressAr", "AddressEn", "FacebookUrl", "InstagramUrl" };

            var settings = _dbContext.GeneralSettings
                            .Where(s => keys.Contains(s.SettingKey))
                            .ToDictionary(s => s.SettingKey, s => s.SettingValue);


            var culture = Thread.CurrentThread.CurrentCulture.Name;
            bool isArabic = culture.StartsWith("ar", StringComparison.OrdinalIgnoreCase);


            var data = new
            {
                PhoneNumber = settings.ContainsKey("PhoneNumber") ? settings["PhoneNumber"] : "",
                WhatsAppNumber = settings.ContainsKey("WhatsAppNumber") ? settings["WhatsAppNumber"] : "",
                Address = isArabic
                  ? (settings.ContainsKey("AddressAr") ? settings["AddressAr"] : "")
                : (settings.ContainsKey("AddressEn") ? settings["AddressEn"] : ""),

            FacebookUrl = settings.ContainsKey("FacebookUrl") ? settings["FacebookUrl"] : "",
                InstagramUrl = settings.ContainsKey("InstagramUrl") ? settings["InstagramUrl"] : ""
            };

            return Json(data);
        }
        #endregion

    }
}