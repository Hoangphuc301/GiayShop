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
			ViewBag.Khachhang = db.Khachhangs;
			return View();
		}

        //Thêm
        [HttpGet]
        public ActionResult them()
        {
            return View();
        }
        [HttpPost]
        public ActionResult them(Khachhang tk)
        {
            db.Khachhangs.Add(tk);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
