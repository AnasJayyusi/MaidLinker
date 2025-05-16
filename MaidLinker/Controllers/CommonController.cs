using MaidLinker.Data;
using MaidLinker.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace MaidLinker.Controllers
{

    [Route("Common")]
    public class CommonController : BaseController
    {
        public CommonController(ApplicationDbContext dbContext, INotificationService notificationService) : base(dbContext, notificationService)
        {
        }

        #region Shared DropDownList

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

        [HttpGet]

        [HttpGet]
        [Route("GetCountriesDDL")]
        public ActionResult GetCountriesDDL()
        {
            // Retrieve the data for the dropdown list
            var dropdownData = _dbContext.Countries.ToList();

            // Pass the data to the view
            return Json(dropdownData);
        }



        #endregion

        #region Helper For Razor Page

        [AllowAnonymous]
        [HttpGet]
        [Route("CheckPhoneNumber")]
        public bool CheckPhoneNumber(string phoneNumber)
        {
            bool isUsed = false;

            var userProfile = _dbContext.UserProfiles.FirstOrDefault(x => x.PhoneNumber.Equals(phoneNumber));
            if (userProfile != null)
            {
                isUsed = true;
            }

            return isUsed;
        }
        #endregion

    }
}