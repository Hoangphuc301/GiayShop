using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.NVGH.Controllers
{
    [Area("NVGH")]
    public class DonhangController : Controller
    {
        private readonly QL_GiayContext _context;

        public DonhangController(QL_GiayContext context)
        {
            _context = context;
        }

        private int? GetMaNvgHienTai()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == null || role != "NVGH")
                return null;

            return HttpContext.Session.GetInt32("UserId");
        }

        public IActionResult Index(string loai = "dang-giao")
        {
            var maNvg = GetMaNvgHienTai();
            if (!maNvg.HasValue)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập tài khoản nhân viên giao hàng!";
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            var query = _context.Donhangs
                .Include(d => d.MakhNavigation)
                .Include(d => d.MaptttNavigation)
                .Where(d => d.MaNvg == maNvg)
                .AsQueryable();

            // === LỌC THEO YÊU CẦU MỚI ===
            if (loai == "dang-giao")
            {
                query = query.Where(d =>
                    d.Trangthai == "ĐANG GIAO"  // đơn đang giao
                    || (d.Trangthai == "ĐÃ NHẬN"
                         && d.MaptttNavigation.Tenphuongthuc.Contains("COD")
                         && d.DaNopTienCOD == false) // COD chưa nộp tiền vẫn tính đang giao
                );
            }
            else if (loai == "da-giao-huy")
            {
                query = query.Where(d =>
                    d.Trangthai == "HỦY"
                    || (d.Trangthai == "ĐÃ NHẬN"
                         && (
                             !d.MaptttNavigation.Tenphuongthuc.Contains("COD")
                             || d.DaNopTienCOD == true
                         )
                    )
                );
            }

            ViewBag.Loai = loai;
            ViewBag.Donhang = query.OrderByDescending(d => d.Ngaydat).ToList();

            return View();
        }


        // =============================
        // XÁC NHẬN GIAO THÀNH CÔNG
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanGiaoThanhCong(int Madh)
        {
            var maNvg = GetMaNvgHienTai();
            if (!maNvg.HasValue)
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập hết hạn.";
                return RedirectToAction("Index");
            }

            var dh = _context.Donhangs
                .Include(d => d.MaptttNavigation)
                .FirstOrDefault(d => d.Madh == Madh && d.MaNvg == maNvg && d.Trangthai == "ĐANG GIAO");

            if (dh == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng hoặc bạn không có quyền xác nhận";
                return RedirectToAction("Index");
            }

            dh.Trangthai = "ĐÃ NHẬN";
            _context.SaveChanges();

            if (dh.MaptttNavigation?.Tenphuongthuc.Contains("COD") == true)
            {
                TempData["InfoMessage"] = $"Thu hộ: {dh.Tongtiencuoi:N0} ₫. Vui lòng nộp lại tiền cho cửa hàng.";
            }

            TempData["SuccessMessage"] = $"Đơn hàng #{dh.Madh} đã giao thành công!";
            return RedirectToAction("Index");
        }


        // =============================
        // XÁC NHẬN NỘP TIỀN COD
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanNopTienCOD(int Madh)
        {
            var maNvg = GetMaNvgHienTai();
            if (!maNvg.HasValue)
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập hết hạn!";
                return RedirectToAction("Index");
            }

            var dh = _context.Donhangs
                .Include(d => d.MaptttNavigation)
                .FirstOrDefault(d => d.Madh == Madh
                                  && d.MaNvg == maNvg
                                  && d.Trangthai == "ĐÃ NHẬN"
                                  && d.MaptttNavigation.Tenphuongthuc.Contains("COD")
                                  && d.DaNopTienCOD == false);

            if (dh == null)
            {
                TempData["ErrorMessage"] = "Đơn hàng không hợp lệ hoặc đã nộp tiền rồi.";
                return RedirectToAction("Index");
            }

            dh.DaNopTienCOD = true;
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Nộp tiền đơn COD #{dh.Madh} thành công!";
            return RedirectToAction("Index");
        }


        // =============================
        // CHI TIẾT ĐƠN
        // =============================
        public IActionResult XemChiTiet(int id)
        {
            var maNvg = GetMaNvgHienTai();
            if (!maNvg.HasValue) return RedirectToAction("Index");

            var dh = _context.Donhangs
                .Include(d => d.MakhNavigation)
                .FirstOrDefault(d => d.Madh == id && d.MaNvg == maNvg);

            if (dh == null) return NotFound();

            var ct = _context.ChitietDonhangs
                .Include(c => c.MactspNavigation)
                    .ThenInclude(p => p.MaspNavigation)
                .Include(c => c.MactspNavigation.MamauNavigation)
                .Include(c => c.MactspNavigation.MasizeNavigation)
                .Where(c => c.Madh == id)
                .ToList();

            ViewBag.DonHang = dh;
            return View(ct);
        }
    }
}
