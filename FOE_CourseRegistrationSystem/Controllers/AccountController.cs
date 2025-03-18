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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOE_CourseRegistrationSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Student> _studentPasswordHasher;
        private readonly PasswordHasher<Staff> _staffPasswordHasher;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _studentPasswordHasher = new PasswordHasher<Student>();
            _staffPasswordHasher = new PasswordHasher<Staff>();
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Console.WriteLine($"🔹 Received Email: {model.Email}");
            Console.WriteLine($"🔹 Received Password: {model.Password}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState is invalid");
                return View(model);
            }

            try
            {
                // Using raw SQL query to avoid EF Core materialization issues with nulls
                var studentQuery = $"SELECT StudentID, Email, Password FROM Student WHERE Email = '{model.Email}'";
                var staffQuery = $"SELECT StaffID, Email, Password, Role FROM Staff WHERE Email = '{model.Email}'";

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = studentQuery;
                    await _context.Database.OpenConnectionAsync();

                    using (var result = await command.ExecuteReaderAsync())
                    {
                        if (await result.ReadAsync())
                        {
                            var studentId = result.GetInt32(0);
                            var email = result.IsDBNull(1) ? null : result.GetString(1);
                            var password = result.IsDBNull(2) ? null : result.GetString(2);

                            if (!string.IsNullOrEmpty(password) && password == model.Password)
                            {
                                await SignInUser(email, "Student", studentId);
                                return RedirectToAction("StudentDashboard", "Dashboard");
                            }
                        }
                    }
                }

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = staffQuery;
                    if (_context.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                        await _context.Database.OpenConnectionAsync();

                    using (var result = await command.ExecuteReaderAsync())
                    {
                        if (await result.ReadAsync())
                        {
                            var staffId = result.GetInt32(0);
                            var email = result.IsDBNull(1) ? null : result.GetString(1);
                            var password = result.IsDBNull(2) ? null : result.GetString(2);
                            var roleValue = result.IsDBNull(3) ? 0 : result.GetInt32(3);
                            var role = (StaffRole)roleValue;

                            if (!string.IsNullOrEmpty(password) && password == model.Password)
                            {
                                string roleName = role.ToString();
                                await SignInUser(email, roleName, staffId);

                                if (role == StaffRole.Advisor)
                                    return RedirectToAction("AdviserDashboard", "Dashboard");
                                else if (role == StaffRole.AR)
                                    return RedirectToAction("AdminDashboard", "Dashboard");
                                else if (role == StaffRole.Coordinator)
                                    return RedirectToAction("CoordinatorDashboard", "Dashboard");
                            }
                        }
                    }
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
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}