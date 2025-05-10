using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace MaidLinker.Helper
{
    public class CookieRequestCultureProvider : IRequestCultureProvider
    {
        private const string _cookieName = "CultureInfo";

        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            string selectedCulture = httpContext.Request.Cookies[_cookieName];

            if (!string.IsNullOrEmpty(selectedCulture))
            {
                var cultureInfo = new CultureInfo(selectedCulture);
                var providerResult = new ProviderCultureResult(cultureInfo.Name, cultureInfo.Name);
                return Task.FromResult(providerResult);
            }

            // Default culture if cookie value is not found
            var defaultCulture = "en-US";
            return Task.FromResult(new ProviderCultureResult(defaultCulture, defaultCulture));
        }
    }
}
