using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Models.Classes;
using MiniTwitter.ViewModels;

namespace MiniTwitter.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, ILogger<AccountController> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this._logger = logger;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    _logger.LogInformation($"Attempting to sign in user: {user.UserName}");

                    var result = await signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User {user.UserName} signed in successfully.");
                        return RedirectToAction("Index", "Home");
                    }
                    _logger.LogInformation($"Failed login attempt for user: {user.UserName}");
                }
                ModelState.AddModelError("", "Invalid email or password.");
            }
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingDisplayName = await userManager.Users
                    .AnyAsync(u => u.DisplayName == model.UserName);

                if (existingDisplayName)
                {
                    _logger.LogInformation($"Registration attempt with taken display name: {model.UserName}");
                    ModelState.AddModelError("UserName", "This display name is already taken.");
                    return View(model);
                }

                User user = new()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    DisplayName = model.UserName
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {user.UserName} registered successfully.");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogWarning($"CreateAsync error for user {user.UserName}: {error.Code} - {error.Description}");
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            return View(model);
        }
        
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }
    }
}
