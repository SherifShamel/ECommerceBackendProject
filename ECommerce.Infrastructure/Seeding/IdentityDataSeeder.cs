using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.Identity;
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Seeding
{
    public class IdentityDataSeeder : IDataSeeder
    {
        private readonly StoreIdentityDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<IdentityDataSeeder> logger;

        public IdentityDataSeeder(StoreIdentityDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IdentityDataSeeder> logger
            )
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }
        public async Task SeedAsync(CancellationToken ct = default)
        {
            try
            {
                var PendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(ct);

                if (PendingMigrations.Any())
                    await dbContext.Database.MigrateAsync(ct);

                if (!await roleManager.Roles.AnyAsync(ct))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                }
                if (!await userManager.Users.AnyAsync(ct))
                {
                    var admin = new ApplicationUser
                    {
                        DisplayName = "Sherif Shamel",
                        Email = "sherif@gmail.com",
                        UserName = "Sherif",
                        PhoneNumber = "01273216464"

                    };
                    var Result = await userManager.CreateAsync(admin, "P@ssw0rd");
                    if (Result.Succeeded)
                        await userManager.AddToRoleAsync(admin, "Admin");
                    else
                        logger.LogWarning("User Is Not Created");
                }
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Cannot Seed Data");
                throw;
            }
        }
    }
}
