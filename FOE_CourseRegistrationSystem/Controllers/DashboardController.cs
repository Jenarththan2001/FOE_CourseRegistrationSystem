using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace FOE_CourseRegistrationSystem.Controllers
{
    public class DashboardController : Controller
    {
        /// <summary>
        /// Student Dashboard - Only accessible by users with the "Student" role.
        /// </summary>
        [Authorize(Roles = "Student")]
        public IActionResult StudentDashboard()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("HandleUnauthorized");
            
            return View();
        }

        /// <summary>
        /// Adviser Dashboard - Only accessible by users with the "Advisor" role.
        /// </summary>
        [Authorize(Roles = "Advisor")]
        public IActionResult AdviserDashboard()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("HandleUnauthorized");
            
            return View();
        }

        /// <summary>
        /// Admin Dashboard - Only accessible by users with the "AR" role.
        /// </summary>
        [Authorize(Roles = "AR")]
        public IActionResult AdminDashboard()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("HandleUnauthorized");
            
            return View();
        }

        /// <summary>
        /// Coordinator Dashboard - Only accessible by users with the "Coordinator" role.
        /// </summary>
        [Authorize(Roles = "Coordinator")]
        public IActionResult CoordinatorDashboard()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("HandleUnauthorized");
            
            return View();
        }

        /// <summary>
        /// Redirect unauthorized users to login page with an error message.
        /// </summary>
        [AllowAnonymous]
        public IActionResult HandleUnauthorized()
        {
            TempData["ErrorMessage"] = "You are not authorized to access this page.";
            return RedirectToAction("Login", "Account");
        }
    }
}
