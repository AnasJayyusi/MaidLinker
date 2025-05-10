using MaidLinker.Data;
using MaidLinker.Data.Entites;
using MaidLinker.Enums;
using MaidLinker.Hubs;
using MaidLinker.Models;
using MaidLinker.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MaidLinker.Dto;
using static MaidLinker.Data.SharedEnum;
using System.Globalization;
using Microsoft.AspNetCore.Identity;

namespace MaidLinker.Controllers
{


    [Route("ServiceProvider")]
    [Authorize(Roles = "ServiceProvider,Beneficiary")]
    public class ServiceProviderController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;


        public ServiceProviderController(
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext dbContext,
            INotificationService notificationService,
            UserManager<IdentityUser> userManager) : base(dbContext, notificationService)
        {
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [Route("Profile")]
        public IActionResult Profile()
        {
            SetImagePathInCookies();
            return View();
        }
        #region Referrals
        [Route("Referrals")]
        public IActionResult Referrals()
        {
            return View();
        }


        [Route("Notifications")]
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

        [Route("CustomChangePassword")]
        public ActionResult CustomChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Route("UpdatePassword")]
        public ActionResult UpdatePassword(string oldPassword, string newPassword)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var changePasswordResult = _userManager.ChangePasswordAsync(user, oldPassword, newPassword).Result;
            if (!changePasswordResult.Succeeded)
            {
                return BadRequest(new { error = "Failed to update password." });
            }
            return Ok(new { message = "Password updated successfully." });
        }

        //[Route("GetIncomingRequests")]
        //public ActionResult GetIncomingRequests(string? name, int? statusId = 0, int? dateId = 0)
        //{
        //    var currentUserId = GetUserProfileId();
        //    if (string.IsNullOrEmpty(name))
        //        name = string.Empty;
        //    var model = _dbContext.ReferralRequests
        //                          .Include(a => a.Order)
        //                          .Include(a => a.Order.Services)
        //                          .Include(a => a.CreatedByUser)
        //                          .Include(a => a.AssignedToUser)
        //                          .Where(a =>
        //                                (a.CreatedByUser.FullName.Contains(name) || name == string.Empty)
        //                                && (statusId == 0 || (int)a.Status == statusId)
        //                                && (dateId == 0 ||
        //                                (dateId == 1 && a.CreationDate.Date == DateTime.Now.Date) ||
        //                                (dateId == 2 && a.CreationDate >= DateTime.Now.AddDays(-7).Date) ||
        //                                (dateId == 3 && a.CreationDate >= DateTime.Now.AddDays(-30).Date)
        //                                )
        //                                && a.AssignedToUserId == currentUserId).OrderByDescending(a => a.CreationDate).AsSplitQuery()
        //                                .ToList();

        //    return PartialView("IncomingRequests", model);
        //}

        //[Route("GetOutgoingRequests")]
        //public ActionResult GetOutgoingRequests(string? name, int? statusId = 0, int? dateId = 0)
        //{
        //    var currentUserId = GetUserProfileId();
        //    if (string.IsNullOrEmpty(name))
        //        name = string.Empty;
        //    var model = _dbContext.ReferralRequests
        //                          .Include(a => a.Order)
        //                          .Include(a => a.Order.Services)
        //                          .Include(a => a.CreatedByUser)
        //                          .Include(a => a.AssignedToUser)
        //                          .Where(a =>
        //                          (a.AssignedToUser.FullName.Contains(name) || name == string.Empty)
        //                          && (statusId == 0 || (int)a.Status == statusId)
        //                          && (dateId == 0 ||
        //                          (dateId == 1 && a.CreationDate.Date == DateTime.Now.Date) ||
        //                          (dateId == 2 && a.CreationDate >= DateTime.Now.AddDays(-7).Date) ||
        //                          (dateId == 3 && a.CreationDate >= DateTime.Now.AddDays(-30).Date)
        //                          )
        //                          && a.CreatedByUserId == currentUserId).OrderByDescending(a => a.CreationDate).AsSplitQuery()
        //                          .ToList();

        //    return PartialView("OutgoingRequests", model);
        //}






        #endregion






        // If want use this in doctor page please move to other controller with anoynomus and other name
        //    #region
        //    [HttpGet]
        //    [Route("GetUserProfileInfoByUseId")]
        //    public ActionResult GetUserProfileInfoByUseId(string userId)
        //    {

        //        var userProfile = _dbContext.UserProfiles.FirstOrDefault(w => w.UserId == userId);
        //        var specialityNames = "";
        //        // Your logic to retrieve the necessary data
        //        if (userProfile.SpecialtiesIds != null)
        //        {
        //            List<int> specialityIds = userProfile.SpecialtiesIds.Split(',').Select(int.Parse).ToList();

        //            // This Code Temp We Should Depnd On Select2 
        //            if (specialityIds != null)
        //            {
        //                var specialities = _dbContext.Specialties
        //                               .Where(t => specialityIds.Contains(t.Id)).Select(a => new
        //                               {
        //                                   a.TitleEn
        //                               }).ToList();
        //                if (specialities.Any())
        //                    specialityNames = String.Join(",", specialities.Select(a => a.TitleEn));
        //            }
        //        }

        //        var data = new
        //        {
        //            fullname = userProfile?.FullName,
        //            numberPatients = userProfile.NumOfPatients ?? 0,
        //            review = userProfile.Reviews ?? 0,
        //            insurance = userProfile.InsuranceAccepted ?? false,
        //            bio = userProfile.Bio ?? "No Bio yet.",
        //            speciality = specialityNames ?? "No Speciality added yet.",
        //            profilePicturePath = userProfile.ProfilePicturePath,
        //            profileStatus = userProfile.ProfileStatus.ToString(),
        //            rejectionReason = userProfile.RejectionReason
        //        };

        //        return Json(data);
        //    }
        //    #endregion
        //    [HttpGet]
        //    [Route("GetFullUserProfileInfo")]
        //    public ActionResult GetFullUserProfileInfo()
        //    {
        //        string cultureCode = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        //        string direction = cultureCode.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "rtl" : "ltr";
        //        bool isEng = direction == "ltr" ? true : false;

        //        _loggedAspNetUserId = GetAspNetUserId();
        //        var userProfile = _dbContext.UserProfiles.Include(i => i.State).Include(i => i.City).FirstOrDefault(w => w.UserId == GetAspNetUserId());

        //        // Your logic to retrieve the necessary data
        //        var data = new
        //        {
        //            FullName = userProfile?.FullName,
        //            Bio = userProfile?.Bio,
        //            AccountTypeId = userProfile.AccountTypeId,
        //            PhoneNumber = userProfile.PhoneNumber,
        //            Email = userProfile.Email,
        //            StateName = isEng ? userProfile?.State?.TitleEn : userProfile?.State?.TitleAr,
        //            CityName = isEng ? userProfile?.City?.TitleEn : userProfile?.City?.TitleAr,
        //            ProfilePicturePath = userProfile.ProfilePicturePath,

        //        };

        //        return Json(data);
        //    }

        //    [HttpGet]
        //    [Route("GetUserProfessionalRanksDDL")]
        //    public ActionResult GetUserProfessionalRanksDDL()
        //    {
        //        _loggedAspNetUserId = GetAspNetUserId();
        //        var practitionerTypeId = _dbContext.UserProfiles
        //                                            .Where(u => u.UserId == GetAspNetUserId())
        //                                            .Select(u => u.PractitionerTypeId);

        //        var professionalRanks = _dbContext.ProfessionalRanks
        //                                           .Where(w => w.IsActive && practitionerTypeId.Contains(w.PractitionerTypeId))
        //                                           .Select(s => new { Id = s.Id, TitleEn = s.TitleEn, TitleAr = s.TitleAr })
        //                                           .ToList();
        //        return Json(professionalRanks);
        //    }

        //    [HttpGet]
        //    [Route("GetSelectedSpecialties")]
        //    public ActionResult GetSpecialtiesSelectedOptions()
        //    {
        //        _loggedAspNetUserId = GetAspNetUserId();

        //        var selectedOptions = _dbContext.UserProfiles.Where(w => w.UserId == _loggedAspNetUserId)
        //                                    .Select(s => new { id = s.Id, text = s.SpecialtiesTitlesAr, textAr = s.SpecialtiesTitlesEn });

        //        return Json(selectedOptions);
        //    }

        //    [HttpGet]
        //    [Route("GetCountriesDDL")]
        //    public ActionResult GetCountriesDDL()
        //    {
        //        var countries = _dbContext.Countries.ToList();
        //        return Json(countries);
        //    }

        //    [HttpGet]
        //    [Route("GetStatesDDL")]
        //    public ActionResult GetStatesDDL(int countryId)
        //    {
        //        var countries = _dbContext.States.Where(w => w.CountryId == countryId).ToList();
        //        return Json(countries);
        //    }

        //    [Route("GetCitiesDDL")]
        //    public ActionResult GetCitiesDDL(int stateId)
        //    {
        //        var cities = _dbContext.Cities.Where(w => w.StateId == stateId).ToList();
        //        return Json(cities);
        //    }


        //    [Route("GetDistricts")]
        //    public ActionResult GetDistricts(int cityId)
        //    {
        //        var districts = _dbContext.Districts.Where(w => w.CityId == cityId).ToList();
        //        return Json(districts);
        //    }

        //    [HttpGet]
        //    [Route("SendProfileToReview")]
        //    public ActionResult SendProfileToReview()
        //    {
        //        var userProfile = _dbContext.UserProfiles.Single(w => w.UserId == GetAspNetUserId());
        //        userProfile.ProfileStatus = ProfileStatus.UnderReview;
        //        _dbContext.UserProfiles.Update(userProfile);
        //        _dbContext.SaveChanges();
        //        PushNewNotification(SharedEnum.NotificationTypeEnum.SendProfileToReview, GetUserProfileId(), _adminUserProfileId, GetShortName());
        //        return Json(ProfileStatus.UnderReview.ToString());
        //    }

        //    [HttpGet]
        //    [Route("CanSendProfile")]
        //    public ActionResult CanSendProfile()
        //    {
        //        var userId = GetAspNetUserId();

        //        var generalsettings = _dbContext.GeneralSettings.FirstOrDefault();
        //        bool isCertifactionsAttachmnetsRequired = generalsettings.IsCertifactionsAttachmnetsRequired;
        //        bool isRequiredAttachmnetsUploaded, isCertifactionsAttachmnetsUploaded, isAgree;

        //        if (generalsettings.IsProfessionalCategoryRequired)
        //            isRequiredAttachmnetsUploaded = _dbContext.RequiredAttachments.Where(w => w.UserId == userId).Count() >= 1;
        //        else
        //            isRequiredAttachmnetsUploaded = true;

        //        if (generalsettings.IsSignedContractRequired)
        //            isAgree = _dbContext.UserProfiles.Single(w => w.UserId == userId).IsAgree;
        //        else
        //            isAgree = true;


        //        if (generalsettings.IsCertifactionsAttachmnetsRequired)
        //            isCertifactionsAttachmnetsUploaded = _dbContext.Certifications.Where(w => w.UserId == userId).Count() >= 1;
        //        else
        //            isCertifactionsAttachmnetsUploaded = true;


        //        if (isRequiredAttachmnetsUploaded && isCertifactionsAttachmnetsUploaded && isAgree)
        //            return Json(true);
        //        return Json(true);
        //    }

        //    [HttpPost]
        //    [Route("UpdateUserProfile")]
        //    public IActionResult UpdateUserProfile(IFormCollection form)
        //    {
        //        _loggedAspNetUserId = GetAspNetUserId();
        //        var userProfile = _dbContext.UserProfiles.Single(w => w.UserId == GetAspNetUserId());

        //        // Handle image file
        //        var imageFile = form.Files["image"];

        //        // Access form data & fill to user profile object
        //        userProfile.FullName = form["FullName"];
        //        userProfile.Bio = form["Bio"];

        //        if (!string.IsNullOrEmpty(form["ProfessionalRankId"]))
        //            userProfile.ProfessionalRankId = Convert.ToInt32(form["ProfessionalRankId"]);

        //        if (!string.IsNullOrEmpty(form["CountryId"]))
        //            userProfile.CountryId = Convert.ToInt32(form["CountryId"]);

        //        if (!string.IsNullOrEmpty(form["StateId"]))
        //            userProfile.StateId = Convert.ToInt32(form["StateId"]);

        //        if (!string.IsNullOrEmpty(form["CityId"]))
        //            userProfile.CityId = Convert.ToInt32(form["CityId"]);

        //        if (!string.IsNullOrEmpty(form["SpecialtiesIds"]))
        //            userProfile.SpecialtiesIds = form["SpecialtiesIds"];
        //        else
        //            userProfile.SpecialtiesIds = null;

        //        if (!string.IsNullOrEmpty(userProfile.SpecialtiesIds))
        //        {
        //            var specialtiesIds = userProfile.SpecialtiesIds.Split(',')
        //                               .Select(int.Parse)
        //                               .ToList();
        //            userProfile.SpecialtiesTitlesAr = string.Join(",", _dbContext.Specialties.Where(w => specialtiesIds.Contains(w.Id)).Select(x => x.TitleAr));
        //            userProfile.SpecialtiesTitlesEn = string.Join(",", _dbContext.Specialties.Where(w => specialtiesIds.Contains(w.Id)).Select(x => x.TitleEn));
        //        }




        //        if (imageFile != null)
        //            userProfile.ProfilePicturePath = StoreImageFilePathInDatabase(imageFile);

        //        _dbContext.UserProfiles.Update(userProfile);
        //        _dbContext.SaveChanges();
        //        return Ok("Updated successfully.");
        //    }

        //    #region Certifications
        //    [HttpGet]
        //    [Route("GetCertifications")]
        //    public ActionResult GetCertifications()
        //    {
        //        var userId = GetUserProfileId();
        //        var attachments = _dbContext.Certifications.Include(i => i.Degree).Where(w => w.UserProfileId == userId).ToList();
        //        return Json(attachments);
        //    }
        //    private string StoreImageFilePathInDatabase(IFormFile profileImage)
        //    {
        //        // File extension
        //        string extension = Path.GetExtension(profileImage.FileName);
        //        // Get the root path of the web application
        //        string webRootPath = _webHostEnvironment.WebRootPath;

        //        // Define the relative path to the image directory within the wwwroot folder
        //        string imagePath = Path.Combine("users", "images");

        //        // Create the full path to save the image
        //        string uploadPath = Path.Combine(webRootPath, imagePath);

        //        // Create the directory if it doesn't exist
        //        Directory.CreateDirectory(uploadPath);

        //        // Generate a unique filename for the image (you can use your own logic here)
        //        string uniqueFileName = GetAspNetUserId();

        //        // Combine the upload path with the unique filename
        //        string filePath = Path.Combine(uploadPath, uniqueFileName) + extension;

        //        // Save the image file
        //        using (var imageStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            profileImage.CopyTo(imageStream);
        //        }

        //        // Path To Save In Database
        //        return $"/users/images/{uniqueFileName}{extension}";
        //    }

        //    [HttpGet]
        //    [Route("GetDegrees")]
        //    public ActionResult GetDegrees()
        //    {
        //        var degrees = _dbContext.Degrees.Where(w => w.IsActive).ToList();
        //        return Json(degrees);
        //    }
        //    #endregion



        //    [HttpPost]
        //    [Route("UpdateTimeClinicLocation")]
        //    public IActionResult UpdateTimeClinicLocation(IFormCollection form)
        //    {
        //        var userProfileId = GetUserProfileId();
        //        bool isAddOperation = false;
        //        var obj = _dbContext.TimeClinicLocations.SingleOrDefault(s => s.UserProfileId == userProfileId);

        //        if (obj == null)
        //        {
        //            obj = new TimeClinicLocation();
        //            isAddOperation = true;
        //            obj.UserProfileId = GetUserProfileId();
        //        }

        //        if (!string.IsNullOrEmpty(form["ClinicName"]))
        //            obj.ClinicName = form["ClinicName"];

        //        // Access form data & fill to user profile object
        //        if (!string.IsNullOrEmpty(form["CountryId"]))
        //            obj.CountryId = Convert.ToInt32(form["CountryId"]);

        //        if (!string.IsNullOrEmpty(form["StateId"]))
        //            obj.StateId = Convert.ToInt32(form["StateId"]);

        //        if (!string.IsNullOrEmpty(form["CityId"]))
        //            obj.CityId = Convert.ToInt32(form["CityId"]);

        //        if (!string.IsNullOrEmpty(form["DistrictId"]))
        //            obj.DistrictId = Convert.ToInt32(form["DistrictId"]);

        //        if (!string.IsNullOrEmpty(form["SundayOpenAt"]) && !string.IsNullOrEmpty(form["SundayClosedAt"]) && form["SundayIsClosed"] != "true")
        //        {
        //            obj.SundayOpenAt = form["SundayOpenAt"];
        //            obj.SundayClosedAt = form["SundayClosedAt"];
        //            obj.SundayIsClosed = false;
        //        }
        //        else
        //        {
        //            obj.SundayIsClosed = form["SundayIsClosed"] == "true" ? true : false;
        //        }

        //        if (!string.IsNullOrEmpty(form["MondayOpenAt"]) && !string.IsNullOrEmpty(form["MondayClosedAt"]) && form["MondayIsClosed"] != "true")
        //        {
        //            obj.MondayOpenAt = form["MondayOpenAt"];
        //            obj.MondayClosedAt = form["MondayClosedAt"];
        //            obj.MondayIsClosed = false;
        //        }
        //        else
        //        {
        //            obj.MondayIsClosed = form["MondayIsClosed"] == "true" ? true : false;
        //        }

        //        if (!string.IsNullOrEmpty(form["TuesdayOpenAt"]) && !string.IsNullOrEmpty(form["TuesdayClosedAt"]) && form["TuesdayIsClosed"] != "true")
        //        {
        //            obj.TuesdayOpenAt = form["TuesdayOpenAt"];
        //            obj.TuesdayClosedAt = form["TuesdayClosedAt"];
        //            obj.TuesdayIsClosed = false;
        //        }
        //        else
        //        {
        //            obj.TuesdayIsClosed = form["TuesdayIsClosed"] == "true" ? true : false;
        //        }

        //        if (!string.IsNullOrEmpty(form["WednesdayOpenAt"]) && !string.IsNullOrEmpty(form["WednesdayClosedAt"]) && form["WednesdayIsClosed"] != "true")
        //        {
        //            obj.WednesdayOpenAt = form["WednesdayOpenAt"];
        //            obj.WednesdayClosedAt = form["WednesdayClosedAt"];
        //            obj.WednesdayIsClosed = false;
        //        }
        //        else
        //        {
        //            obj.WednesdayIsClosed = form["WednesdayIsClosed"] == "true" ? true : false;
        //        }

        //        if (!string.IsNullOrEmpty(form["ThursdayOpenAt"]) && !string.IsNullOrEmpty(form["ThursdayClosedAt"]) && form["ThursdayIsClosed"] != "true")
        //        {
        //            obj.ThursdayOpenAt = form["ThursdayOpenAt"];
        //            obj.ThursdayClosedAt = form["ThursdayClosedAt"];
        //            obj.ThursdayIsClosed = false;
        //        }
        //        else
        //        {
        //            obj.ThursdayIsClosed = form["ThursdayIsClosed"] == "true" ? true : false;
        //        }

        //        if (!string.IsNullOrEmpty(form["FridayOpenAt"]) && !string.IsNullOrEmpty(form["FridayClosedAt"]) && form["FridayIsClosed"] != "true")
        //        {
        //            obj.FridayOpenAt = form["FridayOpenAt"];
        //            obj.FridayClosedAt = form["FridayClosedAt"];
        //            obj.FridayIsClosed = false;
        //        }
        //        else
        //        {
        //            obj.FridayIsClosed = form["FridayIsClosed"] == "true" ? true : false;
        //        }

        //        if (!string.IsNullOrEmpty(form["SaturdayOpenAt"]) && !string.IsNullOrEmpty(form["SaturdayClosedAt"]) && form["SaturdayIsClosed"] != "true")
        //        {
        //            obj.SaturdayOpenAt = form["SaturdayOpenAt"];
        //            obj.SaturdayClosedAt = form["SaturdayClosedAt"];
        //            obj.SaturdayIsClosed = false;
        //        }
        //        else
        //        {
        //            obj.SaturdayIsClosed = form["SaturdayIsClosed"] == "true" ? true : false;
        //        }


        //        if (isAddOperation)
        //            _dbContext.TimeClinicLocations.Add(obj);
        //        else
        //            _dbContext.TimeClinicLocations.Update(obj);
        //        _dbContext.SaveChanges();
        //        return Ok("Updated successfully.");
        //    }

        //    [HttpGet]
        //    [Route("GetTimeClinicLocation")]
        //    public ActionResult GetTimeClinicLocation()
        //    {
        //        string cultureCode = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        //        string direction = cultureCode.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "rtl" : "ltr";
        //        bool isEng = direction == "ltr" ? true : false;

        //        var userProfileId = GetUserProfileId();
        //        var timeClinicLocation = _dbContext.TimeClinicLocations.
        //                                                         Include(i => i.State)
        //                                                        .Include(i => i.Country)
        //                                                        .Include(i => i.City)
        //                                                        .Include(i => i.Districts)
        //                                                        .FirstOrDefault(w => w.UserProfileId == userProfileId);
        //        if (timeClinicLocation != null)
        //        {
        //            // Your logic to retrieve the necessary data
        //            var data = new
        //            {
        //                timeClinicLocation.ClinicName,
        //                timeClinicLocation.SundayOpenAt,
        //                timeClinicLocation.SundayClosedAt,
        //                timeClinicLocation.SundayIsClosed,
        //                timeClinicLocation.MondayOpenAt,
        //                timeClinicLocation.MondayClosedAt,
        //                timeClinicLocation.MondayIsClosed,
        //                timeClinicLocation.TuesdayOpenAt,
        //                timeClinicLocation.TuesdayClosedAt,
        //                timeClinicLocation.TuesdayIsClosed,
        //                timeClinicLocation.WednesdayOpenAt,
        //                timeClinicLocation.WednesdayClosedAt,
        //                timeClinicLocation.WednesdayIsClosed,
        //                timeClinicLocation.ThursdayOpenAt,
        //                timeClinicLocation.ThursdayClosedAt,
        //                timeClinicLocation.ThursdayIsClosed,
        //                timeClinicLocation.FridayOpenAt,
        //                timeClinicLocation.FridayClosedAt,
        //                timeClinicLocation.FridayIsClosed,
        //                timeClinicLocation.SaturdayOpenAt,
        //                timeClinicLocation.SaturdayClosedAt,
        //                timeClinicLocation.SaturdayIsClosed,
        //                timeClinicLocation.CountryId,
        //                timeClinicLocation.StateId,
        //                timeClinicLocation.CityId,
        //                timeClinicLocation.DistrictId,
        //                CountryName = isEng ? timeClinicLocation?.Country?.TitleEn : timeClinicLocation?.Country?.TitleAr,
        //                StateName = isEng ? timeClinicLocation?.State?.TitleEn : timeClinicLocation?.State?.TitleAr,
        //                CityName = isEng ? timeClinicLocation?.City?.TitleEn : timeClinicLocation?.City?.TitleAr,
        //                DistrictName = isEng ? timeClinicLocation?.Districts?.TitleEn : timeClinicLocation?.Districts?.TitleAr
        //            };
        //            return Json(data);
        //        }
        //        return Json("");
        //    }


        //    [HttpGet]
        //    [Route("AddInsuranceCompany")]
        //    public ActionResult AddInsuranceCompany(string companyId)
        //    {
        //        int id = Convert.ToInt32(companyId);

        //        var userProfileId = GetUserProfileId();

        //        var userCompany = new UserCompany();
        //        userCompany.UserProfileId = userProfileId;
        //        userCompany.InsuranceCompanyId = id;

        //        var uc = _dbContext.UserCompanies.FirstOrDefault(w => w.UserProfileId == userProfileId && w.InsuranceCompanyId == id);

        //        var user = _dbContext.UserProfiles.Single(s => s.Id == userProfileId);
        //        user.InsuranceAccepted = true;
        //        _dbContext.UserProfiles.Update(user);

        //        if (uc == null)
        //        {
        //            _dbContext.UserCompanies.Add(userCompany);
        //            _dbContext.SaveChanges();
        //            return Json("Added successfully.");
        //        }
        //        else
        //            return Json("Already Exists.");
        //    }


        //    [HttpGet]
        //    [Route("GetUserCompanies")]
        //    public ActionResult GetUserCompanies()
        //    {
        //        var userProfileId = GetUserProfileId();
        //        var result = _dbContext.UserCompanies
        //                               .Include(i => i.Company)
        //                               .Where(w => w.UserProfileId == userProfileId).Select(s => s.Company).ToList();
        //        return Json(result);
        //    }


        //    [HttpPost]
        //    [Route("DeleteInsuranceCompany")]
        //    public ActionResult DeleteInsuranceCompany(int companyId)
        //    {
        //        var userProfileId = GetUserProfileId();
        //        var obj = _dbContext.UserCompanies.Single(w => w.InsuranceCompanyId == companyId && w.UserProfileId == userProfileId);
        //        _dbContext.UserCompanies.Remove(obj);
        //        _dbContext.SaveChanges();

        //        var isAnyCompanyAdded = _dbContext.UserCompanies.Any(w => w.UserProfileId == userProfileId);
        //        var user = _dbContext.UserProfiles.Single(s => s.Id == userProfileId);
        //        if (isAnyCompanyAdded)
        //        {
        //            user.InsuranceAccepted = true;
        //        }
        //        else
        //            user.InsuranceAccepted = false;

        //        _dbContext.UserProfiles.Update(user);
        //        _dbContext.SaveChanges();
        //        return Json("Deleted Successfully");
        //    }



        //    [Route("GetAvailablePrivileges")]
        //    public ActionResult GetAvailablePrivileges()
        //    {
        //        // Assuming you have a list of items to pass to the view
        //        var currentUserProfileId = GetUserProfileId();
        //        var services = _dbContext.Services
        //                             .Include(i => i.ClinicalSpeciality)
        //                             .Include(i => i.ServiceLevel)
        //                             .Join(_dbContext.UserServices,
        //                                 s => s.Id,
        //                                 us => us.ServiceId,
        //                                 (s, us) => new SupportServiceModal
        //                                 {
        //                                     ServiceId = s.Id,
        //                                     TitleEn = s.TitleEn,
        //                                     TitleAr = s.TitleAr,
        //                                     Status = us.Status,
        //                                     Logo = s.ClinicalSpeciality.LogoImagePath,
        //                                     ClinicalSpecialityNameAr = s.ClinicalSpeciality.TitleAr,
        //                                     ClinicalSpecialityNameEn = s.ClinicalSpeciality.TitleEn,
        //                                     UserProfileId = us.UserId,
        //                                     ServiceLevelNameAr = s.ServiceLevel.TitleEn ?? "-",
        //                                     ServiceLevelNameEn = s.ServiceLevel.TitleEn ?? "-"
        //                                 })
        //                             .Where(us => us.Status == ServicesStatusEnum.Approved && us.UserProfileId != currentUserProfileId)
        //                             .AsSplitQuery();

        //        var supportServices = new List<SupportServiceModal>();

        //        foreach (var svc in services)
        //        {
        //            if (!supportServices.Any(x => x.ServiceId == svc.ServiceId))
        //            {
        //                supportServices.Add(svc);
        //            }
        //        }

        //        if (supportServices != null)
        //            supportServices.First().ClinicalSpecialties = supportServices.Select(s => s.ClinicalSpecialityNameEn).Distinct().ToList();


        //        // Render the partial view and return it as HTML content
        //        //string htmlContent = RenderPartialToString("_CardPartial", model); // Replace with the name of your partial view

        //        return PartialView("PrivilegesOrder", supportServices); // Replace with the name of your partial view
        //    }


        //    [Route("GetAvailableDoctors")]
        //    public ActionResult GetAvailableDoctors(string serviceIds, string name, int cityId, int disctrictId, int sortBy, int insuranceType)
        //    {

        //        var servicesIds = serviceIds?.Split(',')?.Select(Int32.Parse)?.ToList();
        //        var servicesIdLength = servicesIds.Count();
        //        var currentUserId = GetAspNetUserId();
        //        var ids = _dbContext.UserServices
        //                                .Where(a => servicesIds.Contains(a.ServiceId))
        //                                .GroupBy(us => us.UserId)
        //                                .Where(g => g.Select(us => us.ServiceId).Distinct().Count() == servicesIdLength)
        //                                .Select(g => g.Key)
        //                                .ToList();

        //        name = string.IsNullOrEmpty(name) ? string.Empty : name;

        //        var doctors = _dbContext.UserProfiles
        //                                .Include(a => a.TimeClinicLocation)
        //                                .ThenInclude(a => a.City)
        //                                .Include(x => x.Certifications)
        //                                .ThenInclude(a => a.Degree)
        //                                .Include(a => a.PractitionerType)
        //                                .Include(x => x.City)
        //                                .Where(a => a.UserId != currentUserId && a.ProfileStatus == ProfileStatus.Active && a.AccountTypeId == 1
        //                                     && (name == String.Empty || a.FullName.Contains(name))
        //                                     && (ids.Contains(a.Id))
        //                                     && (cityId == 0 || a.TimeClinicLocation.CityId == cityId)
        //                                     && (disctrictId == 0 || a.TimeClinicLocation.DistrictId == disctrictId))
        //                                    .ToList();


        //        if (insuranceType != 0)
        //        {
        //            doctors = doctors.Join(
        //              _dbContext.UserCompanies,
        //              doctor => doctor.Id,
        //              userCompany => userCompany.UserProfileId,
        //              (doctor, userCompany) => new { Doctor = doctor, UserCompany = userCompany })
        //                                    .Where(w => w.UserCompany.InsuranceCompanyId == insuranceType)
        //                                    .Select(s => s.Doctor)
        //                                    .ToList();
        //        }


        //        if (sortBy != 0)
        //        {
        //            if (sortBy == 1)
        //                doctors = doctors.OrderBy(a => a.FullName).ToList();
        //            else
        //            {
        //                doctors = doctors.OrderBy(a => a.TimeClinicLocation?.City?.TitleEn).ToList();
        //            }
        //        }


        //        return PartialView("AvailableDoctorsList", doctors); // Replace with the name of your partial view

        //    }

        //    [HttpPost]
        //    [Route("SendOrderRequest")]
        //    public IActionResult SendOrderRequest([FromBody] OrderDetailsModal orderDetailsModal)
        //    {
        //        if (orderDetailsModal == null)
        //        {
        //            return BadRequest("OrderDetail data is null.");
        //        }

        //        var servicesIds = orderDetailsModal.SelectedServicesIds?.Split(',')?.Select(Int32.Parse)?.ToList();
        //        var qty = orderDetailsModal.Quantities?.Split(',')?.Select(Int32.Parse)?.ToList();
        //        var services = _dbContext.Services.Where(a => servicesIds.Contains(a.Id)).ToList();

        //        var orderDetail = new OrderDetail()
        //        {
        //            PatientName = orderDetailsModal.FullName,
        //            PhoneNumber = orderDetailsModal.Phone,
        //            Email = string.IsNullOrEmpty(orderDetailsModal.Email) ? "" : orderDetailsModal.Email,
        //            Age = orderDetailsModal.Age != null ? (Age)Convert.ToInt32(orderDetailsModal.Age) : Age.Undefined,
        //            ChronicDisease = string.IsNullOrEmpty(orderDetailsModal.ChronicDisease) ? "" : orderDetailsModal.ChronicDisease,
        //            Services = services,
        //            DoctorId = Convert.ToInt32(orderDetailsModal.DoctorId),
        //            CountryId = string.IsNullOrEmpty(orderDetailsModal.Country) ? null : Convert.ToInt32(orderDetailsModal.Country),
        //            CityId = string.IsNullOrEmpty(orderDetailsModal.City) ? null : Convert.ToInt32(orderDetailsModal.City),
        //            StateId = string.IsNullOrEmpty(orderDetailsModal.State) ? null : Convert.ToInt32(orderDetailsModal.State)
        //        };

        //        _dbContext.OrderDetails.Add(orderDetail);
        //        var orderId = _dbContext.SaveChanges();


        //        var vatValue = _dbContext.GeneralSettings.Single().VatValue;
        //        var createdByUserId = GetUserProfileId();

        //        var referralRequest = new ReferralRequest()
        //        {
        //            Status = ReferralStatusEnum.UnderReview,
        //            CreationDate = DateTime.Now,
        //            CreatedByUserId = createdByUserId,
        //            AssignedToUserId = Convert.ToInt32(orderDetailsModal.DoctorId),
        //            OrderId = orderDetail.Id
        //        };

        //        _dbContext.ReferralRequests.Add(referralRequest);
        //        _dbContext.SaveChanges();


        //        var orderServiceDetails = new List<OrderServiceDetail>();
        //        int count = 0;
        //        foreach (var serviceId in servicesIds)
        //        {
        //            var userServiceDetails = _dbContext.UserServices
        //                                               .Include(i => i.Service)
        //                                               .Where(w => w.UserId == orderDetail.DoctorId && w.ServiceId == serviceId)
        //                                               .Single();

        //            var price = userServiceDetails.Price;
        //            var fee = userServiceDetails.Fee;

        //            var obj = new OrderServiceDetail()
        //            {
        //                OrderDetailId = orderDetail.Id,
        //                Price = price,
        //                Fee = fee,
        //                ServiceId = serviceId,
        //                TitleAr = userServiceDetails.TitleAr,
        //                TitleEn = userServiceDetails.TitleEn,
        //                Qty = qty[count],
        //            };
        //            orderServiceDetails.Add(obj);
        //            count++;
        //        }
        //        _dbContext.OrderServiceDetails.AddRange(orderServiceDetails);
        //        _dbContext.SaveChanges();

        //        var referralRequestId = referralRequest.Id;
        //        string requestNumber = referralRequestId.ToString("#0000");

        //        PushNewNotification(SharedEnum.NotificationTypeEnum.NewOrder, GetUserProfileId(), Convert.ToInt32(orderDetailsModal.DoctorId), requestNumber);


        //        // Return a response indicating success

        //        return Ok("Order Send successfully.");
        //    }


        //    [HttpGet]
        //    [Route("ExportReport")]
        //    public ActionResult ExportReport(int referralRequestId)
        //    {

        //        // Get Logo Image
        //        string imagePath = GetFileFullPath(_webHostEnvironment, "images", "logo.png");

        //        // Set Title
        //        string reportName = string.Format("Invoice" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");

        //        // Get Referral Requests 
        //        var referralReq = _dbContext.ReferralRequests.Include(i => i.Order)
        //                                             .Include(i => i.AssignedToUser)
        //                                             .Include(i => i.CreatedByUser)
        //                                             .Single(w => w.Id == referralRequestId);

        //        // Get Clinic Name
        //        var clinicName = _dbContext.TimeClinicLocations
        //                                            .Where(w => w.UserProfileId == referralReq.AssignedToUserId)
        //                                            .Select(s => s.ClinicName)
        //                                            .FirstOrDefault();


        //        // Get Order Details
        //        var orderDetails = _dbContext.OrderDetails
        //                                     .Include(i => i.Services)
        //                                     .Include(i => i.OrderServicesDetails)
        //                                     .Where(w => w.Id == referralReq.OrderId)
        //                                     .ToList();


        //        bool isServiceProviderReport = referralReq.AssignedToUserId == GetUserProfileId();
        //        bool isBeneficiaryReport = referralReq.AssignedToUserId != GetUserProfileId();

        //        if (isServiceProviderReport)
        //        {
        //            var reportFile = GetServiceProviderReport(imagePath, referralReq, clinicName);
        //            // Return File
        //            return File(reportFile, "application/pdf", reportName);
        //        }
        //        if (isBeneficiaryReport)
        //        {
        //            var reportFile = GetBeneficiaryReport(imagePath, referralReq, clinicName, orderDetails);
        //            // Return File
        //            return File(reportFile, "application/pdf", reportName);
        //        }

        //        else
        //        {
        //            var reportFile = GenericReportTemplate(imagePath, reportName, referralReq, clinicName, orderDetails);
        //            // Return File
        //            return File(reportFile, "application/pdf", reportName);
        //        }
        //    }


        //    public MemoryStream GetServiceProviderReport(string imagePath, ReferralRequest referralReq, string clinicName)
        //    {

        //        // Create Object
        //        var reportDto = new ServiceProviderReportDto();

        //        // Prepare Header 
        //        reportDto.MasterDetails = new MasterDetailsDto()
        //        {
        //            OrderDate = referralReq.CompletionDate.Value.ToString("MM/dd/yyyy"),
        //            DoctorName = referralReq.AssignedToUser.FullName,
        //            ClinicName = clinicName ?? "Not Defined Yet",
        //            InvoiceNumber = referralReq.Id.ToString("#0000"),
        //            PatientName = referralReq.Order.PatientName,
        //            RequestFrom = referralReq.CreatedByUser.FullName
        //        };


        //        var orderServiceDetails = _dbContext.OrderServiceDetails.Where(w => w.OrderDetailId == referralReq.OrderId).ToList(); // Original Data
        //        var datasource = new List<ServiceProviderDataTableDto>(); // Mapping to this object

        //        var totalServicesFee = 0.00;
        //        var totalQty = 0;
        //        var totalAmountToBeneficiary = 0.0;
        //        foreach (var item in orderServiceDetails)
        //        {
        //            if (referralReq.Status == ReferralStatusEnum.Completed || (item.IsCompleted && referralReq.Status == ReferralStatusEnum.PartiallyCompleted))
        //            {
        //                var obj = new ServiceProviderDataTableDto()
        //                {
        //                    ServiceCode = item.ServiceId.ToString(),
        //                    ServiceDesc = item.TitleEn,
        //                    Qty = item.Qty.Value,
        //                    ServiceFee = item.Fee,
        //                    Total = item.Fee * item.Qty.Value
        //                };

        //                totalServicesFee = totalServicesFee + (item.Fee * item.Qty.Value);
        //                totalQty = totalQty + item.Qty.Value;
        //                datasource.Add(obj);
        //            }
        //        }

        //        var total = new ServiceProviderDataTableDto()
        //        {
        //            ServiceCode = "#",
        //            ServiceDesc = "Total Amount",
        //            Qty = totalQty,
        //            ServiceFee = totalServicesFee,
        //            Total = totalServicesFee
        //        };

        //        datasource.Add(total);

        //        reportDto.DataTable = datasource;

        //        PdfReportGenerator reportGenerator = new PdfReportGenerator();
        //        MemoryStream reportFile = reportGenerator.GenerateReport(imagePath, reportDto, ReportTypeEnum.ServiceProviderReportDto);
        //        return reportFile;
        //    }

        //    //public MemoryStream GetBeneficiaryReport(string imagePath, object referralReq, string clinicName, List<object> orderDetails)
        //    //{
        //    //    var vatValue = 0;
        //    //    var sitePercentage = 0;

        //    //    // Create Object
        //    //    var reportDto = new BeneficiaryReportDto();

        //    //    // Prepare Header 
        //    //    reportDto.MasterDetails = new MasterDetailsDto()
        //    //    {
        //    //        OrderDate = referralReq.CompletionDate.Value.ToString("MM/dd/yyyy"),
        //    //        DoctorName = referralReq.AssignedToUser.FullName,
        //    //        ClinicName = clinicName ?? "Not Defined Yet",
        //    //        InvoiceNumber = referralReq.Id.ToString("#0000"),
        //    //        PatientName = referralReq.Order.PatientName,
        //    //        RequestFrom = referralReq.CreatedByUser.FullName
        //    //    };


        //    //    var orderServiceDetails = _dbContext.OrderServiceDetails
        //    //                                        .Where(w => w.OrderDetailId == referralReq.OrderId)
        //    //                                        .ToList(); // Original Data


        //    //    var datasource = new List<BeneficiaryDataTableDto>(); // Mapping to this object


        //    //    int totalQty = 0;
        //    //    var totalBeneficiaryPart = 0.0;
        //    //    var totalofTotal = 0.0;
        //    //    var totalPlatformFee = 0.0;
        //    //    var totalVat = 0.0;
        //    //    var totalTPFFees = 0.0;
        //    //    var totalNetDrPart = 0.0;

        //    //    foreach (var item in orderServiceDetails)
        //    //    {
        //    //        if (referralReq.Status == ReferralStatusEnum.Completed || (item.IsCompleted && referralReq.Status == ReferralStatusEnum.PartiallyCompleted))
        //    //        {
        //    //            var obj = new BeneficiaryDataTableDto()
        //    //            {
        //    //                ServiceCode = item.ServiceId.ToString(),
        //    //                ServiceDesc = item.TitleEn,
        //    //                Qty = item.Qty.Value,
        //    //                BeneficiaryPart = item.Fee,
        //    //                Total = item.Fee * item.Qty.Value,
        //    //            };
        //    //            obj.PlatformFee = sitePercentage * obj.Total;
        //    //            obj.VatPercentage = vatValue * obj.PlatformFee;
        //    //            obj.TotalPlatformFee = obj.PlatformFee + obj.VatPercentage;
        //    //            obj.NetDrPart = obj.Total - obj.TotalPlatformFee;

        //    //            datasource.Add(obj);


        //    //            totalQty = totalQty + obj.Qty;
        //    //            totalBeneficiaryPart = totalBeneficiaryPart + obj.BeneficiaryPart;
        //    //            totalofTotal = totalofTotal + obj.Total;
        //    //            totalPlatformFee = totalPlatformFee + obj.TotalPlatformFee;
        //    //            totalVat = totalVat + obj.VatPercentage;
        //    //            totalTPFFees = totalTPFFees + obj.TotalPlatformFee;
        //    //            totalNetDrPart = totalNetDrPart + obj.NetDrPart;

        //    //        }
        //    //    }

        //    //    var total = new BeneficiaryDataTableDto()
        //    //    {
        //    //        ServiceCode = "#",
        //    //        ServiceDesc = "Total Amount",
        //    //        Qty = totalQty,
        //    //        BeneficiaryPart = totalBeneficiaryPart,
        //    //        Total = totalofTotal,
        //    //    };
        //    //    total.PlatformFee = totalPlatformFee;
        //    //    total.VatPercentage = totalVat;
        //    //    total.TotalPlatformFee = totalTPFFees;
        //    //    total.NetDrPart = totalNetDrPart;

        //    //    datasource.Add(total);

        //    //    reportDto.DataTable = datasource;

        //    //    PdfReportGenerator reportGenerator = new PdfReportGenerator();
        //    //    MemoryStream reportFile = reportGenerator.GenerateReport(imagePath, reportDto, ReportTypeEnum.BeneficiaryReportDto);
        //    //    return reportFile;
        //    //}


        //    public MemoryStream GenericReportTemplate(string imagePath, string reportName, ReferralRequest referralReq, string clinicName, List<OrderDetail> orderDetails)
        //    {
        //        var reportDto = new ReportDto();
        //        reportDto.MasterDetails = new MasterDetailsDto()
        //        {
        //            OrderDate = referralReq.CompletionDate.Value.ToString("MM/dd/yyyy"),
        //            DoctorName = referralReq.AssignedToUser.FullName,
        //            ClinicName = clinicName ?? "Not Defined Yet",
        //            InvoiceNumber = referralReq.Id.ToString("#0000"),
        //            PatientName = referralReq.Order.PatientName,
        //            RequestFrom = referralReq.CreatedByUser.FullName
        //        };
        //        var vatValue = Convert.ToDouble(_dbContext.GeneralSettings.Single().VatValue);
        //        List<DataTableDto> dataSource = new List<DataTableDto>();
        //        foreach (var order in orderDetails)
        //        {
        //            int totalQty = 0;
        //            double totalUnitPrice = 0;
        //            double totalPrice = 0;
        //            double totalVatPercentage = 0;
        //            double totalVatValue = 0;
        //            double totalNetWithVat = 0;
        //            foreach (var svc in order.Services)
        //            {
        //                var orderServicesDetails = order.OrderServicesDetails.FirstOrDefault();
        //                if (referralReq.Status == ReferralStatusEnum.PartiallyCompleted && !orderServicesDetails.IsCompleted)
        //                    continue;


        //                var dataTableDto = new DataTableDto();
        //                dataTableDto.ServiceCode = svc.Id.ToString();
        //                dataTableDto.ServiceDesc = svc.TitleEn;
        //                dataTableDto.Qty = orderServicesDetails.Qty ?? 1;


        //                // Total Qty
        //                totalQty = totalQty + dataTableDto.Qty;

        //                dataTableDto.UnitPrice = _dbContext.UserServices.Single(w => w.ServiceId == svc.Id && w.UserId == referralReq.AssignedToUserId).Price;
        //                // Total totalPrice
        //                totalUnitPrice = totalUnitPrice + dataTableDto.UnitPrice;

        //                dataTableDto.Total = dataTableDto.UnitPrice * dataTableDto.Qty;
        //                // Total totalVatPercentage
        //                totalPrice = totalPrice + dataTableDto.Total;

        //                dataTableDto.VatPercentage = vatValue;
        //                // Total totalVatPercentage
        //                totalVatPercentage = totalVatPercentage + dataTableDto.VatPercentage;

        //                dataTableDto.VatValue = (dataTableDto.Total * vatValue) / 100;

        //                // Total totalVatPercentage
        //                totalVatValue = totalVatValue + dataTableDto.VatValue;

        //                dataTableDto.NetWithVat = dataTableDto.Total - dataTableDto.VatValue;

        //                // Total totalNetWithVat
        //                totalNetWithVat = totalNetWithVat + dataTableDto.NetWithVat;

        //                dataSource.Add(dataTableDto);

        //            }
        //            var total = new DataTableDto();
        //            // For Total Rows
        //            total.ServiceCode = "#";
        //            total.ServiceDesc = "Total Amount";
        //            total.Qty = totalQty;
        //            total.UnitPrice = totalUnitPrice;
        //            total.Total = totalPrice;
        //            total.VatValue = totalVatValue;
        //            total.VatPercentage = totalVatPercentage;
        //            total.NetWithVat = totalNetWithVat;
        //            dataSource.Add(total);

        //        }
        //        reportDto.DataTable = dataSource;

        //        PdfReportGenerator reportGenerator = new PdfReportGenerator();
        //        MemoryStream reportFile = reportGenerator.GenerateReport(imagePath, reportDto, ReportTypeEnum.None);
        //        return reportFile;
        //    }


        //    [HttpGet]
        //    [Route("GetReferralsOrderDetails")]
        //    public ActionResult GetReferralsOrderDetails(int orderId)
        //    {
        //        var referralsOrders = _dbContext.ReferralRequests
        //                    .Include(i => i.Order)
        //                    .Include(i => i.CreatedByUser)
        //                    .Include(i => i.AssignedToUser)
        //                    .Include(i => i.Order.Services)
        //                    .Include(i => i.Order.OrderServicesDetails)
        //                    .Select(s => new ReferralOrderDetailModal
        //                    {
        //                        OrderId = s.OrderId,
        //                        ReferralRequestId = s.Id,
        //                        ReferralRequestNumber = s.Id.ToString("#0000"),
        //                        Status = s.Status,
        //                        CreatedBy = s.CreatedByUser.FullName,
        //                        AssignedTo = s.AssignedToUser.FullName,
        //                        Date = s.CreationDate.ToString("MM/dd/yyyy"),
        //                        PatientName = s.Order.PatientName,
        //                        PhoneNumber = s.Order.PhoneNumber,
        //                        Email = s.Order.PhoneNumber,
        //                        CountryTextAr = s.Order.CountryId != null ? s.Order.Country.TitleAr : "-",
        //                        CountryTextEn = s.Order.CountryId != null ? s.Order.Country.TitleEn : "-",
        //                        StateTextAr = s.Order.StateId != null ? s.Order.State.TitleAr : "-",
        //                        StateTextEn = s.Order.StateId != null ? s.Order.State.TitleEn : "-",
        //                        CityTextAr = s.Order.CityId != null ? s.Order.City.TitleAr : "-",
        //                        CityTextEn = s.Order.CityId != null ? s.Order.City.TitleEn : "-",
        //                        Gender = s.Order.Gender,
        //                        Age = s.Order.Age,
        //                        ChronicDisease = s.Order.ChronicDisease,
        //                        ServicesRequests = s.Order.Services,
        //                        OrderServicesDetails = s.Order.OrderServicesDetails,
        //                        RejectionReason = s.RejectionReason
        //                    }).Where(w => w.OrderId == orderId);


        //        return Json(referralsOrders);
        //    }

        //    [Route("GetRequestedServices")]
        //    public ActionResult GetRequestedServices(int refId)
        //    {
        //        var requestedServices = _dbContext.ReferralRequests
        //                    .Include(i => i.Order)
        //                    .Include(i => i.Order.Services)
        //                    .Include(i => i.Order.OrderServicesDetails)
        //                    .Select(s => new ReferralOrderDetailModal
        //                    {
        //                        ReferralRequestId = s.Id,
        //                        OrderServicesDetails = s.Order.OrderServicesDetails,
        //                        ServicesRequests = s.Order.Services,
        //                    }).Where(w => w.ReferralRequestId == refId);


        //        return Json(requestedServices);
        //    }

        //    [HttpPost]
        //    [Route("CompletePartiallyReferal")]
        //    public ActionResult CompletePartiallyReferal(int refId, string inCompletedReason, [FromBody] int[] completeServicesIds)
        //    {
        //        foreach (var id in completeServicesIds)
        //        {
        //            var orderServiceDetail = _dbContext.OrderServiceDetails.Single(s => s.Id == id);
        //            orderServiceDetail.IsCompleted = true;
        //            _dbContext.SaveChanges();
        //        }


        //        var referralRequest = _dbContext.ReferralRequests.Single(w => w.Id == refId);
        //        referralRequest.Status = ReferralStatusEnum.PartiallyCompleted;
        //        referralRequest.CompletionDate = DateTime.Now;
        //        referralRequest.RejectionReason = inCompletedReason;
        //        _dbContext.SaveChanges();


        //        return Ok();
        //    }
        //}
    }
}

//public class Customer
//{
//    public int CustomerId { get; set; }
//    public string CustomerName { get; set; }
//    public string Address { get; set; }
//    public string Email { get; set; }
//    public string ZipCode { get; set; }
//}
