using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;
using System.Security.Claims;

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
            // Lấy loại tài khoản trong session
            var role = HttpContext.Session.GetString("UserRole");

            // Kiểm tra có phải NVGH không
            if (role == null || role != "NVGH")
                return null;

            // Lấy ID của người dùng hiện tại → dùng làm MaNvg
            return HttpContext.Session.GetInt32("UserId");
        }


        public IActionResult Index(int trang = 1, string loaiLoc = "", DateTime? from = null, DateTime? to = null)
        {
            var maNvg = GetMaNvgHienTai();
            if (!maNvg.HasValue)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập tài khoản nhân viên giao hàng!";
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            int kichThuocTrang = 10;

            var query = _context.Donhangs
                .Include(d => d.MakhNavigation)
                .Include(d => d.MaptttNavigation)
                .Include(d => d.MavoucherNavigation)
                .Where(d => d.MaNvg == maNvg)
                .Where(d => d.Trangthai == "ĐANG GIAO" || d.Trangthai == "HỦY")
                .AsQueryable();

            // Lọc thời gian
            if (loaiLoc == "week")
            {
                var dauTuan = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var cuoiTuan = dauTuan.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);
                query = query.Where(d => d.Ngaydat >= dauTuan && d.Ngaydat <= cuoiTuan);
            }
            else if (loaiLoc == "month")
            {
                var dauThang = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var cuoiThang = dauThang.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);
                query = query.Where(d => d.Ngaydat >= dauThang && d.Ngaydat <= cuoiThang);
            }
            else if (loaiLoc == "range" && from.HasValue && to.HasValue)
            {
                var tuNgay = from.Value.Date;
                var denNgay = to.Value.Date.AddHours(23).AddMinutes(59);
                query = query.Where(d => d.Ngaydat >= tuNgay && d.Ngaydat <= denNgay);
            }

            ViewBag.LoaiLoc = loaiLoc;
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            int tong = query.Count();
            int tongTrang = (int)Math.Ceiling(tong / (double)kichThuocTrang);
            if (tongTrang == 0) tongTrang = 1;
            trang = Math.Clamp(trang, 1, tongTrang);

            var donhangs = query
                .OrderByDescending(d => d.Ngaydat)
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToList();

            ViewBag.Donhang = donhangs;
            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongTrang;

            return View();
        }

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
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng hoặc bạn không có quyền!";
                return RedirectToAction("Index");
            }

            dh.Trangthai = "ĐÃ NHẬN";
            _context.SaveChanges();

            if (dh.MaptttNavigation?.Tenphuongthuc?.Contains("COD", StringComparison.OrdinalIgnoreCase) == true)
            {
                TempData["InfoMessage"] = $"Đã thu tiền mặt {dh.Tongtiencuoi:N0} ₫. Nhớ nộp lại cho cửa hàng nhé!";
            }

            TempData["SuccessMessage"] = $"Đơn hàng #{dh.Madh} đã giao thành công!";
            return RedirectToAction("Index");
        }

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