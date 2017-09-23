using System.Threading.Tasks;
using GoldenForCongress.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GoldenForCongress.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DB _db;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, DB db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(username, password, true, false);
            if (signInResult.Succeeded)
                return RedirectToAction("Index", "Admin");

            ViewData["Errors"] = "Login failed";
            return View();
        }


        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            _userManager.PasswordValidators.Clear();
            var registerResult = await _userManager.CreateAsync(new IdentityUser(username), password);
            if (registerResult.Succeeded)
                return RedirectToAction("Index", "Admin");

            ViewData["Errors"] = registerResult.Errors;
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByNameAsync(id);
            await _db.UserRoles.AddAsync(new IdentityUserRole<string> {UserId = user.Id, RoleId = "Admin"});
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult AccessDenied() => View();
    }
}