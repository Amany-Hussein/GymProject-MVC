using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using GymProject.Context;
using GymProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

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
            var Plans = await planService.GetAllPlansAsync(ct : ct); //pass by name

            return View(Plans);
        }

        // Get :: BaseUrl/Plan/Details/{id}
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var plan = await planService.GetPlanDetailsByIdAsync(id, ct);

            if (!plan.success) return NotFound(); 

            return View(plan.Value); 

        }

        // :: plan/Activate/{id}
        [HttpPost]

        public async Task<IActionResult> Activate(int id, CancellationToken ct = default)
        {
            var plan = await planService.ActivateButtom(id, ct);
            return RedirectToAction(nameof(Index));


        }

        //Get :: baseurl/plan/update/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await planService.GetPlanToUpdate(id, ct);
            if (result.success)
            {
                return View(result.Value);
            }
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await planService.UpdatePlanAsync(id, model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Plan Updated Successfully";
                return RedirectToAction(nameof(Index));

            }
            TempData["ErrorMessage"] = result.error;
            return View(model);

        }
    }
}