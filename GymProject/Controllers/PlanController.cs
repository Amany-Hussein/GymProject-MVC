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

        private readonly IGenericRepository<Plan> _planRepository;

        public PlanController(IGenericRepository<Plan> planRepository)
        {
            _planRepository = planRepository;
        }

        //Get :: BaseUrl/Plan/Index

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Plans = await _planRepository.GetAllAsync(ct : ct); //pass by name

            return View(Plans);
        }

        // Get :: BaseUrl/Plan/Details/{id}
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var Plan = await _planRepository.GetByIdAsync(id , ct);

            if (Plan == null)
                return RedirectToAction(nameof(Index));


            return View(Plan);
        }
    }
}