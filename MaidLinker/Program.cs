using MaidLinker.Data;
using MaidLinker.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

string logger = string.Empty;
logger = "Starting Program";

try
{
    var builder = WebApplication.CreateBuilder(args);
    logger = "Create CreateBuilder";

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    logger = "connectionString Applied";
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    logger = "AddDatabaseDeveloperPageExceptionFilter Applied";

    builder.Services.AddDefaultIdentity<IdentityUser>(options => options.User.RequireUniqueEmail = false).AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.AddLocalization(o => { o.ResourcesPath = "Resources"; });
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.DefaultRequestCulture = new RequestCulture("en-US");
        options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("ar-SA") };
        options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("ar-SA") };
        options.RequestCultureProviders.Insert(0, new MaidLinker.Helper.CookieRequestCultureProvider());
    });

    builder.Services.AddSignalR();
    builder.Services.AddSingleton<INotificationService, NotificationService>();
    builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation()
         .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
         .AddDataAnnotationsLocalization();

    builder.Services.AddRazorPages();
    logger = "AddRazorPages Applied";



    builder.Services.Configure<IdentityOptions>(options =>
    {
        // Default Password settings.
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 0;
    });
    builder.Services.Configure<IdentityOptions>(opts =>
    {
        opts.SignIn.RequireConfirmedEmail = true;
    });
    logger = "IdentityOptions Applied";


    //builder.Services.AddScoped<RoleManager<IdentityRole>>();

    //logger = "IdentityRole Applied";
    ////// Add Admin Role
    //builder.Services.AddAuthorization(options =>
    //{
    //    options.AddPolicy("RequireAdministratorRole",
    //         policy => policy.RequireRole("Administrator"));

    //    options.AddPolicy("RequireUserRole",
    //         policy => policy.RequireRole("User"));

    //    options.AddPolicy("RequireDoctorRole",
    //         policy => policy.RequireRole("Doctor"));
    //});

    //logger = "Roles Applied";

    var app = builder.Build();
    logger = "Start Building";
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        // For Feeding Role 
        using (var scope = app.Services.CreateScope())
        {
            await DbInitializer.Initialize(scope.ServiceProvider);
        }
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    logger = "Static Files Applied";
    app.UseRouting();
    logger = "Routing Applied";
    app.UseAuthentication();
    app.UseAuthorization();
    //app.UseMiddleware<CultureMiddleware>();

    logger = "Authorization Applied";

    app.UseRequestLocalization();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    app.MapRazorPages();

    using (StreamWriter writer = new StreamWriter("./Debug.txt"))
        writer.WriteLine(logger);
    app.MapHub<NotificationHub>("/notificationHub");

    app.Run();


}
catch (Exception)
{
    using (StreamWriter writer = new StreamWriter("./Debug.txt"))
        writer.WriteLine(logger);
    throw;
}

