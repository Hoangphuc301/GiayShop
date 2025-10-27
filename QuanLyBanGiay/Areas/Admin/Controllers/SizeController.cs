using Microsoft.AspNetCore.Mvc;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        public IActionResult Index()
        {
            ViewBag.Size = db.Sizes;
            return View();
        }

        //Thêm
        [HttpGet]
        public ActionResult them()
        {
            return View();
        }
        [HttpPost]
        public ActionResult them(Size s)
        {
            db.Sizes.Add(s);
            db.SaveChanges();


            return RedirectToAction("Index");
        }

        //Sửa
        [HttpGet]
        public ActionResult sua(int id)
        {
            var s = db.Sizes.Find(id);
            if (s == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Size = s;
            return View(s);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sua(Size s)
        {
            if (ModelState.IsValid)
            {
                var size = db.Sizes.Find(s.Masize);

                if (size == null)
                {
                    ModelState.AddModelError("", "Số size này không tồn tại hoặc đã bị xóa.");
                    return View();
                }
                size.Tensize = s.Tensize;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(s);
        }

        //Xóa
        [HttpGet]
        public IActionResult xoa(int id)
        {
            var s = db.Sizes.Find(id);
            if (s == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Size = s;
            return View();
        }
        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult xoa_Post(int Masize)
        {
            var s = db.Sizes.Find(Masize);
            if (s == null)
            {
                return RedirectToAction("Index");
            }

            db.Sizes.Remove(s);
            db.SaveChanges();

            TempData["Success"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }
    }
}
