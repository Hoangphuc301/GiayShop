using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanphamController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        public IActionResult Index()
        {
            ViewBag.Sanpham = db.Sanphams;
            return View();
        }

        //Thêm
        [HttpGet]
        public ActionResult them()
        {
            return View();
        }
        [HttpPost]
        public ActionResult them(Sanpham sp)
        {
            db.Sanphams.Add(sp);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
