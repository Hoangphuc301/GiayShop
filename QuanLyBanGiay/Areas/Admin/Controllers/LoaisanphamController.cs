using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoaisanphamController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        public IActionResult Index()
        {
            ViewBag.Loaisanpham = db.Danhmucs;
            return View();
        }

        //Thêm
        [HttpGet]
        public ActionResult them()
        {
            return View();
        }
        [HttpPost]
        public ActionResult them(Danhmuc loaisanpham)
        {
            db.Danhmucs.Add(loaisanpham);
            db.SaveChanges();


            return RedirectToAction("Index");
        }

        //Sửa
        [HttpGet]
        public ActionResult sua(int id)
        {
            var dm = db.Danhmucs.Find(id);
            if (dm == null)
            {
                return RedirectToAction("Index");
            }     
            ViewBag.Loaisanpham = dm;
            return View(dm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sua(Danhmuc dm)
        {
            if (ModelState.IsValid)
            {
                var danhmuc = db.Danhmucs.Find(dm.Madm);

                if (danhmuc == null) 
                {
                    ModelState.AddModelError("", "Danh mục này không tồn tại hoặc đã bị xóa.");
                    return View();
                }
                danhmuc.Tendm = dm.Tendm;
                danhmuc.Mota = dm.Mota;

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(dm);
        }

        //Xóa
        [HttpGet]
        public IActionResult xoa(int id)
        {
            var dm = db.Danhmucs.Find(id);
            if (dm == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Loaisanpham = dm;

            return View();
        }
        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult xoa_Post(int Madm)
        {
            var dm = db.Danhmucs.Find(Madm);
            if (dm == null)
            {
                return RedirectToAction("Index");
            }

            db.Danhmucs.Remove(dm);
            db.SaveChanges();

            TempData["Success"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }

    }
}
