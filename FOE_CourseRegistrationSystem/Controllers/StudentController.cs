using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOE_CourseRegistrationSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        public IActionResult Dashboard()
        {
            return View("~/Views/Dashboard/Student/StudentDashboard.cshtml");
        }

        public IActionResult ResultPage()
        {
            return View("~/Views/Dashboard/Student/ResultPage.cshtml");
        }

        public IActionResult CourseRegistration()
        {
            return View("~/Views/Dashboard/Student/CourseRegistration.cshtml");
        }

        public IActionResult RegisteredCourse()
        {
            return View("~/Views/Dashboard/Student/RegisteredCourse.cshtml");
        }

        public IActionResult RegisterNewCourse()
        {
            return View("~/Views/Dashboard/Student/RegisterNewCourse.cshtml");
        }
    }
}
