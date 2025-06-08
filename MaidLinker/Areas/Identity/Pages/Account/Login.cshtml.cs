using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MaidLinker.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager
           )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public enum UserRolesEnum
        {
            Administrator = 0,
            Reception = 1,
            Accountant = 2
        }

        [BindProperty]
        public InputModel? Input { get; set; }

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [EmailAddress]
            public string? Email { get; set; }

            public string? PhoneNumber { get; set; }

            [Required(ErrorMessage = "The Password is Required.")]
            [DataType(DataType.Password)]
            public string? Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            string cultureCode = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            bool isEng = cultureCode.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? true : false;

            string? username = string.Empty;

            if (Input == null || string.IsNullOrEmpty(Input.Password) || (string.IsNullOrEmpty(Input.PhoneNumber) && string.IsNullOrEmpty(Input.Email)))
            {
                // If we got this far, something failed, redisplay form
                return Page();
            }
            else
            {
                if (!string.IsNullOrEmpty(Input.PhoneNumber))
                {
                    // Search In Phone
                    var user = await _signInManager.UserManager.Users
                                                  .SingleOrDefaultAsync(x => x.PhoneNumber == Input.PhoneNumber);
                    if (user != null)
                        username = user.UserName;
                }
                else
                {
                    // Search In Email
                    var user = await _signInManager.UserManager.Users
                                                   .SingleOrDefaultAsync(x => x.Email == Input.Email);
                    if (user != null)
                        username = user.UserName;
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(username, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(username);
                    var userRole = await _userManager.GetRolesAsync(user);
                    if (userRole.Contains(UserRolesEnum.Accountant.ToString()) || userRole.Contains(UserRolesEnum.Reception.ToString()))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }

                    if (userRole.Contains(UserRolesEnum.Administrator.ToString()))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    return LocalRedirect(returnUrl);
                }


                if (result.IsNotAllowed)
                {
                    TempData["IsNeedConfirmation"] = true;
                    var msg = isEng ? "This account is not active yet, please check your email inbox to activate your account " : "هذا الحساب غير مفعل بعد، يرجى التحقق من صندوق البريد الإلكتروني الخاص بك لتفعيل حسابك";
                    ModelState.AddModelError(string.Empty, msg);
                    return Page();
                }
                #region RequiresTwoFactor,IsLockedOut Checks
                // Later On maybe we will use it 
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                //}
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning("User account locked out.");
                //    return RedirectToPage("./Lockout");
                //}
                #endregion

                else
                {
                    TempData["IsNeedConfirmation"] = false;
                    var msg = isEng ? "Invalid login attempt" : "محاولة تسجيل الدخول غير صالحة";
                    ModelState.AddModelError(string.Empty, msg);
                    return Page();
                }
            }

        }
    }
}
