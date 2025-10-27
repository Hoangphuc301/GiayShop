using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        //[Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
