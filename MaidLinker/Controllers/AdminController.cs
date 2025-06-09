using MaidLinker.Data;
using MaidLinker.Data.Entites;
using MaidLinker.Dto;
using MaidLinker.Hubs;
using MaidLinker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula.Functions;
using System.Globalization;
using System.Security.Claims;
using static MaidLinker.Enums.SharedEnum;



namespace MaidLinker.Controllers
{

    [Route("Admin")]
    [Authorize(Roles = "Administrator,Reception,Accountant")]
    public class AdminController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminController(
            ApplicationDbContext dbContext,
            IWebHostEnvironment webHostEnvironment,
            INotificationService notificationService,
            UserManager<IdentityUser> userManager
            ) : base(dbContext, notificationService, userManager)
        {
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }



        #region Dashboard
        [Route("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");
            var isAccountant = User.IsInRole("Accountant");
            var isReception = User.IsInRole("Reception");

            // New requests for everyone
            var newRequests = await _dbContext.Requests
                .Include(r => r.Maid)
                .Where(r => r.Status == RequestStatus.New && r.Status != RequestStatus.Cancelled)
                .OrderBy(o => o.Id)
                .ToListAsync();

            // Filter InProgress requests based on role
            List<Request> inProgressRequests = new();
            if (isAccountant)
            {
                inProgressRequests = await _dbContext.Requests
                    .Include(r => r.Maid)
                    .Where(r => r.Status == RequestStatus.InProgress || r.Status == RequestStatus.Prepared)
                    .ToListAsync();
            }

            if (isAdmin)
            {
                inProgressRequests = await _dbContext.Requests
                    .Include(r => r.Maid)
                    .Where(r => r.Status == RequestStatus.InProgress || r.Status == RequestStatus.Prepared)
                    .ToListAsync();
            }
            else if (isReception)
            {
                inProgressRequests = await _dbContext.Requests
                    .Include(r => r.Maid)
                    .Where(r => r.Status == RequestStatus.InProgress || r.Status == RequestStatus.Prepared && r.ServedByUserId == userId)
                    .ToListAsync();
            }

            // Status Summary
            var statusCounts = new StatusCountsViewModel
            {
                New = await _dbContext.Requests.CountAsync(r => r.Status == RequestStatus.New),
                InProgress = await _dbContext.Requests.CountAsync(r => r.Status == RequestStatus.InProgress),
                Completed = await _dbContext.Requests.CountAsync(r => r.Status == RequestStatus.Completed)
            };

            var viewModel = new DashboardViewModel
            {
                NewRequests = newRequests,
                InProgressRequests = inProgressRequests,
                StatusCounts = statusCounts
            };

            return View(viewModel);
        }


        [HttpPost]
        [Route("TakeOverRequest")]
        public async Task<IActionResult> TakeOverRequest(int id)
        {
            var request = await _dbContext.Requests
                .Include(r => r.Maid)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null || request.ServedByUserId != null)
            {
                return Json(new { success = false, message = "Invalid or already taken request." });
            }

            var userId = _userManager.GetUserId(User);
            request.ServedByUserId = userId;
            request.Status = RequestStatus.InProgress;

            // Mark the maid as unavailable
            if (request.Maid != null)
            {
                request.Maid.IsAvailable = false;
            }

            await _dbContext.SaveChangesAsync();

            PushNewNotification(NotificationTypeEnum.TakeOverRequest, AccountTypeEnum.Accountant, $"#{request.Id.ToString()}");
            return Json(new { success = true });
        }


        [HttpPost]
        [Route("CancelRequest")]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var request = await _dbContext.Requests
                .Include(r => r.Maid)
                .FirstOrDefaultAsync(r => r.Id == id);

            request.Status = RequestStatus.Cancelled;

            await _dbContext.SaveChangesAsync();

            PushNewNotification(NotificationTypeEnum.Cancel, AccountTypeEnum.All, $"#{request.Id.ToString()}");
            return Json(new { success = true });
        }

        [HttpPost]
        [Route("ConfirmRequest")]
        public async Task<IActionResult> ConfirmRequest(int id)
        {
            var request = await _dbContext.Requests
                .Include(r => r.Maid)
                .FirstOrDefaultAsync(r => r.Id == id);

            request.Status = RequestStatus.Prepared;

            await _dbContext.SaveChangesAsync();
            PushNewNotification(NotificationTypeEnum.Confirm, AccountTypeEnum.All, $"#{request.Id.ToString()}");

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("CompleteRequest")]
        public async Task<IActionResult> CompleteRequest(int id)
        {
            var request = await _dbContext.Requests
                .Include(r => r.Maid)
                .FirstOrDefaultAsync(r => r.Id == id);

            request.Status = RequestStatus.Completed;

            await _dbContext.SaveChangesAsync();
            PushNewNotification(NotificationTypeEnum.Completed, AccountTypeEnum.All, $"#{request.Id.ToString()}");
            return Json(new { success = true });
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

        #region Maids Data
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

            return View(_dbContext.Maids.Include(i => i.Nationality).ToList());
        }

        [Route("GetMaids")]
        public IActionResult MaidsList()
        {
            var result = _dbContext.Maids.Include(i => i.Nationality).ToList();
            return PartialView("MaidsList", result);
        }

        [HttpPost]
        [Route("AddMaid")]
        public async Task<IActionResult> AddMaid([FromForm] MaidDto dto)
        {
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
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/maids");
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImagePath.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

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

        [HttpPost]
        [Route("ToggleAvailability/{id}/{isAvailable}")]
        public async Task<IActionResult> ToggleAvailability(int id, bool isAvailable)
        {
            var maid = await _dbContext.Maids.FindAsync(id);
            if (maid == null)
                return NotFound();

            maid.IsAvailable = isAvailable;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("GetMaid/{id}")]
        public JsonResult GetMaid(int id)
        {
            var maid = _dbContext.Maids.Include(i => i.Nationality)
                                       .Include(i => i.ServedCountries)
                                       .Include(i => i.Langauges)
                                       .Where(w => w.Id == id)
                                       .Single();
            if (maid == null)
            {
                return null;
            }

            var result = new
            {
                Id = maid.Id,
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
                servedCountryIds = maid.ServedCountries.Select(s => s.Id),
                languageIds = maid.Langauges.Select(s => s.Id),
                note = maid.Note,
                videoURL = maid.VideoURL,
                imagePath = maid.ImagePath
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


        [HttpPost]
        [Route("UploadAttachments")]
        public async Task<IActionResult> UploadAttachments([FromForm] AttachmentDto model)
        {
            var maid = await _dbContext.Maids
                .Include(m => m.Attachments)
                .FirstOrDefaultAsync(m => m.Id == model.MaidId);

            if (maid == null)
                return NotFound("Maid not found");

            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "maids", maid.Id.ToString());

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            async Task SaveFileAndUpdateAttachment(IFormFile file, AttachmentType type)
            {
                var existing = maid.Attachments.FirstOrDefault(a => a.AttachmentType == type);

                // Handle deletion request
                if (model.AttachmentsToDelete != null && model.AttachmentsToDelete.Contains(type))
                {
                    if (existing != null)
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existing.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);

                        _dbContext.Attachments.Remove(existing);
                    }

                    // If user only wants to delete without uploading a new file, exit here
                    if (file == null)
                        return;
                }

                // Skip if no new file and no delete request
                if (file == null)
                    return;

                // Save new file
                var uniqueFileName = $"{type}_{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Save attachment record
                var attachment = new Attachment
                {
                    FileName = file.FileName,
                    MaidId = maid.Id,
                    AttachmentType = type,
                    FilePath = "/uploads/maids/" + maid.Id + "/" + uniqueFileName,
                    UploadedAt = DateTime.Now
                };
                _dbContext.Attachments.Add(attachment);
            }

            await SaveFileAndUpdateAttachment(model.MedicalFile, AttachmentType.Medical);
            await SaveFileAndUpdateAttachment(model.ResidencyFile, AttachmentType.Residency);
            await SaveFileAndUpdateAttachment(model.PassportFile, AttachmentType.Passport);
            await SaveFileAndUpdateAttachment(model.OtherFile, AttachmentType.Other);

            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Attachments processed successfully" });
        }
        [HttpGet]
        [Route("GetAttachments/{id}")]
        public IActionResult GetAttachments(int id)
        {
            var attachments = _dbContext.Attachments
                .Where(a => a.MaidId == id)
                .Select(a => new
                {
                    type = a.AttachmentType.ToString(),
                    filePath = a.FilePath,
                    fileName = a.FileName
                })
                .ToList();

            return Json(attachments);
        }

        #endregion

        #region MaidRequests
        [Route("MaidsManagement/MaidRequests")]
        public IActionResult MaidRequests()
        {
            var maids = _dbContext.Maids.Where(w => w.IsAvailable == true).Include(i => i.Nationality).Include(i => i.Langauges).ToList();
            return View(maids);
        }

        public ActionResult FillMaidsList()
        {
            var maids = _dbContext.Maids.Where(w => w.IsAvailable == true).Include(i => i.Nationality).Include(i => i.Langauges).ToList();
            return PartialView("MaidList", maids);
        }

        public IActionResult FillMaidsListWithFilter(string name, int nationalityId, int langId, Age age, Experience experience, MaritalStatus maritalStatus, string sortBy)
        {
            IQueryable<Maid> query = _dbContext.Maids
                               .Where(w => w.IsAvailable == true)
                               .Include(m => m.Nationality)
                               .Include(m => m.Langauges)
                               .Include(m => m.ServedCountries);



            if (!string.IsNullOrWhiteSpace(name))
            {
                var loweredName = name.ToLower();

                query = query.Where(m =>
                 (m.FirstNameEn != null && EF.Functions.Like(m.FirstNameEn, $"%{name}%")) ||
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

        #endregion

        #region UserManagment


        [Route("UserManagement/ManageUsers")]
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, IList<string>>();
            var userPermissions = new Dictionary<string, List<string>>();

            foreach (var user in users)
            {
                userRoles[user.Id] = await _userManager.GetRolesAsync(user);

                // Simulate getting permissions from DB or claims
                var claims = await _userManager.GetClaimsAsync(user);
                userPermissions[user.Id] = claims
                    .Where(c => c.Type == "PageAccess")
                    .Select(c => c.Value)
                    .ToList();
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.UserPermissions = userPermissions;
            ViewBag.AvailablePermissions = AvailablePermissions;

            return View(users);
        }

        [HttpPost]
        [Route("UserManagement/ToggleUserStatus")]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.LockoutEnabled = !user.LockoutEnabled;
            user.LockoutEnd = user.LockoutEnabled ? DateTimeOffset.MaxValue : (DateTimeOffset?)null;

            await _userManager.UpdateAsync(user);
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [Route("UserManagement/UpdateMobile")]
        public async Task<IActionResult> UpdateMobile(string userId, string newPhone)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.PhoneNumber = newPhone;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [Route("UserManagement/UpdateUsername")]
        public async Task<IActionResult> UpdateUsername(string userId, string newUsername)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Check if the new username already exists
            var existingUser = await _userManager.FindByNameAsync(newUsername);
            if (existingUser != null && existingUser.Id != userId)
            {
                ModelState.AddModelError("", "Username already taken.");
                // You might want to return a better error view or just redirect with a message
                return RedirectToAction("ManageUsers");
            }

            user.UserName = newUsername;
            user.NormalizedUserName = _userManager.NormalizeName(newUsername); // Ensure it's normalized

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                // Log or show errors if needed
                return BadRequest(result.Errors);
            }

            return RedirectToAction("ManageUsers");
        }


        [HttpPost]
        [Route("UserManagement/UpdateUserPermissions")]
        public async Task<IActionResult> UpdateUserPermissions(string userId, List<string> permissions)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var existingClaims = await _userManager.GetClaimsAsync(user);
            var currentClaims = existingClaims.Where(c => c.Type == "PageAccess").ToList();

            // Remove old claims
            foreach (var claim in currentClaims)
                await _userManager.RemoveClaimAsync(user, claim);

            // Add selected permissions as claims
            foreach (var permission in permissions)
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("PageAccess", permission));

            return RedirectToAction("ManageUsers");
        }
        private static readonly List<string> AvailablePermissions = new List<string>
                                                                    {
                                                                        "Dashboard",
                                                                        "InternalRequest",
                                                                        "Maids",
                                                                        "Nationalities",
                                                                        "Countries",
                                                                        "ManageUsers",
                                                                        "FinancialReport",
                                                                        "GeneralSettings",
                                                                        "Feedbacks"
                                                                    };

        #endregion

        #region GeneralSettings
        [Route("Settings/GeneralSettings")]
        public IActionResult GeneralSettings()
        {
            var keys = new[] { "PhoneNumber", "WhatsAppNumber", "AddressAr", "AddressEn", "FacebookUrl", "InstagramUrl" };
            var settingsDict = _dbContext.GeneralSettings
                                       .Where(s => keys.Contains(s.SettingKey))
                                       .ToDictionary(s => s.SettingKey, s => s.SettingValue);

            var model = new GeneralSettingsViewModel
            {
                PhoneNumber = settingsDict.GetValueOrDefault("PhoneNumber", ""),
                WhatsAppNumber = settingsDict.GetValueOrDefault("WhatsAppNumber", ""),
                AddressAr = settingsDict.GetValueOrDefault("AddressAr", ""),
                AddressEn = settingsDict.GetValueOrDefault("AddressEn", ""),
                FacebookUrl = settingsDict.GetValueOrDefault("FacebookUrl", ""),
                InstagramUrl = settingsDict.GetValueOrDefault("InstagramUrl", "")
            };

            return View(model);
        }


        [HttpPost]
        [Route("Settings/UpdateGeneralSetting")]
        public IActionResult UpdateGeneralSetting(GeneralSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var keys = new[] { "PhoneNumber", "WhatsAppNumber", "AddressAr", "AddressEn", "FacebookUrl", "InstagramUrl" };
                var settings = _dbContext.GeneralSettings.Where(s => keys.Contains(s.SettingKey)).ToList();

                foreach (var key in keys)
                {
                    var setting = settings.FirstOrDefault(s => s.SettingKey == key);
                    if (setting != null)
                    {
                        setting.SettingValue = key switch
                        {
                            "PhoneNumber" => model.PhoneNumber,
                            "WhatsAppNumber" => model.WhatsAppNumber,
                            "AddressAr" => model.AddressAr,
                            "AddressEn" => model.AddressEn,
                            "FacebookUrl" => model.FacebookUrl,
                            "InstagramUrl" => model.InstagramUrl,
                            _ => setting.SettingValue
                        };
                    }
                    else
                    {
                        _dbContext.GeneralSettings.Add(new GeneralSettings
                        {
                            SettingKey = key,
                            SettingGroup = "Contact", // or "Social" accordingly
                            SettingValue = key switch
                            {
                                "PhoneNumber" => model.PhoneNumber,
                                "WhatsAppNumber" => model.WhatsAppNumber,
                                "AddressAr" => model.AddressAr,
                                "AddressEn" => model.AddressEn,
                                "FacebookUrl" => model.FacebookUrl,
                                "InstagramUrl" => model.InstagramUrl,
                                _ => ""
                            },
                            CreatedDate = DateTime.Now
                        });
                    }
                }

                _dbContext.SaveChanges();

                ViewBag.SuccessMessage = "Settings saved successfully.";

                // Return the view with updated model and ViewBag (no redirect)
                return View("GeneralSettings", model);
            }

            // If model state invalid
            ViewBag.ErrorMessage = "Please correct the errors.";
            return View("GeneralSettings", model);
        }

        #endregion

        #region FinancialReport



        [Route("Financial/FinancialReport")]
        public IActionResult FinancialReport(DateTime? fromDate, DateTime? toDate)
        {
            var model = new FinancialReportViewModel
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            var query = _dbContext.Requests
                .Where(r => (r.Status == RequestStatus.Completed || r.Status == RequestStatus.Prepared));

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(r => r.RequestDate.Date >= fromDate.Value.Date && r.RequestDate.Date <= toDate.Value.Date);
            }

            var filteredRequests = query.ToList();

            model.FilteredRequests = filteredRequests;
            model.CompletedCount = filteredRequests.Count(r => r.Status == RequestStatus.Completed);
            model.PreparedCount = filteredRequests.Count(r => r.Status == RequestStatus.Prepared);

            var financialEntriesquery = _dbContext.FinancialEntries.AsQueryable();

            if (fromDate.HasValue)
                financialEntriesquery = financialEntriesquery.Where(x => x.CreatedDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                financialEntriesquery = financialEntriesquery.Where(x => x.CreatedDate.Date <= toDate.Value.Date);

            model.FinancialEntries =  financialEntriesquery.OrderByDescending(x => x.CreatedDate).ToList();
     

            return View(model);
        }

        [HttpGet("GetFinancialSummary")]
        public IActionResult GetFinancialSummary()
        {
            var financialEntries = _dbContext.FinancialEntries.ToList();

            var totalIncome = financialEntries.Where(x => x.Type == FinancialEntryType.Income).Sum(x => x.Amount);
            var totalExpenses = financialEntries.Where(x => x.Type == FinancialEntryType.Expense).Sum(x => x.Amount);

            return Json(new { income = totalIncome, expenses = totalExpenses });
        }

        [HttpPost("UpdateContractStatus")]
        public IActionResult UpdateContractStatus(int id, bool isSigned)
        {
            var request = _dbContext.Requests.Find(id);
            if (request == null) return NotFound();

            request.ContractSigned = isSigned;
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpPost("UploadContract")]
        public async Task<IActionResult> UploadContract(IFormFile contractFile, int requestId)
        {
            if (contractFile != null && contractFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{contractFile.FileName}";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "contracts", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await contractFile.CopyToAsync(stream);
                }

                var request = _dbContext.Requests.Find(requestId);
                if (request == null) return NotFound();

                request.ContractFilePath = $"/uploads/contracts/{fileName}";
                request.ContractSigned = true;
                request.SignedDate = DateTime.Now;
                _dbContext.SaveChanges();

                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("GetContractAttachment")]
        public IActionResult GetContractAttachment(int id)
        {
            var request = _dbContext.Requests.Find(id);
            if (request == null || string.IsNullOrEmpty(request.ContractFilePath))
                return Json(new { contractUrl = "" });

            return Json(new { contractUrl = request.ContractFilePath });
        }


        [HttpPost("AddFinancialEntry")]
        public async Task<IActionResult> AddFinancialEntry(string title, decimal amount, FinancialEntryType type)
        {
            var entry = new FinancialEntry
            {
                Title = title,
                Amount = amount,
                Type = type,
                CreatedDate = DateTime.Now
            };

            _dbContext.FinancialEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("DeleteFinancialEntry")]
        public async Task<IActionResult> DeleteFinancialEntry(int id)
        {
            var entry = await _dbContext.FinancialEntries.FindAsync(id);
            if (entry == null) return NotFound();

            _dbContext.FinancialEntries.Remove(entry);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

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
            var currentUserId = GetAspNetUserId();
            var model = _dbContext.Notifications.Where(a => a.AssignedToUserId == currentUserId)
                                                .OrderByDescending(a => a.CreationDate)
                                                .Take(100)
                                                .ToList();

            foreach (var notification in model)
            {
                // Assuming CreationDate is a DateTime property in your model
                notification.CreationDate = notification.CreationDate.ToLocalTime(); // Convert to local time if necessary
                notification.CreationDateFormatted = notification.CreationDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
            }


            return View("Notifications", model);
        }
        #endregion
    }
}
