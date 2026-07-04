using GymManagement.BLL.Services.Interfaces;
using GymProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GymProject.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IAnalyticsService analyticsService;

        public HomeController(ILogger<HomeController> logger, IAnalyticsService analyticsService)
        {
            this.logger = logger;
            this.analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var data = await analyticsService.GetDataAsync(ct);
            if (data.success)
            {
                return View(data.Value);
            }
            return View(data.Value);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
