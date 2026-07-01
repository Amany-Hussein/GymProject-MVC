using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymProject.PL.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionService sessionService;

        public SessionController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        // Get => BaseUrl/Session/Index 
        public async Task<IActionResult> Index(CancellationToken ct )
        {
            var Sessions = await sessionService.GetAllSessionsAsync(ct);
            return View(Sessions);
        }

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await DropDownList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model , CancellationToken ct) 
        {
            //Check ModelState
            if (!ModelState.IsValid)
            {
                await DropDownList();
                return View(model);
            }
            var result = await sessionService.CreateSessionAsync(model, ct);

            if (result.success)
            {
                TempData["SuccessMessage"] = "Session CreateD Successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.error;

            await DropDownList();
            return View(model);


        }

        private async Task DropDownList()
        {
            ViewBag.Trainers = new SelectList(await sessionService.GetTrainerForDropDown(), "Id", "Name");
            ViewBag.Categories = new SelectList(await sessionService.GetCategoryrForDropDown(), "Id", "CategoryName");

        }

        #endregion
    }
}

