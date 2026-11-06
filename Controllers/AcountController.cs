using AgriStream_ST10157545_Practium.Data;
using AgriStream_ST10157545_Practium.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AgriStream_ST10157545_Practium.Controllers
{
    public class AcountController : Controller
    {

        private const string AuthScheme = "Cookies";
        private const string InvalidCredentialsErrorMessage = "Invalid username or password.";

        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AcountController(ApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _db = dbContext;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View(new LoginViewModel());
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _db.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Username == model.Username);
            if (user is null ||
                _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, InvalidCredentialsErrorMessage);
                return View(model);
            }
            var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.Role, user.Role?.RoleName ?? AppRoles.Farmer)
    };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthScheme));
            await HttpContext.SignInAsync(AuthScheme, principal);
            return RedirectToAction("Index", "Dashboard");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View(new RegisterViewModel());
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Self-registration is limited to the Farmer and Agronomist roles; Admin accounts are seeded.
            if (model.Role != AppRoles.Farmer && model.Role != AppRoles.Agronomist)
            {
                ModelState.AddModelError(nameof(model.Role), "Please choose a valid role.");
                return View(model);
            }
            if (await _db.Users.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "That username is already taken.");
                return View(model);
            }
            var role = await _db.Roles.SingleAsync(r => r.RoleName == model.Role);
            var user = new User { Username = model.Username, RoleId = role.RoleId };
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(AuthScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
