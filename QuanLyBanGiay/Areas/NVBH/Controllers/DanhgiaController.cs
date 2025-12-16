using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("NVBH")]
    public class DanhgiaController : Controller
    {
        private readonly QL_GiayContext _db;

        public DanhgiaController(QL_GiayContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int? sao, DateTime? from, DateTime? to)
        {
            // 1. Khởi tạo query
            var query = _db.Danhgias
                .Include(d => d.Sanpham)
                .Include(d => d.Khachhang)
                .AsQueryable();

            // 2. Lọc theo ngày trước (để tính số lượng sao chính xác trong khoảng thời gian đó)
            if (from.HasValue)
            {
                // Reset giờ về 00:00:00
                var fromDate = from.Value.Date;
                query = query.Where(d => d.NgayDanhGia >= fromDate);
            }
            if (to.HasValue)
            {
                // Lấy đến cuối ngày 23:59:59
                var toDate = to.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.NgayDanhGia <= toDate);
            }

            // 3. Tính toán số lượng cho các nút bấm (Dựa trên dữ liệu đã lọc theo ngày)
            var allReviewsInDateRange = await query.ToListAsync();

            ViewBag.CountAll = allReviewsInDateRange.Count;
            ViewBag.Count5 = allReviewsInDateRange.Count(d => d.Sao == 5);
            ViewBag.Count4 = allReviewsInDateRange.Count(d => d.Sao == 4);
            ViewBag.Count3 = allReviewsInDateRange.Count(d => d.Sao == 3);
            ViewBag.Count2 = allReviewsInDateRange.Count(d => d.Sao == 2);
            ViewBag.Count1 = allReviewsInDateRange.Count(d => d.Sao == 1);

            // 4. Bây giờ mới lọc theo sao (nếu user chọn)
            if (sao.HasValue)
            {
                query = query.Where(d => d.Sao == sao.Value);
            }

            // 5. Sắp xếp và lấy dữ liệu hiển thị
            var listHienThi = await query.OrderByDescending(d => d.NgayDanhGia).ToListAsync();

            // 6. Lưu lại các giá trị filter để hiển thị trên View
            ViewBag.CurrentSao = sao;
            ViewBag.FromDate = from?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = to?.ToString("yyyy-MM-dd");

            return View(listHienThi);
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int id)
        {
            var dg = await _db.Danhgias.FindAsync(id);
            if (dg != null)
            {
                _db.Danhgias.Remove(dg);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa đánh giá thành công.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> PhanHoi(int id)
        {
            var dg = await _db.Danhgias
                .Include(d => d.Sanpham)
                .Include(d => d.Khachhang)
                .FirstOrDefaultAsync(d => d.MaDanhGia == id);

            if (dg == null) return NotFound();
            return View(dg);
        }

        [HttpPost]
        public async Task<IActionResult> LuuPhanHoi(int id, string noiDungPhanHoi)
        {
            var dg = await _db.Danhgias.FindAsync(id);
            if (dg != null)
            {
                dg.PhanHoi = noiDungPhanHoi;
                dg.NgayPhanHoi = DateTime.Now;
                _db.Danhgias.Update(dg);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã gửi phản hồi thành công!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
