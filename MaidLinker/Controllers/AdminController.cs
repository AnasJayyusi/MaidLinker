using MaidLinker.Data;
using MaidLinker.Data.Entites;
using MaidLinker.Hubs;
using MaidLinker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using NPOI.SS.Formula.Functions;
using System.Globalization;



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


        #endregion


        #region Maids
        [Route("MaidsManagement/Maids")]
        public IActionResult Maids()
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

            return View(_dbContext.Maids.ToList());
        }

        [Route("GetMaids")]
        public IActionResult MaidsList()
        {
            var result = _dbContext.Maids.ToList();
            return PartialView("MaidsList", result);
        }

        [HttpPost]
        [Route("AddMaid")]
        public async Task<IActionResult> AddMaid([FromForm] MaidDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var maid = new Maid
            {
                FirstNameEn = dto.FirstNameEn,
                SecondNameEn = dto.SecondNameEn,
                ThirdNameEn = dto.ThirdNameEn,
                LastNameEn = dto.LastNameEn,

                FirstNameAr = dto.FirstNameAr,
                SecondNameAr = dto.SecondNameAr,
                ThirdNameAr = dto.ThirdNameAr,
                LastNameAr = dto.LastNameAr,

                DateOfBirth = dto.DateOfBirth,
                TotalExperience = dto.TotalExperience,
                MaritalStatus = dto.MaritalStatus,
                Childs = dto.Childs,
                Note = dto.Note,
                NationalityId = dto.NationalityId,
                VideoURL = dto.VideoURL,

                // Assuming you fetch countries/languages from DB
                ServedCountries = await _dbContext.Countries.Where(c => dto.ServedCountryIds.Contains(c.Id)).ToListAsync(),
                Langauges = await _dbContext.Languages.Where(l => dto.LanguageIds.Contains(l.Id)).ToListAsync(),
            };

            // Handle Image Upload
            if (dto.ImagePath != null && dto.ImagePath.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImagePath.FileName);
                var filePath = Path.Combine("wwwroot/uploads/maids", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImagePath.CopyToAsync(stream);
                }

                maid.ImagePath = "/uploads/maids/" + fileName;
            }

            _dbContext.Maids.Add(maid);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Maid created successfully." });
        }

        [HttpGet]
        [Route("GetMaid/{id}")]
        public JsonResult GetMaid(int id)
        {
            var maid = _dbContext.Maids.Include(i=>i.Nationality)
                                       .Include(i=>i.ServedCountries)
                                       .Include(i=>i.Langauges)
                                       .Where(w=>w.Id == id)
                                       .Single(); 
            if (maid == null)
            {
                return null;
            }

            var result = new
            {
                Id=maid.Id,
                firstNameEn = maid.FirstNameEn,
                secondNameEn = maid.SecondNameEn,
                thirdNameEn = maid.ThirdNameEn,
                lastNameEn = maid.LastNameEn,
                firstNameAr = maid.FirstNameAr,
                secondNameAr = maid.SecondNameAr,
                thirdNameAr = maid.ThirdNameAr,
                lastNameAr = maid.LastNameAr,
                dateOfBirth = maid.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                totalExperience = maid.TotalExperience,
                maritalStatusId = maid.MaritalStatus,
                childs = maid.Childs,
                nationalityId = maid.NationalityId,
                servedCountryIds = maid.ServedCountries.Select(s=>s.Id), 
                languageIds = maid.Langauges.Select(s=>s.Id),          
                note = maid.Note,
                videoURL = maid.VideoURL,
                imagePath=maid.ImagePath
            };

            return Json(result);
        }

        [HttpGet]
        [Route("DeleteMaid/{id}")]
        public IActionResult DeleteMaid(int id)
        {
            // Retrieve the obj from the database using the id
            var obj = _dbContext.Maids.Find(id);


            if (obj == null)
            {
                // Handle the case where the obj type doesn't exist
                TempData["isSuccessDelete"] = false;
            }

            // Remove the practitioner type from the DbSet
            _dbContext.Maids.Remove(obj);

            // Save the changes to the database
            _dbContext.SaveChanges();
            TempData["isSuccessDelete"] = true;


            // Set the value in TempData
            TempData["isFromDeleteRequest"] = true;
            return RedirectToAction("Maids");
        }

        [HttpPost]
        [Route("UpdateMaid")]
        public async Task<IActionResult> UpdateMaid([FromForm] MaidDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var maid = await _dbContext.Maids
                .Include(m => m.Nationality)
                .Include(m => m.ServedCountries)
                .Include(m => m.Langauges)
                .FirstOrDefaultAsync(m => m.Id == dto.Id);

            if (maid == null)
                return NotFound("Maid not found.");

            // Update simple properties
            maid.FirstNameEn = dto.FirstNameEn;
            maid.SecondNameEn = dto.SecondNameEn;
            maid.ThirdNameEn = dto.ThirdNameEn;
            maid.LastNameEn = dto.LastNameEn;

            maid.FirstNameAr = dto.FirstNameAr;
            maid.SecondNameAr = dto.SecondNameAr;
            maid.ThirdNameAr = dto.ThirdNameAr;
            maid.LastNameAr = dto.LastNameAr;

            maid.DateOfBirth = dto.DateOfBirth;
            maid.TotalExperience = dto.TotalExperience;
            maid.MaritalStatus = dto.MaritalStatus;
            maid.Childs = dto.Childs;
            maid.Note = dto.Note;
            maid.NationalityId = dto.NationalityId;
            maid.VideoURL = dto.VideoURL;

            // Update many-to-many relationships
            // ServedCountries
            maid.ServedCountries.Clear();
            var countries = await _dbContext.Countries.Where(c => dto.ServedCountryIds.Contains(c.Id)).ToListAsync();
            foreach (var country in countries)
                maid.ServedCountries.Add(country);

            // Languages
            maid.Langauges.Clear();
            var languages = await _dbContext.Languages.Where(l => dto.LanguageIds.Contains(l.Id)).ToListAsync();
            foreach (var lang in languages)
                maid.Langauges.Add(lang);

            // Handle Image upload if present
            if (dto.ImagePath != null && dto.ImagePath.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/maids");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImagePath.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImagePath.CopyToAsync(stream);
                }

                // Optionally delete old image file if exists
                if (!string.IsNullOrEmpty(maid.ImagePath))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, maid.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                maid.ImagePath = "/uploads/maids/" + fileName;
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Maid updated successfully." });
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
