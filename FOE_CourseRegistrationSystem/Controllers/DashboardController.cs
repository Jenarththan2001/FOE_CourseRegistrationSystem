﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOE_CourseRegistrationSystem.Controllers
{
    public class DashboardController : Controller
    {
        [Authorize(Roles = "Student")]
        public IActionResult StudentDashboard() => View();

        [Authorize(Roles = "Adviser")]
        public IActionResult AdviserDashboard() => View();

        [Authorize(Roles = "AR")]
        public IActionResult AdminDashboard() => View();
    }
}
