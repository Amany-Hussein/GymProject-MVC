using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymProject.PL.Controllers
{
    //[Authorize]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class MembershipController : Controller
    {

        private readonly IMembershipService memberShipService;

        public MembershipController(IMembershipService memberShipService)
        {
            this.memberShipService = memberShipService;
        }

        // Get all Membership
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await memberShipService.GetAllMemberShipsAsync(ct);
            if (result.success)
            {

                return View(result.Value);
            }
            TempData["ErrorMessage"] = result.error;
            return View(Enumerable.Empty<MembershipViewModel>());

        }

        // Create new Membership
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await DropDownLists();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                await DropDownLists();
                return View(model);
            }
            var result = await memberShipService.CreateMemberShipAsync(model, ct);
            if (!result.success)
            {

                TempData["ErrorMessage"] = result.error;
                await DropDownLists();
                return View(model);
            }
            TempData["SuccessMessage"] = "MemberShip  Created  Successfully";
            return RedirectToAction(nameof(Index));

        }


        // Delete Membership
        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var result = await memberShipService.DeleteActiveMemberShipp(id);
            if (result.success)
            {
                TempData["SuccessMessage"] = "MemberShip Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));

        }

        // Helper Method
        private async Task DropDownLists()
        {
            var result = await memberShipService.GetMembersForDropDownList();
            if (result.success)
            {
                ViewBag.Members = new SelectList(result.Value, "Id", "Name");
            }
            else
            {
                ViewBag.Members = new SelectList(Enumerable.Empty<MemberSelectListViewModel>(), "Id", "Name");
            }

            var result2 = await memberShipService.GetPlansForDropDownList();
            if (result2.success)
            {
                ViewBag.Plans = new SelectList(result2.Value, "Id", "Name");
            }
            else
            {
                ViewBag.Plans = new SelectList(Enumerable.Empty<PlanSelectListViewModel>(), "Id", "Name");
            }

        }

    }
}
