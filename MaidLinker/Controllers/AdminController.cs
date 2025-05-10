using MaidLinker.Areas.Identity.Pages.Account.Manage;
using MaidLinker.Data;
using MaidLinker.Data.Entites;
using MaidLinker.Hubs;
using MaidLinker.Models;
using MaidLinker.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Globalization;
using static MaidLinker.Data.SharedEnum;


namespace MaidLinker.Controllers
{

    [Route("Admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminController(
            ApplicationDbContext dbContext,
            IWebHostEnvironment webHostEnvironment,
            INotificationService notificationService,
            UserManager<IdentityUser> userManager
            ) : base(dbContext, notificationService)
        {
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }



        #region Dashboard
        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            return View();

        }

        [Route("Statistics")]
        public IActionResult Statistics()
        {
            return View();

        }
        #endregion

        #region Nationalities
        [Route("MasterList/Nationalities")]
        public IActionResult Nationalities()
        {
            // Retrieve the value from TempData
            bool? isFromDeleteRequest = TempData["isFromDeleteRequest"] as bool?;
            bool? isSuccessDelete = TempData["isSuccessDelete"] as bool?;

            if (isFromDeleteRequest != null && isSuccessDelete != null)
            {
                // Clear the TempData value to avoid persisting it across subsequent requests
                TempData.Remove("isFromDeleteRequest");
                TempData.Remove("isSuccessDelete");

                // Use the value as needed
                ViewBag.SuccessMessage = isSuccessDelete;
            }

            return View(_dbContext.Nationalities.ToList());
        }

        [Route("GetNationalities")]
        public IActionResult NationalitiesList()
        {
            var Nationalitys = _dbContext.Nationalities.ToList();
            return PartialView("NationalitiesList", Nationalitys);
        }

        [HttpPost]
        [Route("AddNationality")]
        public IActionResult AddNationality([FromBody] Nationality Nationality)
        {
            if (Nationality == null || string.IsNullOrEmpty(Nationality.TitleEn) || string.IsNullOrEmpty(Nationality.TitleAr))
            {
                return BadRequest("Please fill all fields.");
            }

            bool isDuplicate = _dbContext.Nationalities.Any(w => w.TitleEn == Nationality.TitleEn && w.TitleAr == Nationality.TitleAr);
            if (isDuplicate)
            {
                return BadRequest("The details for the Practitioner Type have already been added.");
            }

            else
            {
                if (!string.IsNullOrEmpty(Nationality.TitleAr) || !string.IsNullOrEmpty(Nationality.TitleEn))
                {
                    _dbContext.Nationalities.Add(Nationality);
                    _dbContext.SaveChanges();
                }
            }

            return Ok();
        }

        [HttpPost]
        [Route("UpdateNationality")]
        public IActionResult UpdateNationality([FromBody] Nationality Nationality)
        {
            if (Nationality == null || string.IsNullOrEmpty(Nationality.TitleEn) || string.IsNullOrEmpty(Nationality.TitleAr))
            {
                return BadRequest("Please fill all fields.");
            }

            bool isDuplicate = _dbContext.Nationalities.Any(w => w.TitleEn == Nationality.TitleEn && w.TitleAr == Nationality.TitleAr);
            if (isDuplicate)
            {
                return BadRequest("The details for the Practitioner Type have already been added.");
            }

            else
            {
                if (!string.IsNullOrEmpty(Nationality.TitleAr) || !string.IsNullOrEmpty(Nationality.TitleEn))
                {
                    _dbContext.Nationalities.Update(Nationality);
                    _dbContext.SaveChanges();
                }
            }

            return Ok();
        }

        [HttpGet]
        [Route("DeleteNationality/{id}")]
        public IActionResult DeleteNationality(int id)
        {


            // Retrieve the Nationality from the database using the id
            var Nationality = _dbContext.Nationalities.Find(id);


            if (Nationality == null)
            {
                // Handle the case where the Nationality type doesn't exist
                TempData["isSuccessDelete"] = false;
            }

            // Remove the practitioner type from the DbSet
            _dbContext.Nationalities.Remove(Nationality);

            // Save the changes to the database
            _dbContext.SaveChanges();
            TempData["isSuccessDelete"] = true;


            // Set the value in TempData
            TempData["isFromDeleteRequest"] = true;
            return RedirectToAction("Nationalities");
        }


        [HttpGet]
        [Route("GetNationality/{id}")]
        public Nationality GetNationality(int id)
        {
            var Nationalitys = _dbContext.Nationalities.Single(w => w.Id == id);
            return Nationalitys;
        }

        #endregion

        #region PractitionerTypes
        [Route("MasterList/PractitionerTypes")]
        public IActionResult PractitionerTypes()
        {
            // Retrieve the value from TempData
            bool? isFromDeleteRequest = TempData["isFromDeleteRequest"] as bool?;
            bool? isSuccessDelete = TempData["isSuccessDelete"] as bool?;

            if (isFromDeleteRequest != null && isSuccessDelete != null)
            {
                // Clear the TempData value to avoid persisting it across subsequent requests
                TempData.Remove("isFromDeleteRequest");
                TempData.Remove("isSuccessDelete");

                // Use the value as needed
                ViewBag.SuccessMessage = isSuccessDelete;
            }

            return View(_dbContext.PractitionerTypes.ToList());
        }

        [Route("GetPractitionerTypes")]
        public IActionResult PractitionerTypesList()
        {
            var practitionerTypes = _dbContext.PractitionerTypes.ToList();
            return PartialView("PractitionerTypesList", practitionerTypes);
        }

        [HttpPost]
        [Route("AddPractitionerType")]
        public IActionResult AddPractitionerType([FromBody] PractitionerType practitionerType)
        {
            if (practitionerType == null || string.IsNullOrEmpty(practitionerType.TitleEn) || string.IsNullOrEmpty(practitionerType.TitleAr))
            {
                return BadRequest("Please fill all fields.");
            }

            bool isDuplicate = _dbContext.PractitionerTypes.Any(w => w.TitleEn == practitionerType.TitleEn && w.TitleAr == practitionerType.TitleAr);
            if (isDuplicate)
            {
                return BadRequest("The details for the Practitioner Type have already been added.");
            }

            else
            {
                if (!string.IsNullOrEmpty(practitionerType.TitleAr) || !string.IsNullOrEmpty(practitionerType.TitleEn))
                {
                    _dbContext.PractitionerTypes.Add(practitionerType);
                    _dbContext.SaveChanges();
                }
            }

            return Ok();
        }

        [HttpPost]
        [Route("UpdatePractitionerType")]
        public IActionResult UpdatePractitionerType([FromBody] PractitionerType practitionerType)
        {
            if (practitionerType == null || string.IsNullOrEmpty(practitionerType.TitleEn) || string.IsNullOrEmpty(practitionerType.TitleAr))
            {
                return BadRequest("Please fill all fields.");
            }

            bool isDuplicate = _dbContext.PractitionerTypes.Any(w => w.TitleEn == practitionerType.TitleEn && w.TitleAr == practitionerType.TitleAr);
            if (isDuplicate)
            {
                return BadRequest("The details for the Practitioner Type have already been added.");
            }

            else
            {
                if (!string.IsNullOrEmpty(practitionerType.TitleAr) || !string.IsNullOrEmpty(practitionerType.TitleEn))
                {
                    _dbContext.PractitionerTypes.Update(practitionerType);
                    _dbContext.SaveChanges();
                }
            }

            return Ok();
        }

        [HttpGet]
        [Route("UpdatePractitionerTypeStatus/{id}/{isActive}")]
        public IActionResult UpdatePractitionerTypeStatus(int id, bool isActive)
        {
            // Logic to update the status of the practitioner type with the given ID
            try
            {
                var practitionerType = _dbContext.PractitionerTypes.SingleOrDefault(p => p.Id == id);
                if (practitionerType == null)
                {
                    return NotFound();
                }

                practitionerType.IsActive = isActive;
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the status.");
            }
        }

      


        [HttpGet]
        [Route("GetPractitionerType/{id}")]
        public PractitionerType GetPractitionerType(int id)
        {
            var practitionerTypes = _dbContext.PractitionerTypes.Single(w => w.Id == id);
            return practitionerTypes;
        }
        #endregion

        #region GeneralSettings
        [Route("Settings/GeneralSettings")]
        public IActionResult GeneralSettings()
        {
            // Retrieve the value from TempData
            bool? isFromDeleteRequest = TempData["isFromDeleteRequest"] as bool?;
            bool? isSuccessDelete = TempData["isSuccessDelete"] as bool?;

            if (isFromDeleteRequest != null && isSuccessDelete != null)
            {
                // Clear the TempData value to avoid persisting it across subsequent requests
                TempData.Remove("isFromDeleteRequest");
                TempData.Remove("isSuccessDelete");

                // Use the value as needed
                ViewBag.SuccessMessage = isSuccessDelete;
            }

            return View(_dbContext.GeneralSettings.FirstOrDefault());
        }

        [HttpPost]
        [Route("Settings/UpdateGeneralSetting")]

        public IActionResult UpdateGeneralSetting(GeneralSettings updatedSetting)
        {
            var generalSettings = _dbContext.GeneralSettings.FirstOrDefault();
            if (generalSettings == null)
                generalSettings = new GeneralSettings();
                _dbContext.GeneralSettings.Add(generalSettings);
            _dbContext.SaveChanges();
            return RedirectToAction("GeneralSettings");
        }
        #endregion

        #region ServicesReport
      

        //[HttpGet]
        //[Route("ExportReport")]
        //public ActionResult ExportReport()
        //{
        //    // Set Title
        //    string reportName = string.Format("Invoice" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        //    // Generate Report
        //    ExcelReportGenerator reportGenerator = new ExcelReportGenerator();
        //    var vatValue = Convert.ToDouble(0);
        //    var sitePercentage = Convert.ToDouble(0);
        //    var tempFilePath = reportGenerator.GenerateReport(GetReport(), vatValue, sitePercentage);

        //    return File(System.IO.File.ReadAllBytes(tempFilePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportName);
        //}

        #endregion

        #region Feedbacks
        [Route("Settings/Feedbacks")]
        public IActionResult Feedbacks()
        {
            // Retrieve the value from TempData
            bool? isFromDeleteRequest = TempData["isFromDeleteRequest"] as bool?;
            bool? isSuccessDelete = TempData["isSuccessDelete"] as bool?;

            if (isFromDeleteRequest != null && isSuccessDelete != null)
            {
                // Clear the TempData value to avoid persisting it across subsequent requests
                TempData.Remove("isFromDeleteRequest");
                TempData.Remove("isSuccessDelete");

                // Use the value as needed
                ViewBag.SuccessMessage = isSuccessDelete;
            }

            return View(_dbContext.Feedbacks.ToList());
        }

        [Route("GetFeedbacks")]
        public IActionResult GetFeedbacks()
        {
            var feedbacks = _dbContext.Feedbacks.ToList();
            return PartialView("FeedbacksList", feedbacks);
        }
        [HttpGet]
        [Route("UpdatefeedbacksStatus/{id}/{isSeen}")]
        public IActionResult UpdatefeedbacksStatus(int id, bool isSeen)
        {
            // Logic to update the status of the practitioner type with the given ID
            try
            {
                var practitionerType = _dbContext.Feedbacks.SingleOrDefault(p => p.Id == id);
                if (practitionerType == null)
                {
                    return NotFound();
                }

                practitionerType.StatusId = isSeen ? Enums.FeedbackStatusEnum.Seen : Enums.FeedbackStatusEnum.Unread;
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the status.");
            }
        }
        #endregion

        #region Countries
        [Route("MasterList/Countries")]
        public IActionResult Countries()
        {
            // Retrieve the value from TempData
            bool? isFromDeleteRequest = TempData["isFromDeleteRequest"] as bool?;
            bool? isSuccessDelete = TempData["isSuccessDelete"] as bool?;

            if (isFromDeleteRequest != null && isSuccessDelete != null)
            {
                // Clear the TempData value to avoid persisting it across subsequent requests
                TempData.Remove("isFromDeleteRequest");
                TempData.Remove("isSuccessDelete");

                // Use the value as needed
                ViewBag.SuccessMessage = isSuccessDelete;
            }

            return View(_dbContext.Countries.ToList());
        }

        [Route("GetCountries")]
        public IActionResult CountriesList()
        {
            var countrys = _dbContext.Countries.ToList();
            return PartialView("CountriesList", countrys);
        }

        [HttpPost]
        [Route("AddCountry")]
        public IActionResult AddCountry([FromBody] Country country)
        {
            if (country == null || string.IsNullOrEmpty(country.TitleEn) || string.IsNullOrEmpty(country.TitleAr))
            {
                return BadRequest("Please fill all fields.");
            }

            bool isDuplicate = _dbContext.Countries.Any(w => w.TitleEn == country.TitleEn && w.TitleAr == country.TitleAr);
            if (isDuplicate)
            {
                return BadRequest("The details for the Practitioner Type have already been added.");
            }

            else
            {
                if (!string.IsNullOrEmpty(country.TitleAr) || !string.IsNullOrEmpty(country.TitleEn))
                {
                    _dbContext.Countries.Add(country);
                    _dbContext.SaveChanges();
                }
            }

            return Ok();
        }

        [HttpPost]
        [Route("UpdateCountry")]
        public IActionResult UpdateCountry([FromBody] Country country)
        {
            if (country == null || string.IsNullOrEmpty(country.TitleEn) || string.IsNullOrEmpty(country.TitleAr))
            {
                return BadRequest("Please fill all fields.");
            }

            bool isDuplicate = _dbContext.Countries.Any(w => w.TitleEn == country.TitleEn && w.TitleAr == country.TitleAr);
            if (isDuplicate)
            {
                return BadRequest("The details for the Practitioner Type have already been added.");
            }

            else
            {
                if (!string.IsNullOrEmpty(country.TitleAr) || !string.IsNullOrEmpty(country.TitleEn))
                {
                    _dbContext.Countries.Update(country);
                    _dbContext.SaveChanges();
                }
            }

            return Ok();
        }

        [HttpGet]
        [Route("DeleteCountry/{id}")]
        public IActionResult DeleteCountry(int id)
        {

          
                // Retrieve the Country from the database using the id
                var country = _dbContext.Countries.Find(id);


                if (country == null)
                {
                    // Handle the case where the Country type doesn't exist
                    TempData["isSuccessDelete"] = false;
                }

                // Remove the practitioner type from the DbSet
                _dbContext.Countries.Remove(country);

                // Save the changes to the database
                _dbContext.SaveChanges();
                TempData["isSuccessDelete"] = true;


            // Set the value in TempData
            TempData["isFromDeleteRequest"] = true;
            return RedirectToAction("Countries");
        }


        [HttpGet]
        [Route("GetCountry/{id}")]
        public Country GetCountry(int id)
        {
            var countrys = _dbContext.Countries.Single(w => w.Id == id);
            return countrys;
        }

        private int? GetCountryIdByCityId(int? cityId)
        {
            return _dbContext.Cities.SingleOrDefault(x => x.Id == cityId)?.CountryId;
        }
        #endregion

        #region Cities
        [Route("MasterList/Cities")]
        public IActionResult Cities()
        {
            // Retrieve the value from TempData
            bool? isFromDeleteRequest = TempData["isFromDeleteRequest"] as bool?;
            bool? isSuccessDelete = TempData["isSuccessDelete"] as bool?;

            if (isFromDeleteRequest != null && isSuccessDelete != null)
            {
                // Clear the TempData value to avoid persisting it across subsequent requests
                TempData.Remove("isFromDeleteRequest");
                TempData.Remove("isSuccessDelete");

                // Use the value as needed
                ViewBag.SuccessMessage = isSuccessDelete;
            }

            return View(_dbContext.Cities.Include(i => i.Country)
                                          .ToList());
        }


        [Route("GetCities")]
        public IActionResult CitiesList()
        {
            var cities = _dbContext.Cities.Include(i => i.Country)
                                          .ToList();

            return PartialView("CitiesList", cities);
        }

        [HttpPost]
        [Route("AddCity")]
        public IActionResult AddCity([FromBody] City city)
        {
            if (city == null || string.IsNullOrEmpty(city.TitleEn) || string.IsNullOrEmpty(city.TitleAr))
            {
                return BadRequest("Please fill all fields.");
            }

            bool isDuplicate = _dbContext.Cities
                                         .Any(w => (w.TitleEn == city.TitleEn && w.TitleAr == city.TitleAr)
                                         && w.CountryId == city.CountryId);
            if (isDuplicate)
            {
                return BadRequest("The details for the City have already been added.");
            }

          

            return Ok();
        }


        [HttpPost]
        [Route("UpdateCity")]
        public IActionResult UpdateCity([FromBody] City city)
        {
            if (city == null || string.IsNullOrEmpty(city.TitleEn) || string.IsNullOrEmpty(city.TitleAr))
            {
                return BadRequest("Please fill all fields.");
            }

            bool isDuplicate = _dbContext.Cities
                                        .Any(w => (w.TitleEn == city.TitleEn && w.TitleAr == city.TitleAr)
                                        && w.CountryId == city.CountryId);
            if (isDuplicate)
            {
                return BadRequest("The details for the City have already been added.");
            }

            else
            {
                if (!string.IsNullOrEmpty(city.TitleAr) || !string.IsNullOrEmpty(city.TitleEn))
                {
                    _dbContext.Cities.Update(city);
                    _dbContext.SaveChanges();
                }
            }

            return Ok();
        }


        [HttpGet]
        [Route("DeleteCity/{id}")]
        public IActionResult DeleteCity(int id)
        {
            // Retrieve the practitioner type from the database using the id
            var city = _dbContext.Cities.Find(id);
            if (city == null)
            {
                // Handle the case where the practitioner type doesn't exist
                TempData["isSuccessDelete"] = false;
                TempData["isFromDeleteRequest"] = true;

                return RedirectToAction("Cities");
            }

            // Remove the practitioner type from the DbSet
            _dbContext.Cities.Remove(city);

            // Save the changes to the database
            _dbContext.SaveChanges();
            TempData["isSuccessDelete"] = true;
            // Set the value in TempData
            TempData["isFromDeleteRequest"] = true;
            return RedirectToAction("Cities");
        }


        [HttpGet]
        [Route("GetCity/{id}")]
        public City GetCity(int id)
        {
            var city = _dbContext.Cities.Single(w => w.Id == id);
            return city;
        }
        #endregion

        #region CustomChangePassword
        [Route("CustomChangePassword")]
        [Route("MyProfile/ChangePassword")]
        public ActionResult CustomChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Route("UpdatePassword")]
        public void UpdatePassword(string oldPassword, string newPassword)
        {
            bool? isSuccessDelete = TempData["isSuccessDelete"] as bool?;
            var user = _userManager.GetUserAsync(User).Result;
            var changePasswordResult = _userManager.ChangePasswordAsync(user, oldPassword, newPassword).Result;
            if (!changePasswordResult.Succeeded)
            {
                ViewBag.SuccessMessage = isSuccessDelete;
            }
            ViewBag.SuccessMessage = isSuccessDelete;
        }
        #endregion

        #region Notifications
        [Route("MyProfile/Notifications")]
        public IActionResult Notifications()
        {
            var currentUserId = GetUserProfileId();
            var model = _dbContext.Notifications.Where(a => a.AssignedToUserId == currentUserId)
                                                .OrderByDescending(a => a.CreationDate)
                                                .Take(100)
                                                .ToList();

            // Set the culture for date formatting
            var georgianCulture = new CultureInfo("en-US");
            foreach (var notification in model)
            {
                // Assuming CreationDate is a DateTime property in your model
                notification.CreationDate = notification.CreationDate.ToLocalTime(); // Convert to local time if necessary
                notification.CreationDateFormatted = notification.CreationDate.ToString("G", georgianCulture);
            }


            return View("Notifications", model);
        }
        #endregion
    }
}
