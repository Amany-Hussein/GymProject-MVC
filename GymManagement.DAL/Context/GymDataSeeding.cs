using GymProject.Context;
using GymProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GymManagement.DAL.Context
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext , string seedFolderPath , ILogger logger)
        {
            // Get Data from json


            try
            {
                // Check if table plans has data or not
                if (!await dbContext.Plans.AnyAsync())
                {
                    // seed from json file
                    var plans = LoadDataFromJsonFile<Plan>(seedFolderPath, "plans.json");

                    if (plans.Any())
                    {
                        dbContext.Plans.AddRange(plans);
                        logger.LogInformation($"PLans Seeded With Count {plans.Count}");
                    }

                    //savechangesasync 
                    // check for changetracker has done any change in it or no  to avoid do savechange
                    if (dbContext.ChangeTracker.HasChanges())
                    {
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        logger.LogInformation("Plans Already  Seeded");
                    }
                }
            }

            catch (Exception ex)
            {
                logger.LogInformation(ex, "Seeding Failed");
                throw;

            }
        }
        

        public static List<T> LoadDataFromJsonFile<T>(string folderPath , string fileName)
        {
            //file path
            //D:\.NET Core\MVC\Projects\MyGym\MyGym\MyGym\wwwroot\           Files\plans.json
            var filePath = Path.Combine(folderPath , fileName);

            if (!File.Exists(filePath)) 
                throw new FileNotFoundException("File Date Not Found");

            //read date from json file
            var data = File.ReadAllText(filePath);

            // important option => ignore to case sensitive
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            //confert it from json to my list
            return JsonSerializer.Deserialize<List<T>>(data, options) ?? []; // {??} if returned null make empty collection  

        }

    }
}
