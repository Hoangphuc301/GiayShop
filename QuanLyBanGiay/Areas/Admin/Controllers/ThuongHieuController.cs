using Microsoft.AspNetCore.Mvc;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThuongHieuController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        public IActionResult Index()
        {
            ViewBag.ThuongHieu = db.Thuonghieus;
            return View();
        }

        //Thêm
        [HttpGet]
        public ActionResult them()
        {
            return View();
        }
        [HttpPost]
        public ActionResult them(Thuonghieu th)
        {
            db.Thuonghieus.Add(th);
            db.SaveChanges();


            return RedirectToAction("Index");
        }

        //Sửa
        [HttpGet]
        public ActionResult sua(int id)
        {
            var th = db.Thuonghieus.Find(id);
            if (th == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.ThuongHieu = th;
            return View(th);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sua(Thuonghieu th)
        {
            if (ModelState.IsValid)
            {
                var thuonghieu = db.Thuonghieus.Find(th.Math);

                if (thuonghieu == null)
                {
                    ModelState.AddModelError("", "Thương hiệu này không tồn tại hoặc đã bị xóa.");
                    return View();
                }
                thuonghieu.Tenth = th.Tenth;
                thuonghieu.Mota = th.Mota;
                thuonghieu.Logo = th.Logo;

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(th);
        }

        //Xóa
        [HttpGet]
        public IActionResult xoa(int id)
        {
            var th = db.Thuonghieus.Find(id);
            if (th == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.ThuongHieu = th;

            return View();
        }
        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult xoa_Post(int Math)
        {
            var th = db.Thuonghieus.Find(Math);
            if (th == null)
            {
                return RedirectToAction("Index");
            }

            db.Thuonghieus.Remove(th);
            db.SaveChanges();

            TempData["Success"] = "Xóa thương hiệu thành công!";
            return RedirectToAction("Index");
        }

    }
}
