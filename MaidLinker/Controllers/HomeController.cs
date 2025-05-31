using MaidLinker.Data;
using MaidLinker.Data.Entites;
using MaidLinker.Hubs;
using MaidLinker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using static MaidLinker.Areas.Identity.Pages.Account.LoginModel;

namespace MaidLinker.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext dbContext, ILogger<HomeController> logger, INotificationService notificationService) : base(dbContext, notificationService)
        {
            _logger = logger;
        }

        #region Handling Routing 
        public IActionResult Index()
        {
            if (User.IsInRole("Administrator"))
                return RedirectToAction("Dashboard", "Admin");
            return View();
        }
        public IActionResult MyProfile()
        {
            if (User.IsInRole(UserRolesEnum.Accountant.ToString()) || User.IsInRole(UserRolesEnum.Reception.ToString()))
            {
                return RedirectToAction("Profile", "ServiceProvider");
            }
            if (User.IsInRole(UserRolesEnum.Administrator.ToString()))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else return View();
        }
        #endregion

        #region About Us
        public IActionResult About()
        {
            return View();
        }
        #endregion

        #region Contact Us
        [HttpPost]
        public ActionResult AddNewFeedback(string name, string contact, string email, string message)
        {
            Feedback feedback = new Feedback();
            feedback.Name = name;
            feedback.Phone = contact;
            feedback.Email = email;
            feedback.Message = message;
            _dbContext.Feedbacks.Add(feedback);
            _dbContext.SaveChanges();
            return Ok("Added Successfully !!");
        }
        public IActionResult Contact()
        {
            return View();
        }
        #endregion

        #region Maids 
        public IActionResult Maids()
        {
            var maids = _dbContext.Maids.Include(i => i.Nationality).Include(i=>i.Langauges).ToList();
            return View(maids);
        }
        public ActionResult FillMaidsList()
        {
            var maids = _dbContext.Maids.Include(i=>i.Nationality).Include(i => i.Langauges).ToList();
            return PartialView("MaidList", maids);
        }

        public ActionResult FillMaidsList()
        {
            var maids = _dbContext.Maids.Include(i => i.Nationality).Include(i => i.Langauges).ToList();
            return PartialView("MaidList", maids);
        }

        public IActionResult GetDetails(int id)
        {
            var maid = _dbContext.Maids
                               .Include(m => m.Nationality)
                               .Include(m => m.Langauges)
                               .Include(m => m.ServedCountries)
                               .FirstOrDefault(m => m.Id == id);

            if (maid == null)
                return NotFound();

            return PartialView("_MaidDetailsPartial", maid);
        }

        //public ActionResult GetDoctorDetails(string userId)
        //{

        //    var doctorDetails = new DoctorDetailsModel();

        //    doctorDetails = _dbContext.UserProfiles.Where(w => w.UserId == userId)
        //                                                    .Include(i => i.Certifications)
        //                                                    .ThenInclude(cer => cer.Degree)
        //                                                     .Select(s => new DoctorDetailsModel
        //                                                     {
        //                                                         UserProfileId = s.Id,
        //                                                         UserId = s.UserId,
        //                                                         FullName = s.FullName,
        //                                                         ProfilePicturePath = s.ProfilePicturePath,
        //                                                         InsuranceAccepted = s.InsuranceAccepted ?? false,
        //                                                         OverView = new OverView()
        //                                                         {
        //                                                             Bio = s.Bio ?? "-",
        //                                                             SpecialtiesTitlesAr = s.SpecialtiesTitlesAr ?? "-",
        //                                                             SpecialtiesTitlesEn = s.SpecialtiesTitlesEn ?? "-",
        //                                                             Certifications = s.Certifications
        //                                                         }

        //                                                     }).Single();



        //    return PartialView("DoctorDetails", doctorDetails);
        //}
        //public ActionResult GetMaidservices(int userProfileId)
        //{
        //    List<UserServices> model = _dbContext.UserServices
        //                                        .Where(a => a.UserId == userProfileId && a.Status == Enums.ServicesStatusEnum.Approved)
        //                                        .ToList();

        //    return PartialView("Maidservices", model);
        //}
        //public ActionResult GetTimeClinicLocations(int userProfileId)
        //{
        //    string cultureCode = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        //    string direction = cultureCode.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "rtl" : "ltr";
        //    bool isEng = direction == "ltr" ? true : false;


        //    var model = _dbContext.TimeClinicLocations.Include(i => i.State)
        //                                              .Include(i => i.Country)
        //                                              .Include(i => i.City)
        //                                              .Include(i => i.Districts)
        //                                              .SingleOrDefault(w => w.UserProfileId == userProfileId);


        //    if (model == null)
        //        model = new TimeClinicLocation();

        //    if (string.IsNullOrWhiteSpace(model?.ClinicName))
        //    {
        //        model.ClinicName = "MaidLinker Clinic";
        //    }

        //    if (isEng)
        //    {
        //        model.Location = string.Join("-",
        //            model.Country?.TitleEn ?? "",
        //            model.State?.TitleEn ?? "",
        //            model.City?.TitleEn ?? "",
        //            model.Districts?.TitleEn ?? "");
        //    }
        //    else
        //    {
        //        model.Location = string.Join("-",
        //                            model.Country?.TitleAr ?? "",
        //                            model.State?.TitleAr ??"",
        //                            model.City?.TitleAr ?? "",
        //                            model.Districts?.TitleAr ?? "");
        //    }


        //    if (string.IsNullOrWhiteSpace(model.SundayOpenAt))
        //    {
        //        model.SundayOpenAt = "00:00";
        //    }
        //    if (string.IsNullOrWhiteSpace(model.SundayClosedAt))
        //    {
        //        model.SundayClosedAt = "00:00";
        //    }

        //    if (string.IsNullOrWhiteSpace(model.MondayOpenAt))
        //    {
        //        model.MondayOpenAt = "00:00";
        //    }
        //    if (string.IsNullOrWhiteSpace(model.MondayClosedAt))
        //    {
        //        model.MondayClosedAt = "00:00";
        //    }

        //    if (string.IsNullOrWhiteSpace(model.TuesdayOpenAt))
        //    {
        //        model.TuesdayOpenAt = "00:00";
        //    }
        //    if (string.IsNullOrWhiteSpace(model.TuesdayClosedAt))
        //    {
        //        model.TuesdayClosedAt = "00:00";
        //    }

        //    if (string.IsNullOrWhiteSpace(model.WednesdayOpenAt))
        //    {
        //        model.WednesdayOpenAt = "00:00";
        //    }
        //    if (string.IsNullOrWhiteSpace(model.WednesdayClosedAt))
        //    {
        //        model.WednesdayClosedAt = "00:00";
        //    }

        //    if (string.IsNullOrWhiteSpace(model.ThursdayOpenAt))
        //    {
        //        model.ThursdayOpenAt = "00:00";
        //    }
        //    if (string.IsNullOrWhiteSpace(model.ThursdayClosedAt))
        //    {
        //        model.ThursdayClosedAt = "00:00";
        //    }

        //    if (string.IsNullOrWhiteSpace(model.FridayOpenAt))
        //    {
        //        model.FridayOpenAt = "00:00";
        //    }
        //    if (string.IsNullOrWhiteSpace(model.FridayClosedAt))
        //    {
        //        model.FridayClosedAt = "00:00";
        //    }

        //    if (string.IsNullOrWhiteSpace(model.SaturdayOpenAt))
        //    {
        //        model.SaturdayOpenAt = "00:00";
        //    }
        //    if (string.IsNullOrWhiteSpace(model.SaturdayClosedAt))
        //    {
        //        model.SaturdayClosedAt = "00:00";
        //    }

        //    return PartialView("DoctorTimeClinicLocations", model);
        //}

        //[HttpGet]
        //public ActionResult GetUserCompanies(int userProfileId)
        //{
        //    var result = _dbContext.UserCompanies
        //                           .Include(i => i.Company)
        //                           .Where(w => w.UserProfileId == userProfileId).Select(s => s.Company).ToList();
        //    return Json(result);
        //}
        #endregion

        #region Culture
        [HttpGet]
        public IActionResult SetCulture(string culureCode, string returnUrl)
        {
            // Update the culture for the current request
            CultureInfo.CurrentCulture = new CultureInfo(culureCode);
            CultureInfo.CurrentUICulture = new CultureInfo(culureCode);

            // Store the selected culture in a persistent store (e.g., database, session, cookie)
            StoreCultureInCookies(culureCode);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        private void StoreCultureInCookies(string cultureCode)
        {
            // Store the selected culture in a cookie
            Response.Cookies.Append("CultureInfo", cultureCode);
        }
        #endregion

        #region Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

    }
}
