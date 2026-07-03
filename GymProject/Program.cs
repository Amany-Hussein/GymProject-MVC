using GymManagement.BLL.Profiles;
using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using GymProject.Context;
using GymProject.PL;
using Microsoft.EntityFrameworkCore;

namespace GymProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddAutoMapper(x => x.AddProfile(new MappingProfile()));

            builder.Services.AddDbContext<GymDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            

            var app = builder.Build();


            #region seeding

            // Seeding
            await app.MigrateAndSeedDataAsync();
            

            #endregion


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Middlewares
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
