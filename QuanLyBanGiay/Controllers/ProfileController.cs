using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;
using QuanLyBanGiay.Models.ViewModels;

namespace QuanLyBanGiay.Controllers
{
    public class ProfileController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();

        [HttpGet]
        public IActionResult Profile()
        {
            // Lấy email từ session
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null)
                return RedirectToAction("Login", "Account");
            // Tìm tài khoản theo email
            var taikhoan = db.Taikhoans.FirstOrDefault(t => t.Email == email);
            if (taikhoan == null)
                return NotFound();
            // Tìm khách hàng theo mã tài khoản
            var khach = db.Khachhangs.FirstOrDefault(k => k.Matk == taikhoan.Matk);
            if (khach == null)
            {
                khach = new Khachhang
                {
                    Matk = taikhoan.Matk,
                    Tenkh = taikhoan.Email.Split('@')[0], //Lấy phần tên trước @ làm tên mặc định
                    Sdt = null,
                    Diachi = null
                };
                db.Khachhangs.Add(khach);
                db.SaveChanges();
            }
            
            var model = new ProfileViewModel
            {
                Makh = khach.Makh,
                Tenkh = khach.Tenkh,
                Email = taikhoan.Email,
                Sdt = khach.Sdt
            };

            // Nếu có địa chỉ trong DB thì tự động tách ra
            if (!string.IsNullOrEmpty(khach.Diachi))
            {
                model.Diachi = khach.Diachi;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null)
                return RedirectToAction("Login", "Account");

            var taikhoan = db.Taikhoans.FirstOrDefault(t => t.Email == email);
            if (taikhoan == null)
                return NotFound();

            var khach = db.Khachhangs.FirstOrDefault(k => k.Matk == taikhoan.Matk);
            if (khach == null)
            {
                khach = new Khachhang { Matk = taikhoan.Matk };
                db.Khachhangs.Add(khach);
            }

            // Cập nhật thông tin
            khach.Tenkh = model.Tenkh;
            khach.Sdt = model.Sdt;
            khach.Diachi = model.Diachi; 

            db.SaveChanges();

            ViewBag.Message = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }
    }
}
