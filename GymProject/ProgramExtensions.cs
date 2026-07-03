using GymManagement.DAL.Context;
using GymProject.Context;
using Microsoft.EntityFrameworkCore;

namespace GymProject.PL
{
    public static class ProgramExtensions
    {
        public static async Task MigrateAndSeedDataAsync(this WebApplication app) // the magic in word this => so will refer in the class itself
        {

            // add my  ownn scope for seeding
            // scope unmanagment resource => i opened it so i must close it so use usiung
            using var scope = app.Services.CreateScope();

            //check  gymdbcontext
            var dbcontext = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            //logger
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            //any pending migration 
            var PendingMigration = await dbcontext.Database.GetPendingMigrationsAsync();
            if (PendingMigration.Any())
            {
                await dbcontext.Database.MigrateAsync(); //Apply => Update-Database
            }

            //FolderPath
            //D:\.NET Core\MVC\Projects\MyGym\MyGym\MyGym\wwwroot\Files\plans.json
            var SeedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            await GymDataSeeding.SeedAsync(dbcontext, SeedFolderPath, logger);

        }
    }
}
