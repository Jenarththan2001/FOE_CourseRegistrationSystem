using FOE_CourseRegistrationSystem.Data;
using FOE_CourseRegistrationSystem.Models;
using FOE_CourseRegistrationSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var student = await _context.Students
                .Where(s => s.Email == model.Email && s.Password != null)  // ✅ Prevent NULL Errors
                .FirstOrDefaultAsync();

            var staff = await _context.Staffs
                .Where(s => s.Email == model.Email && s.Password != null)  // ✅ Prevent NULL Errors
                .FirstOrDefaultAsync();

            if (student != null)
            {
                Console.WriteLine($"✅ Found Student: {student.Email}");

                if (!string.IsNullOrEmpty(student.Password) && student.Password == model.Password)
                {
                    await SignInUser(student.Email, "Student");
                    return RedirectToAction("StudentDashboard", "Dashboard");
                }
                else
                {
                    Console.WriteLine("❌ Student Password Verification Failed");
                }
            }
            else
            {
                Console.WriteLine("❌ Student Not Found");
            }

            if (staff != null)
            {
                Console.WriteLine($"✅ Found Staff: {staff.Email}");

                if (!string.IsNullOrEmpty(staff.Password) && staff.Password == model.Password)
                {
                    Console.WriteLine($"✅ Staff Verified");

                    if (staff.Role == StaffRole.Advisor)  // ✅ Compare using Enum
                    {
                        await SignInUser(staff.Email, "Adviser");
                        Console.WriteLine("🔹 Redirecting to AdviserDashboard...");
                        return RedirectToAction("AdviserDashboard", "Dashboard");
                    }
                    else if (staff.Role == StaffRole.AR)  // ✅ Compare using Enum
                    {
                        await SignInUser(staff.Email, "AR");
                        Console.WriteLine("🔹 Redirecting to AdminDashboard...");
                        return RedirectToAction("AdminDashboard", "Dashboard");
                    }
                }

                else
                {
                    Console.WriteLine("❌ Staff Password Verification Failed");
                }
            }


            Console.WriteLine("❌ Login Failed");
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }



        private async Task SignInUser(string email, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
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
