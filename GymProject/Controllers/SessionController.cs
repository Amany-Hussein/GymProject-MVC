using GymManagement.BLL.Services.Classes;
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

        #region Index
        // Get => BaseUrl/Session/Index 
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Sessions = await sessionService.GetAllSessionsAsync(ct);
            return View(Sessions);
        }

        #endregion

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await DropDownList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
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

        #endregion

        #region details
        //Get Session details 

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await sessionService.GetSessionByIdAsync(id, ct);
            if (result.success)
            {
                return View(result.Value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion
        private async Task DropDownList()
        {
            ViewBag.Trainers = new SelectList(await sessionService.GetTrainerForDropDown(), "Id", "Name");
            ViewBag.Categories = new SelectList(await sessionService.GetCategoryrForDropDown(), "Id", "CategoryName");

        }


        #region Update
        //Get Session to update
        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct)
        {
            var result =await sessionService.GetSessionToUpdateAsync (id, ct);

            if (result.success)
            {
                await GetTrainerList();
                return View(result.Value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id , UpdateSessionViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await GetTrainerList();
                return View(model);
            }


            var result = await sessionService.UpdateSessionAsync(id, model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Updated Successfully";
                return RedirectToAction(nameof(Index));

            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                await GetTrainerList();
                return View(model);
            }
        }

        private async Task GetTrainerList()
        {
            ViewBag.Trainers = new SelectList(await sessionService.GetTrainerForDropDown(), "Id", "Name");

        }
        #endregion


    }
}


