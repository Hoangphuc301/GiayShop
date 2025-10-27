using Microsoft.AspNetCore.Mvc;

namespace QuanLyBanGiay.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult FormRegister()
        {

            return View();
        }
    }
}
