using MaidLinker.Data;
using MaidLinker.Data.Entites;
using MaidLinker.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static MaidLinker.Data.SharedEnum;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace MaidLinker.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private IHostingEnvironment _environment;
        protected readonly ApplicationDbContext _dbContext;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IHostingEnvironment environment,
            ApplicationDbContext dbContext
          )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _environment = environment;
            _dbContext = dbContext;
        }



        [BindProperty]
        public List<AccountType> AccountTypes { get; set; }

        [BindProperty]
        public List<PractitionerType> PractitionerTypes { get; set; }


        [BindProperty]
        public InputModel Input { get; set; }

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Required]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string? Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }

            public int AccountTypeId { get; set; }
            [BindProperty]
            public int PractitionerTypeId { get; set; } = 0;
            [BindProperty]
            public int ProfessionalRankId { get; set; } = 0;

            [Required]
            public string? FullName { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            LoadDDLs();
        }

        private void LoadDDLs()
        {
            FillAccountTypes();
            FillPractitionerTypes();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl, int account_type, int practitioner_type, int professional_Rank)
        {
            string debug = "Start";



            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();
                if (account_type == (int)AccountTypeEnum.Accountant)
                {
                    user = new ApplicationUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        PhoneNumber = Input.PhoneNumber,
                        AccountTypeId = account_type,
                        FullName = Input.FullName,
                    };
                }
                else
                {
                    user = new ApplicationUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        PhoneNumber = Input.PhoneNumber,
                        AccountTypeId = account_type,
                        FullName = Input.FullName,
                    };
                }


                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    debug = "Succeeded";

                    _logger.LogInformation("User created a new account with password.");

                    if (account_type == (int)AccountTypeEnum.Reception)
                        await _userManager.AddToRoleAsync(user, AccountTypeEnum.Reception.ToString());

                    if (account_type == (int)AccountTypeEnum.Accountant)
                        await _userManager.AddToRoleAsync(user, AccountTypeEnum.Accountant.ToString());



                    #region Generate Confirmation Code 
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    #endregion

                    debug = "callbackUrl";
                    //CreateUserProfileData(user.Id);
                    await SendEmailConfirmation(callbackUrl);
                    debug = "send";

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }



                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            using (StreamWriter writer = new StreamWriter("./RegisterDebug.txt"))
                writer.WriteLine(debug);
            // If we got this far, something failed, redisplay form

            LoadDDLs();
            return Page();
        }

        private async Task SendEmailConfirmation(string callbackUrl)
        {
            string body = string.Empty;
            body = GetTemplateBody();
            body = body.Replace("{ConfirmationLink}", callbackUrl);
            body = body.Replace("{UserName}", Input.Email);
            await EmailSender.SendEmailAsync(Input.Email, "Confirm your account", body, true);
        }

        private string GetTemplateBody()
        {
            string body = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" style=\"font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">\r\n<head>\r\n    <meta name=\"viewport\" content=\"width=device-width\" />\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />\r\n    <title>WSS</title>\r\n    <style type=\"text/css\">\r\n        img {\r\n            max-width: 100%;\r\n        }\r\n\r\n        body {\r\n            -webkit-font-smoothing: antialiased;\r\n            -webkit-text-size-adjust: none;\r\n            width: 100% !important;\r\n            height: 100%;\r\n            line-height: 1.6em;\r\n        }\r\n\r\n        body {\r\n            background-color: #f6f6f6;\r\n        }\r\n\r\n        @media only screen and (max-width: 640px) {\r\n            body {\r\n                padding: 0 !important;\r\n            }\r\n\r\n            h1 {\r\n                font-weight: 800 !important;\r\n                margin: 20px 0 5px !important;\r\n            }\r\n\r\n            h2 {\r\n                font-weight: 800 !important;\r\n                margin: 20px 0 5px !important;\r\n            }\r\n\r\n            h3 {\r\n                font-weight: 800 !important;\r\n                margin: 20px 0 5px !important;\r\n            }\r\n\r\n            h4 {\r\n                font-weight: 800 !important;\r\n                margin: 20px 0 5px !important;\r\n            }\r\n\r\n            h1 {\r\n                font-size: 22px !important;\r\n            }\r\n\r\n            h2 {\r\n                font-size: 18px !important;\r\n            }\r\n\r\n            h3 {\r\n                font-size: 16px !important;\r\n            }\r\n\r\n            .container {\r\n                padding: 0 !important;\r\n                width: 100% !important;\r\n            }\r\n\r\n            .content {\r\n                padding: 0 !important;\r\n            }\r\n\r\n            .content-wrap {\r\n                padding: 10px !important;\r\n            }\r\n\r\n            .invoice {\r\n                width: 100% !important;\r\n            }\r\n        }\r\n    </style>\r\n</head>\r\n<body itemscope itemtype=\"http://schema.org/EmailMessage\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6em; background-color: #f6f6f6; margin: 0;\" bgcolor=\"#f6f6f6\">\r\n    <table class=\"body-wrap\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; background-color: #f6f6f6; margin: 0;\" bgcolor=\"#f6f6f6\">\r\n        <tr style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">\r\n            <td style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;\" valign=\"top\"></td>\r\n            <td class=\"container\" width=\"600\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; display: block !important; max-width: 600px !important; clear: both !important; margin: 0 auto;\" valign=\"top\">\r\n                <div class=\"content\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; max-width: 600px; display: block; margin: 0 auto; padding: 20px;\">\r\n                    <table class=\"main\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; border-radius: 3px; background-color: #fff; margin: 0; border: 1px solid #e9e9e9;\" bgcolor=\"#fff\">\r\n                        <tr style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">\r\n                            <td class=\"alert alert-warning\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 16px; vertical-align: top; color: #fff; font-weight: 500; text-align: center; border-radius: 3px 3px 0 0; background-color: #5F7AE7; margin: 0; padding: 20px;\" align=\"center\" bgcolor=\"#FF9F00\" valign=\"top\">\r\n                                Account Confirmation\r\n                            </td>\r\n                        </tr>\r\n                        <tr style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">\r\n                            <td class=\"content-wrap\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 20px;\" valign=\"top\">\r\n                                <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">\r\n                                    <tr style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">\r\n                                        <td class=\"content-block\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;\" valign=\"top\">\r\n                                            Hello, <strong style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">{UserName}</strong>\r\n                                        </td>\r\n                                    </tr>\r\n                                    <tr style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">\r\n                                        <td class=\"content-block\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;\" valign=\"top\">\r\n                                            Please click bellow button for confirm your account.\r\n                                        </td>\r\n                                    </tr>\r\n                                    <tr style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">\r\n                                        <td class=\"content-block\" style=\"\">\r\n                                            <a href=\"{ConfirmationLink}\" class=\"btn-primary\" style=\"background-color: #5F7AE7; border: none;color: white; padding: 10px 41px;text-align: center;text-decoration: none;display: inline-block; font-size: 16px;margin: 4px 2px;cursor: pointer;\">Confirmation Link </a>\r\n                                        </td>\r\n                                    </tr>\r\n                                    <tr style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">\r\n                                        <td class=\"content-block\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;\" valign=\"top\">\r\n                                            <br /><b>Thanks & Regards,</b><br />\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table><div class=\"footer\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; clear: both; color: #999; margin: 0; padding: 20px;\">\r\n                        <table width=\"100%\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">\r\n                            <tr style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;\">\r\n                                <td class=\"aligncenter content-block\" style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 12px; vertical-align: top; color: #999; text-align: center; margin: 0; padding: 0 0 20px;\" align=\"center\" valign=\"top\"></td>\r\n                            </tr>\r\n                        </table>\r\n                    </div>\r\n                </div>\r\n            </td>\r\n            <td style=\"font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;\" valign=\"top\"></td>\r\n        </tr>\r\n    </table>\r\n</body>\r\n</html>  ";
            return body;
        }

        private void FillAccountTypes()
        {
            AccountTypes = _dbContext.AccountTypes.ToList();

        }

        private void FillPractitionerTypes()
        {
            PractitionerTypes = _dbContext.PractitionerTypes.Where(x => x.IsActive).ToList();
        }

 

        public FileResult DownloadFileFromFolder(string fileName)
        {
            //Build the File Path.  
            string path = Path.Combine(_environment.WebRootPath, "staticfiles/") + fileName;

            //Read the File data into Byte Array.  
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.  
            return File(bytes, "application/octet-stream", fileName);
        }


        //private void CreateUserProfileData(string userId)
        //{
        //    var applicationUser = _dbContext.ApplicationUsers.Single(x => x.Id == userId);
        //    var userProfile = new UserProfile();

        //    var generalsettings = _dbContext.GeneralSettings.First();



        //    if (applicationUser.AccountTypeId == (int)AccountTypeEnum.Accountant)
        //    {
        //        userProfile = new UserProfile()
        //        {
        //            UserId = userId,
        //            FullName = applicationUser.FullName,
        //            AccountTypeId = applicationUser.AccountTypeId,
        //            PhoneNumber = applicationUser.PhoneNumber,
        //            Email = applicationUser.Email,
        //            ProfileStatus = ProfileStatus.Active // Based On Request From Client
        //        };
        //    }
        //    else
        //    {
        //        userProfile = new UserProfile()
        //        {
        //            UserId = userId,
        //            FullName = applicationUser.FullName,
        //            AccountTypeId = applicationUser.AccountTypeId,
        //            PhoneNumber = applicationUser.PhoneNumber,
        //            Email = applicationUser.Email,
        //            ProfileStatus = ProfileStatus.Active,
        //        };
        //    }
        //    _dbContext.UserProfiles.Add(userProfile);
        //    _dbContext.SaveChanges();
        //}
    }
}
