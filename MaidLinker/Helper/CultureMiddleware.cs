using Microsoft.AspNetCore.Localization;
using System.Globalization;



namespace MaidLinker.Helper
{
    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;

        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();

            // Get the selected culture from the cookie or use a default value
            string selectedCulture = context.Request.Cookies["CultureInfo"] ?? "ar-SA";

            // Update the culture for the current request
            CultureInfo.CurrentCulture = new CultureInfo(selectedCulture);
            CultureInfo.CurrentUICulture = new CultureInfo(selectedCulture);

            await _next(context);
        }
    }
}