using Airbnb.Application.Const;
using Airbnb.DATA.models;
using Microsoft.AspNetCore.Identity;

namespace School.Application.SeedRoles;

public class DefaultUser
{
    public async static Task SeedAdminUserAsync(UserManager<User> userManager)
    {
        var Admin = new User()
        {
            FirstName = "Eman",
            LastName = "Elsayed",
            Email = "host@gmail.com",
            UserName = "host@gmail.com",
            EmailConfirmed = true
        };
        var user = await userManager.FindByEmailAsync(Admin.Email);
        if (user is null)
        {
            await userManager.CreateAsync(Admin, "P@ssword123");
            await userManager.AddToRoleAsync(Admin, AppRoles.Host);
        }
    }
}
