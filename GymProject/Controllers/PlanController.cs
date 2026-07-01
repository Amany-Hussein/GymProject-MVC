using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using GymProject.Context;
using GymProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProject.Controllers
{
    public class PlanController : Controller
    {

        private readonly IPlanService planService;

        public PlanController(IPlanService planService)
        {
            this.planService = planService;
        }

        //Get :: BaseUrl/Plan/Index

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Plans = await planService.GetAllAsync(ct : ct); //pass by name

            return View(Plans);
        }

        // Get :: BaseUrl/Plan/Details/{id}
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var Plan = await planService.GetPlanDetailsByIdAsync(id, ct);

            if (Plan == null)
                return RedirectToAction(nameof(Index));


            return View(Plan);
        }
    }
}