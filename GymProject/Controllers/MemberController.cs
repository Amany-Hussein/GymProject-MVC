using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.PL.Controllers
{
    public class MemberController : Controller
    {


        private readonly IMemberService memberService;

        public MemberController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        // Get :: BaseUrl/Members/Index => List All Members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Member = await memberService.GetAllAsync(ct : ct);
            return View(Member);
        }


        // Get :: BaseUrl/Members/Details{Id} => get specific Member
        public async Task<IActionResult> MemberDetails(int id , CancellationToken ct)
        {
            var member = await memberService.GetMemberDetailsByIdAsync(id , ct);

            if(member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found !";
            }
            return View(member);
            
        }


        // Get :: BaseUrl/Members/HealthRecordDetails{Id} => Get data of specific Member with Healt
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct) 
        {
            var record = await memberService.GetMemberHealthRecord(id, ct);

            if (record is null)
            {
                TempData["ErrorMessage"] = "No Health Record found !";
                return RedirectToAction(nameof(Index));
            }
            else

                return View(record);
        }

        #region Create

        // Get :: BaseUrl/Members/Create => Show Empty Form
        [HttpGet]
        public  IActionResult Create()
        {
            return View();
        }

        // Post :: BaseUrl/Members/Create{Member} => Submit form
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model , CancellationToken ct)
        {
            if(!ModelState.IsValid)
                return View(nameof(Create),model);

            var result = await memberService.CreateMemberAsync(model, ct);

            if (result)
                TempData["SuccesMessage"] = "Member Created Succesfully";
            else
                TempData["ErrorMessage"] = "failed to be creat Member !";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Edit

        // Get :: BaseUrl/Members/Edit => Show Edit Form

        // Post :: BaseUrl/Members/Edit{Member} => Submit Edit form


        #endregion

        #region Delete

        // Get :: BaseUrl/Members/Delete => Show Validation Page

        #endregion
    }
}


