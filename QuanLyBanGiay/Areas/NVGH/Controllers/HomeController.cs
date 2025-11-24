using Microsoft.AspNetCore.Mvc;

namespace QuanLyBanGiay.Areas.NVGH.Controllers
{
    [Area("NVGH")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
