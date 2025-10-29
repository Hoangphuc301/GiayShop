using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ProjectModel;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanphamController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        public IActionResult Index()
        {
            var sanphams = db.Sanphams
                .Include(sp => sp.MadmNavigation)
                .Include(sp => sp.MathNavigation)
                .ToList();
            ViewBag.Sanpham = db.Sanphams;
            return View();
        }

        //Thêm
        [HttpGet]
        public ActionResult them()
        {
            ViewBag.DanhmucList = db.Danhmucs.ToList();
            ViewBag.ThuonghieuList = db.Thuonghieus.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult them(Sanpham sp, IFormFile LogoFile)
        {
            if (LogoFile != null && LogoFile.Length > 0)
            {
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Sanpham");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var fileName = Path.GetFileName(LogoFile.FileName);

                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    LogoFile.CopyTo(stream);
                }
                sp.Hinhdaidien = fileName;
            }

            db.Sanphams.Add(sp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Sửa
        [HttpGet]
        public ActionResult sua(int id)
        {
            var sp = db.Sanphams.Find(id);
            if (sp == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.DanhmucList = db.Danhmucs.ToList();
            ViewBag.ThuonghieuList = db.Thuonghieus.ToList();
            ViewBag.Sanpham = sp;
            return View(sp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sua(Sanpham sp, IFormFile? LogoFile)
        {
            System.Diagnostics.Debug.WriteLine("MASP POST: " + sp.Masp);

            var sanpham = db.Sanphams.Find(sp.Masp);
            if (sanpham == null)
            {
                ModelState.AddModelError("", "Sản phẩm này không tồn tại hoặc đã bị xóa.");
                return View(sp);
            }

            if (LogoFile != null && LogoFile.Length > 0)
            {
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Sanpham");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var fileName = Path.GetFileName(LogoFile.FileName);
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    LogoFile.CopyTo(stream);
                }
                sanpham.Hinhdaidien = fileName;
            }
            sanpham.Madm = sp.Madm;
            sanpham.Math = sp.Math;
            sanpham.Tensp = sp.Tensp;
            sanpham.Gia = sp.Gia;
            sanpham.Mota = sp.Mota;
            sanpham.Trangthai = sp.Trangthai;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult xoa(int id)
        {
            var sp = db.Sanphams.Find(id);
            if (sp == null)
            {
                return RedirectToAction("Index");
            }
			ViewBag.DanhmucList = db.Danhmucs.ToList();
			ViewBag.ThuonghieuList = db.Thuonghieus.ToList();
			ViewBag.Sanpham = sp;
            return View(sp);
        }

        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public ActionResult XacNhanXoa(int id)
        {
            var sp = db.Sanphams.Find(id);
            if (sp == null)
            {
                ModelState.AddModelError("", "Sản phẩm không tồn tại hoặc đã bị xóa.");
                return RedirectToAction("Index");
            }

            db.Sanphams.Remove(sp);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
