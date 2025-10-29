using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaikhoanController : Controller
	{
		private QL_GiayContext db = new QL_GiayContext();
		public IActionResult Index()
		{
			ViewBag.Taikhoan = db.Taikhoans;
			return View();
		}

        //Thêm
        [HttpGet]
        public ActionResult them()
        {
            return View();
        }
        [HttpPost]
        public ActionResult them(Taikhoan tk)
        {
            db.Taikhoans.Add(tk);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
