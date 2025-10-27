using Microsoft.AspNetCore.Mvc;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MauController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        public IActionResult Index()
        {
            ViewBag.Mau = db.Maus;
            return View();
        }

        //Thêm
        [HttpGet]
        public ActionResult them()
        {
            return View();
        }
        [HttpPost]
        public ActionResult them(Mau m)
        {
            db.Maus.Add(m);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Sửa
        [HttpGet]
        public ActionResult sua(int id)
        {
            var m = db.Maus.Find(id);
            if (m == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Mau = m;
            return View(m);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sua(Mau m)
        {
            if (ModelState.IsValid)
            {
                var dm = db.Maus.Find(m.Mamau);

                if (dm == null)
                {
                    ModelState.AddModelError("", "Màu này không tồn tại hoặc đã bị xóa.");
                    return View();
                }
                dm.Tenmau = m.Tenmau;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(m);
        }

        //Xóa
        [HttpGet]
        public IActionResult xoa(int id)
        {
            var m = db.Maus.Find(id);
            if (m == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Mau = m;

            return View();
        }
        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult xoa_Post(int Mamau)
        {
            var m = db.Maus.Find(Mamau);
            if (m == null)
            {
                return RedirectToAction("Index");
            }

            db.Maus.Remove(m);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
