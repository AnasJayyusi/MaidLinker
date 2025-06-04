using MaidLinker.Data;
using MaidLinker.Data.Entites;
using MaidLinker.Hubs;
using MaidLinker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using static MaidLinker.Areas.Identity.Pages.Account.LoginModel;
using static MaidLinker.Data.SharedEnum;

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
            var maids = _dbContext.Maids.Include(i => i.Nationality).Include(i => i.Langauges).ToList();
            return View(maids);
        }
        public ActionResult FillMaidsList()
        {
            var maids = _dbContext.Maids.Include(i => i.Nationality).Include(i => i.Langauges).ToList();
            return PartialView("MaidList", maids);
        }


        public IActionResult FillMaidsListWithFilter(string name, int nationalityId, int langId, Age age, Experience experience, MaritalStatus maritalStatus, string sortBy)
        {
            IQueryable<Maid> query = _dbContext.Maids
                               .Include(m => m.Nationality)
                               .Include(m => m.Langauges)
                               .Include(m => m.ServedCountries);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var loweredName = name.ToLower();

                query = query.Where(m =>
          (m.FirstNameEn != null && EF.Functions.Like(m.FirstNameEn, $"%{name}%")) ||
          (m.SecondNameEn != null && EF.Functions.Like(m.SecondNameEn, $"%{name}%")) ||
          (m.ThirdNameEn != null && EF.Functions.Like(m.ThirdNameEn, $"%{name}%")) ||
          (m.LastNameEn != null && EF.Functions.Like(m.LastNameEn, $"%{name}%")) ||
          (m.FirstNameAr != null && EF.Functions.Like(m.FirstNameAr, $"%{name}%")) ||
          (m.SecondNameAr != null && EF.Functions.Like(m.SecondNameAr, $"%{name}%")) ||
          (m.ThirdNameAr != null && EF.Functions.Like(m.ThirdNameAr, $"%{name}%")) ||
          (m.LastNameAr != null && EF.Functions.Like(m.LastNameAr, $"%{name}%")));
            }

            if (nationalityId > 0)
            {
                query = query.Where(m => m.NationalityId == nationalityId);
            }

            if (langId > 0)
            {
                query = query.Where(m => m.Langauges.Any(l => l.Id == langId));
            }

            if (age > 0)
            {
                var ageRange = GetDateRangeFromAgeEnum(age);
                if (ageRange.From.HasValue && ageRange.To.HasValue)
                {
                    query = query.Where(m => m.DateOfBirth >= ageRange.From.Value && m.DateOfBirth <= ageRange.To.Value);
                }
                else if (ageRange.To.HasValue) // Age.Old case
                {
                    query = query.Where(m => m.DateOfBirth <= ageRange.To.Value);
                }
            }

            if (experience > 0)
            {
                var experienceRange = GetExperienceRange(experience);
                if (experienceRange.Value.Min is not null)
                {
                    query = query.Where(m => m.TotalExperience >= experienceRange.Value.Min.Value);
                }

                if (experienceRange.Value.Max is not null)
                {
                    query = query.Where(m => m.TotalExperience <= experienceRange.Value.Max.Value);
                }
            }

            if (maritalStatus > 0)
            {
                query = query.Where(m => m.MaritalStatus == maritalStatus);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy == "Experience")
                {
                    query = query.OrderByDescending(m => m.TotalExperience);
                }
                if (sortBy == "Age")
                {
                    query = query.OrderByDescending(m => m.DateOfBirth);
                }
            }

            return PartialView("MaidList", query.ToList());
        }

        private (DateTime? From, DateTime? To) GetDateRangeFromAgeEnum(Age age)
        {
            var today = DateTime.Today;

            return age switch
            {
                Age.Child => (today.AddYears(-25), today.AddYears(-18)),
                Age.Teenager => (today.AddYears(-35), today.AddYears(-26)),
                Age.Young => (today.AddYears(-45), today.AddYears(-36)),
                Age.Aged => (today.AddYears(-50), today.AddYears(-46)),
                Age.Old => (null, today.AddYears(-51)), // Only before this
                _ => (null, null)
            };
        }

        private (double? Min, double? Max)? GetExperienceRange(Experience experience)
        {
            return experience switch
            {
                Experience.NoExperience => (0, 1),
                Experience.LowExperience => (2, 3),
                Experience.MediumExperience => (4, 5),
                Experience.HighExperience => (6, 10),
                Experience.Expert => (10.01, null), // أكبر من 10
                _ => null
            };
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
