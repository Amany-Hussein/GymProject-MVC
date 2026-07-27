using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Context
{
    public class IdentityDataSeeding
    {
        public static async Task SeedIdentityData(RoleManager<IdentityRole> roleManager,
                                                  UserManager<ApplicationUser> userManager,
                                                  ILogger logger,
                                                  CancellationToken ct = default)
        {

            try
            {
                //check if users of roles exist or not
                bool hasusers = await userManager.Users.AnyAsync(ct);
                bool hasroles = await roleManager.Roles.AnyAsync(ct);
                if (hasroles && hasusers) return;
                //Rolles
                var roles = new List<IdentityRole>()
                {
                    new IdentityRole("SuperAdmin"),
                    new IdentityRole("Admin")
                };

                foreach (var role in roles)
                {

                    if (!await roleManager.RoleExistsAsync(role.Name))
                    {
                        var roleresult = await roleManager.CreateAsync(role);
                        if (!roleresult.Succeeded)
                        {
                            logger.LogError($"Failed To Add Role {role.Name}");
                        }
                    }
                }

                //Users
                if (!hasusers)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Amany",
                        LastName = "Hussein",
                        Email = "Amany@gmail.com",
                        UserName = "AmanyHussein1",
                        PhoneNumber = "01103928640"

                    };
                    await userManager.CreateAsync(MainAdmin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");
                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Aya",
                        LastName = "Hassan",
                        Email = "Aya@gmail.com",
                        UserName = "AyaHassan5",
                        PhoneNumber = "01284785578"

                    };
                    await userManager.CreateAsync(Admin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(Admin, "Admin");

                    logger.LogInformation("Identity Seeded Successfully");

                }

            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return;

            }



        }
    }
}
//need to call it in main but we already made seprate class that call gymdataseeding so use it