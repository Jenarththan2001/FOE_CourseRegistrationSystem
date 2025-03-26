using FOE_CourseRegistrationSystem.Data;
using FOE_CourseRegistrationSystem.Models;
using FOE_CourseRegistrationSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOE_CourseRegistrationSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Console.WriteLine($"🔹 Received Email: {model.Email}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState is invalid");
                return View(model);
            }

            try
            {
                // 🔹 Validate student login
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Email == model.Email && s.Password == model.Password);

                if (student != null)
                {
                    await SignInUser(student.Email, "Student", student.StudentID);
                    return RedirectToAction("Dashboard", "Student");
                }

                // 🔹 Validate staff login
                var staff = await _context.Staffs
                    .FirstOrDefaultAsync(st => st.Email == model.Email && st.Password == model.Password);

                if (staff != null)
                {
                    string roleName = staff.Role.ToString(); // No manual renaming
                    await SignInUser(staff.Email, roleName, staff.StaffID);

                    return roleName switch
                    {
                        "Advisor" => RedirectToAction("AdviserDashboard", "Advisor"),
                        "AR" => RedirectToAction("AdminDashboard", "Admin"),
                        "Coordinator" => RedirectToAction("CoordinatorDashboard", "Coordinator"),
                        _ => RedirectToAction("Login", "Account"),
                    };

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception during login: {ex.Message}");
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }

            Console.WriteLine("❌ Login Failed");
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        private async Task SignInUser(string email, string role, int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email ?? ""),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            Console.WriteLine($"✔️ Logging in with role: {role}");

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
