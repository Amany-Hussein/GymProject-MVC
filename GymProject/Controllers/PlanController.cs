using GymProject.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProject.Controllers
{
    public class PlanController : Controller
    {
        //Database connection
        private readonly GymDbContext Context;

        public PlanController()
        {
            Context = new GymDbContext();
        }

        //Get :: BaseUrl/Plan/Index

        public async Task<IActionResult> Index()
        {
            var Plans =await Context.Plans.ToListAsync();

            return View(Plans);
        }

        // Get :: BaseUrl/Plan/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var Plan = await Context.Plans.FindAsync(id);

            if (Plan == null) 
                return RedirectToAction(nameof(Index));
            

            return View(Plan);
        }
    }
}
