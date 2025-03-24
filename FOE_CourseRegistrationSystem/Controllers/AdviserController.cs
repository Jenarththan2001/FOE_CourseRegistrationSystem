using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOE_CourseRegistrationSystem.Controllers
{
    [Authorize(Roles = "Adviser")]
    public class AdviserController : Controller
    {
        public IActionResult AdviserDashboard()
        {
            ViewBag.Title = "Adviser Dashboard";
            return View("~/Views/Dashboard/Adviser/AdviserDashboard.cshtml");
        }

        public IActionResult ResultPage()
        {
            return View("~/Views/Dashboard/Adviser/ResultPage.cshtml");
        }

        public IActionResult CourseRegistration()
        {
            return View("~/Views/Dashboard/Adviser/CourseRegistration.cshtml");
        }

        public IActionResult RegisteredCourse()
        {
            return View("~/Views/Dashboard/Adviser/RegisteredCourse.cshtml");
        }

        public IActionResult RegisterNewCourse()
        {
            return View("~/Views/Dashboard/Adviser/RegisterNewCourse.cshtml");
        }
    }
}
