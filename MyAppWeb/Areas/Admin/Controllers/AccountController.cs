using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Models.ViewModels;

namespace MyAppWeb.Areas.Admin.Controllers
{
    [AllowAnonymous]
    [Area("Admin")]
    public class AccountController : Controller
    {
        public UserManager<ApplicationUser> userManager { get; }
        public SignInManager<ApplicationUser> signInManager { get; }
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Category");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("login", "Account");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Category");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }
        [AcceptVerbs("Get","Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user=await userManager.FindByEmailAsync(email);
            if (user==null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use ");
            }
        }
    }
}
