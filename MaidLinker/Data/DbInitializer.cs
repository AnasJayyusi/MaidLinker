using Microsoft.AspNetCore.Identity;

namespace MaidLinker.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider
                .GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider
                       .GetRequiredService<UserManager<IdentityUser>>();
            var config = serviceProvider
                .GetRequiredService<IConfiguration>();
            var roleName = "Administrator";
            IdentityResult result;
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                result = await roleManager
                    .CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {

                    var admin = await userManager
                        .FindByEmailAsync(config["AdminCredentials:Email"]);

                    if (admin == null)
                    {
                        admin = new IdentityUser()
                        {
                            UserName = config["AdminCredentials:Email"],
                            Email = config["AdminCredentials:Email"],
                            PhoneNumber = config["AdminCredentials:PhoneNumber"],
                            EmailConfirmed = true
                        };
                        result = await userManager
                            .CreateAsync(admin, config["AdminCredentials:Password"]);
                        if (result.Succeeded)
                        {
                            result = await userManager
                                .AddToRoleAsync(admin, roleName);
                            if (!result.Succeeded)
                            {
                                // todo: process errors
                            }
                        }
                    }
                }
            }


            roleName = "Accountant";
            roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
                await roleManager.CreateAsync(new IdentityRole(roleName));


            roleName = "Reception";
            roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
                await roleManager.CreateAsync(new IdentityRole(roleName));


            // Create Reception Users
            var receptionUsers = new[]
            {
        new { Email = "reception.frontdesk1@maidlinker.com", Password = "123456789" },
        new { Email = "reception.frontdesk2@maidlinker.com", Password = "123456789" }
    };

            foreach (var userInfo in receptionUsers)
            {
                int i = 0;
                var user = await userManager.FindByEmailAsync(userInfo.Email);
                if (user == null)
                {
                    user = new IdentityUser
                    {
                        UserName = userInfo.Email,
                        Email = userInfo.Email,
                        EmailConfirmed = true,
                        PhoneNumber = "078503940" + i // Example phone number, you can change it as needed
                    };
                    var result2 = await userManager.CreateAsync(user, userInfo.Password);
                    if (result2.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Reception");
                    }
                }
                i++;
            }

            // Create Accountant User
            var accountantEmail = "accountant@maidlinker.com";
            var accountantUser = await userManager.FindByEmailAsync(accountantEmail);
            if (accountantUser == null)
            {
                accountantUser = new IdentityUser
                {
                    UserName = accountantEmail,
                    Email = accountantEmail,
                    EmailConfirmed = true,
                    PhoneNumber = "0785039403" // Example phone number, you can change it as needed
                };
                var result3 = await userManager.CreateAsync(accountantUser, "123456789");
                if (result3.Succeeded)
                {
                    await userManager.AddToRoleAsync(accountantUser, "Accountant");
                }
            }

        }
    }
}