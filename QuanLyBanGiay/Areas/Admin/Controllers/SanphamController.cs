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
        //Hiển thị danh sách với phân trang
        public IActionResult Index(int trang = 1) 
        {
            int kichThuocTrang = 10; 
            var sanpham = db.Sanphams
                .Include(sp => sp.MadmNavigation)
                .Include(sp => sp.MathNavigation)
                .AsQueryable(); 

            int tongSoMuc = sanpham.Count();
            int tongSoTrang = (int)Math.Ceiling((double)tongSoMuc / kichThuocTrang);

            if (trang < 1) 
                trang = 1;
            if (trang > tongSoTrang) 
                trang = tongSoTrang;

            var sanphams = sanpham
                .OrderByDescending(sp => sp.Masp)
                .Skip((trang - 1) * kichThuocTrang) 
                .Take(kichThuocTrang)
                .ToList();

            ViewBag.Sanpham = sanphams;
            ViewBag.TrangHienTai = trang;  
            ViewBag.TongSoTrang = tongSoTrang; 
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

        //xóa
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
