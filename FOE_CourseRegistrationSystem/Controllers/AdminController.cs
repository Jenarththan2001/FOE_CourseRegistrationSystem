using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOE_CourseRegistrationSystem.Controllers
{
    [Authorize(Roles = "AR")]
    public class AdminController : Controller
    {
        public IActionResult AdminDashboard()
        {
            ViewBag.Title = "Admin Dashboard";
            return View("~/Views/Dashboard/Admin/AdminDashboard.cshtml");
        }

        public IActionResult RegistrationDetails()
        {
            return View("~/Views/Dashboard/Admin/RegistrationDetails.cshtml");
        }

        public IActionResult CourseOffering()
        {
            return View("~/Views/Dashboard/Admin/CourseOffering.cshtml");
        }

        public IActionResult AdvisorDetails()
        {
            return View("~/Views/Dashboard/Admin/AdvisorDetails.cshtml");
        }
    }
}
