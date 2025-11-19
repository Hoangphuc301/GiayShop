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
            var taikhoan = db.Khachhangs.FirstOrDefault(t => t.Email == email);
            if (taikhoan == null)
                return NotFound();
            // Tìm khách hàng theo mã tài khoản
            var khach = db.Khachhangs.FirstOrDefault(k => k.Makh == taikhoan.Makh);
            if (khach == null)
            {
                khach = new Khachhang
                {
                    Makh = taikhoan.Makh,
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

            var taikhoan = db.Khachhangs.FirstOrDefault(t => t.Email == email);
            if (taikhoan == null)
                return NotFound();

            var khach = db.Khachhangs.FirstOrDefault(k => k.Makh == taikhoan.Makh);
            if (khach == null)
            {
                khach = new Khachhang { Makh = taikhoan.Makh };
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

        //Xem đơn hàng
        [HttpGet]
        public IActionResult XemDH()
        {
            // Lấy email đang đăng nhập
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null)
                return RedirectToAction("Login", "Account");

            // Lấy khách hàng theo email
            var khach = db.Khachhangs.FirstOrDefault(k => k.Email == email);
            if (khach == null)
                return NotFound();

            // Lấy danh sách đơn hàng của khách đó
            var donhang = db.Donhangs
                            .Where(d => d.Makh == khach.Makh)
                            .OrderByDescending(d => d.Ngaydat)
                            .ToList();

            return View(donhang);
        }

        //Xem chi tiết đơn hàng
        [HttpGet]
        public IActionResult ChiTietDH(int id)
        {
            var don = db.Donhangs
                .Include(d => d.ChitietDonhangs)
                    .ThenInclude(ct => ct.MactspNavigation)
                        .ThenInclude(ctsp => ctsp.MaspNavigation) 
                .Include(d => d.ChitietDonhangs)
                    .ThenInclude(ct => ct.MactspNavigation)
                        .ThenInclude(ctsp => ctsp.MamauNavigation) 

                .Include(d => d.ChitietDonhangs)
                    .ThenInclude(ct => ct.MactspNavigation)
                        .ThenInclude(ctsp => ctsp.MasizeNavigation)

                .FirstOrDefault(d => d.Madh == id);

            if (don == null)
                return NotFound();
            return View(don);
        }

        [HttpGet]
        public IActionResult Huy(int id)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null)
                return RedirectToAction("Login", "Account");

            var dh = db.Donhangs
                        .FirstOrDefault(d => d.Madh == id);

            if (dh == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng này.";
                return RedirectToAction("XemDH");
            }

            var khach = db.Khachhangs.FirstOrDefault(k => k.Email == email);
            if (dh.Makh != khach?.Makh)
            {
                TempData["Error"] = "Bạn không có quyền hủy đơn hàng này.";
                return RedirectToAction("XemDH");
            }

            if (dh.Trangthai != "Chờ xác nhận")
            {
                TempData["Error"] = $"Đơn hàng #{dh.Madh} đang ở trạng thái '{dh.Trangthai}' và không thể hủy.";
                return RedirectToAction("XemDH");
            }

            if (khach != null)
            {
                dh.Lydohuy = $"Khách hàng {khach.Tenkh} yêu cầu hủy đơn.";
            }

            return View(dh);
        }
        [HttpPost, ActionName("Huy")]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanHuyDonHang(int Madh, string Lydohuy)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var khach = db.Khachhangs.FirstOrDefault(k => k.Email == email);

            var dh = db.Donhangs
                        .Include(d => d.ChitietDonhangs)
                        .FirstOrDefault(d => d.Madh == Madh);

            if (dh == null || dh.Makh != khach?.Makh || dh.Trangthai != "Chờ xác nhận")
            {
                TempData["Error"] = "Không thể hủy đơn hàng này do không tồn tại hoặc trạng thái không hợp lệ.";
                return RedirectToAction("XemDH");
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    dh.Trangthai = "HỦY";
                    dh.Lydohuy = !string.IsNullOrWhiteSpace(Lydohuy) ? Lydohuy : "Khách hàng yêu cầu hủy đơn.";

                    foreach (var ct in dh.ChitietDonhangs)
                    {
                        var ctsp = db.ChitietSanphams.Find(ct.Mactsp);
                        if (ctsp != null)
                        {
                            ctsp.Slton += ct.Sl; 
                        }
                    }
                    db.SaveChanges(); 
                    transaction.Commit();

                    TempData["Success"] = $"Đã hủy đơn hàng #{dh.Madh} thành công.";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Error"] = "Lỗi hệ thống khi hủy đơn: " + (ex.InnerException?.Message ?? ex.Message);
                }
            }
            return RedirectToAction("XemDH");
        }
    }
}
