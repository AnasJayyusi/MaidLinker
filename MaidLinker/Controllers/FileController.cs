using MaidLinker.Data;
using MaidLinker.Data.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace MaidLinker.Controllers
{
    [Route("File")]
    [Authorize(Roles = "Administrator,ServiceProvider")]
    public class FileController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileController(ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("DownloadFileFromFolder")]
        public FileResult DownloadFileFromFolder(string fileName)
        {
            //Build the File Path.  
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "uploadfiles\\") + fileName;

            //Read the File data into Byte Array.  
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.  
            return File(bytes, "application/octet-stream", fileName);
        }

        [HttpPost]
        [Route("UploadCertification")]
        public async Task<IActionResult> UploadCertification(IFormFile file, int degreeId, string universityNameAr, string universityNameEn)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (degreeId < -1 || string.IsNullOrEmpty(universityNameAr) || string.IsNullOrEmpty(universityNameEn))
            {
                return BadRequest("Please provide the degree and university names.");
            }

            try
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Specify the directory where you want to save the uploaded file
                string webRootPath = _webHostEnvironment.WebRootPath;
                string attachmentsPath = Path.Combine("users", "attachments");
                string uploadPath = Path.Combine(webRootPath, attachmentsPath, currentUserId);
                // Create the directory if it doesn't exist
                Directory.CreateDirectory(uploadPath);
                // Generate a unique file name
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string fileExtension = Path.GetExtension(file.FileName);
                string uniqueFileName = fileName + fileExtension;

                // Combine the directory and unique file name
                string filePath = Path.Combine(uploadPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    // Copy the file to the specified path
                    await file.CopyToAsync(stream);
                }
                //var attachment = new Certification()
                //{
                //    UserId = currentUserId,
                //    UserProfileId = GetUserId(),
                //    Name = fileName,
                //    Extension = fileExtension,
                //    Path = $"/users/attachments/{currentUserId}/{uniqueFileName}",
                //    DegreeId = degreeId,
                //    UniversityNameAr = universityNameAr,
                //    UniversityNameEn = universityNameEn
                //};

                //_dbContext.Certifications.Add(attachment);
                //_dbContext.SaveChanges();

                // File uploaded successfully
                return Ok("File uploaded successfully.");
            }
            catch (IOException ex)
            {
                // Handle the error
                return StatusCode(StatusCodes.Status500InternalServerError, $"File upload failed: {ex.Message}");
            }
        }


        //[HttpPost]
        //[Route("UploadRequiredFiles")]
        //public async Task<IActionResult> UploadRequiredFiles(/*IFormFile contractfile,*/ IFormFile categoryfile)
        //{
        //    if (/*contractfile == null || contractfile.Length == 0 || */categoryfile == null || categoryfile.Length == 0)
        //    {
        //        return BadRequest("Contract File Or Category File is not Uploaded");
        //    }

        //    try
        //    {
        //        string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        // Specify the directory where you want to save the uploaded file
        //        string webRootPath = _webHostEnvironment.WebRootPath;
        //        string attachmentsPath = Path.Combine("users", "attachments");
        //        string uploadPath = Path.Combine(webRootPath, attachmentsPath, currentUserId);
        //        // Create the directory if it doesn't exist
        //        Directory.CreateDirectory(uploadPath);

        //        var oldDocs = _dbContext.RequiredAttachments.Where(w => w.UserId == currentUserId);
        //        foreach (var attachment in oldDocs)
        //        {
        //            _dbContext.RequiredAttachments.Remove(attachment);
        //        }


        //        //#region SignedContract
        //        //// SignedContract File
        //        //// Generate a unique file name
        //        //string signedContractfileName = Path.GetFileNameWithoutExtension(contractfile.FileName);
        //        //string signedContractfileExtension = Path.GetExtension(contractfile.FileName);
        //        //string signedContractUniqueFileName = signedContractfileName + signedContractfileExtension;

        //        //// Combine the directory and unique file name
        //        //string signedContractfilePath = Path.Combine(uploadPath, signedContractUniqueFileName);

        //        //using (var stream = new FileStream(signedContractfilePath, FileMode.Create))
        //        //{
        //        //    // Copy the file to the specified path
        //        //    await contractfile.CopyToAsync(stream);
        //        //}


        //        //var contractAttachment = new RequiredAttachment()
        //        //{
        //        //    UserId = currentUserId,
        //        //    UserProfileId = GetUserId(),
        //        //    Name = signedContractfileName,
        //        //    Extension = signedContractfileExtension,
        //        //    RequiredFileType = RequiredFileType.SignedContract,
        //        //    Path = $"/users/attachments/{currentUserId}/{signedContractUniqueFileName}",
        //        //};
                
        //        //_dbContext.RequiredAttachments.Add(contractAttachment);
        //        //#endregion

        //        #region Category
        //        // Generate a unique file name
        //        string categoryfileName = Path.GetFileNameWithoutExtension(categoryfile.FileName);
        //        string categoryfileExtension = Path.GetExtension(categoryfile.FileName);
        //        string categoryUniqueFileName = categoryfileName + categoryfileExtension;

        //        // Combine the directory and unique file name
        //        string categoryfilePath = Path.Combine(uploadPath, categoryUniqueFileName);
        //        // ProfessionalCategory File
        //        // Generate a unique file name
        //        using (var stream = new FileStream(categoryfilePath, FileMode.Create))
        //        {
        //            // Copy the file to the specified path
        //            await categoryfile.CopyToAsync(stream);
        //        }

        //        var categoryAttachment = new RequiredAttachment()
        //        {
        //            UserId = currentUserId,
        //            UserProfileId = GetUserId(),
        //            Name = categoryfileName,
        //            Extension = categoryfileExtension,
        //            RequiredFileType= RequiredFileType.ProfessionalCategory,
        //            Path = $"/users/attachments/{currentUserId}/{categoryUniqueFileName}",
        //        };

        //        _dbContext.RequiredAttachments.Add(categoryAttachment);
        //        #endregion

        //        _dbContext.SaveChanges();

        //        // File uploaded successfully
        //        return Ok("Files uploaded successfully.");
        //    }
        //    catch (IOException ex)
        //    {
        //        // Handle the error
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"File upload failed: {ex.Message}");
        //    }
        //}


        //[HttpPost]
        //[Route("DeleteFile/{id}")]
        //public IActionResult DeleteFile(Guid id)
        //{
        //    try
        //    {
        //        // Remove From Database
        //        var attachment = _dbContext.Certifications.Find(id);
        //        _dbContext.Certifications.Remove(attachment);
        //        _dbContext.SaveChanges();

        //        // Remove From Disk Storage 
        //        string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        string webRootPath = _webHostEnvironment.WebRootPath;
        //        string attachmentsPath = Path.Combine("users", "attachments");
        //        string uploadPath = Path.Combine(webRootPath, attachmentsPath, currentUserId);
        //        string filePath = Path.Combine(uploadPath, attachment.Name + attachment.Extension);
        //        System.IO.File.Delete(filePath);

        //        return Ok("File Deleted successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle the error
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"File delete failed: {ex.Message}");
        //    }
        //}

        //private int GetUserId()
        //{
        //    string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    return _dbContext.UserProfiles.Where(w => w.UserId == currentUserId).Select(x => x.Id).Single();
        //}

    }
}
