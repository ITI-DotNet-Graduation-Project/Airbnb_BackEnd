


using Airbnb.Application.Const;
using Airbnb.DATA.models.Identity;
using Microsoft.AspNetCore.Identity;


namespace School.Application.SeedRoles;

public class DefaultRoles
{
    public async static Task SeedRolesAsync(RoleManager<Role> roleManager)
    {
        if (!roleManager.Roles.Any())
        {
            await roleManager.CreateAsync(new Role { Name = AppRoles.Host });
            await roleManager.CreateAsync(new Role { Name = AppRoles.Guest });

        }
    }
}
