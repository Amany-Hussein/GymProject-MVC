using GymManagement.BLL.ViewModels.AccountViewModel;
using GymManagement.DAL.Models;
using GymProject.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)

        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        //get :: empty form
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //post :: SignIn
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) 
                return View(model);

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password");
                return View(model);
            }

            //signin
            var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("InvalidLogin", "This Account Locked Out , Try Again Later");
                return View(model);

            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password");
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
