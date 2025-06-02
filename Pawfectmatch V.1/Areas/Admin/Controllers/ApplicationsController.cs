using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawfectmatch_V._1.Models;



namespace Pawfectmatch_V._1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ApplicationsController : Controller
    {
        public IActionResult Requests()
        {
            return View();
        }

        public IActionResult Form()
        {
            return View();
        }
    }

}
