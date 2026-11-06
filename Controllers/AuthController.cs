using Microsoft.AspNetCore.Mvc;

namespace AgriStream_ST10157545_Practium.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Implement your authentication logic here
            // For example, check the username and password against a database
            // If authentication is successful, redirect to the dashboard or home page
            // return RedirectToAction("Index", "Home");
            // If authentication fails, return to the login page with an error message
            ViewBag.ErrorMessage = "Invalid username or password.";
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string password)
        {
            // Implement your registration logic here
            // For example, save the user details to a database
            // After successful registration, redirect to the login page
            // return RedirectToAction("Login");
            // If registration fails, return to the register page with an error message
            ViewBag.ErrorMessage = "Registration failed. Please try again.";
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }


        public IActionResult ResetPassword()
        {
            return View();
        }

        public IActionResult Logout()
        {
            // Implement your logout logic here
            // For example, clear the user session or authentication cookie
            // After logout, redirect to the login page
            return RedirectToAction("Login");
        }
    }
}
