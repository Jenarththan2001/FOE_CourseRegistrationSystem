using Microsoft.AspNetCore.Mvc;
using FOE_CourseRegistrationSystem.Data;
using FOE_CourseRegistrationSystem.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FOE_CourseRegistrationSystem.Models.ViewModels;

namespace FOE_CourseRegistrationSystem.Controllers
{
    [ApiController]  // ✅ Marks this as an API controller (No Views, only JSON responses)
    [Route("api/auth")]  // ✅ This defines `/api/auth` as the base route
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]  // ✅ This makes `POST /api/auth/login` available
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == model.Email);
            if (staff == null || staff.Password != model.Password)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            return Ok(new { message = "Login successful", role = staff.Role });
        }
    }
}
