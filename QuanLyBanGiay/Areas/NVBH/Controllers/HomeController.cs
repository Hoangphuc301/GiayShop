using Microsoft.AspNetCore.Mvc;

namespace QuanLyBanGiay.Areas.NVBH.Controllers
{
    [Area("NVBH")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
