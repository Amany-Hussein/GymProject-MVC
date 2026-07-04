using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TrainerController : Controller
    {
        private readonly ITrainerService trainerService;

        public TrainerController(ITrainerService trainerService)
        {
            this.trainerService = trainerService;
        }

        //Gel all Trainers
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var result = await trainerService.GetAllTrainersAsync(ct);
            if (result.success)
            {
                return View(result.Value);
            }
            TempData["ErrorMessage"] = result.error;
            return View(Enumerable.Empty<TrainerViewModel>());

        }


        //Create new Trainer
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await trainerService.CreateTrainerAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Trainer Added Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            return View(model);


        }

        // Get Trainer details
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var result = await trainerService.GetTrainerDetailsByIdAsync(id, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
            }
            return View(result.Value);

        }

        //Update Trainer
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
        {
            var trainer = await trainerService.GetTrainerToUpdate(id, ct);
            if (trainer.success)
            {
                return View(trainer.Value);
            }
            TempData["ErrorMessage"] = trainer.error;
            return RedirectToAction(nameof(Index));


        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, UpdateTrainerViewModel model, CancellationToken ct = default)
        {


            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await trainerService.UpdateTrainerAsync(id, model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Trainer Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            return View(model);
        }

        //Delete Trainer
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var trainer = await trainerService.GetTrainerDetailsByIdAsync(id);
            if (trainer is null)
            {
                return NotFound();

            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            var result = await trainerService.DeleteTrainer(id);
            if (result.success)
            {
                TempData["Successmessage"] = "Trainer Deleted Successfully";
                return RedirectToAction(nameof(Index));

            }
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }
    }
}
