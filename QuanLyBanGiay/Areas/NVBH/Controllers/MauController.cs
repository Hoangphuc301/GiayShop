using Microsoft.AspNetCore.Mvc;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.NVBH.Controllers
{
    [Area("NVBH")]
    public class MauController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        public IActionResult Index(int trang = 1)
        {
            int kichthuoctrang = 7;
            var mau = db.Maus.AsQueryable();

            int tongSoMuc = mau.Count();
            int tongSoTrang = (int)Math.Ceiling((double)tongSoMuc / kichthuoctrang);

            if (trang < 1)
                trang = 1;
            if (trang > tongSoTrang)
                trang = tongSoTrang;

            var maus = mau
                .OrderByDescending(m => m.Mamau)
                .Skip((trang - 1) * kichthuoctrang)
                .Take(kichthuoctrang)
                .ToList();

            ViewBag.Mau = maus;
            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;
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
