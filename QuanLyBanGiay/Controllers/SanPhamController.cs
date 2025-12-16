using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyBanGiay.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly QL_GiayContext db = new QL_GiayContext();

        public IActionResult Index() => View();

        // ===== ACTION CHI TIẾT SẢN PHẨM (FULL CHỨC NĂNG) =====
        public async Task<IActionResult> Chitiet(int id, int? rating, int page = 1)
        {
            int pageSize = 5; // Số đánh giá mỗi trang

            // 1. LẤY THÔNG TIN SẢN PHẨM
            var sp = await db.Sanphams
                .Include(x => x.MadmNavigation)
                .Include(x => x.MathNavigation)
                .Include(x => x.ChitietSanphams).ThenInclude(ct => ct.MamauNavigation)
                .Include(x => x.ChitietSanphams).ThenInclude(ct => ct.MasizeNavigation)
                .FirstOrDefaultAsync(x => x.Masp == id);

            if (sp == null) return RedirectToAction("Index", "Home");

            // 2. LẤY TOÀN BỘ ĐÁNH GIÁ ĐỂ TÍNH THỐNG KÊ (Trước khi lọc)
            var allDanhGias = await db.Danhgias
                .Where(d => d.MaSp == id)
                .Include(d => d.Khachhang)
                .ToListAsync();

            // A. Tính điểm trung bình
            ViewBag.AverageRating = allDanhGias.Any() ? Math.Round(allDanhGias.Average(d => d.Sao), 1) : 0;

            // B. Đếm số lượng cho các nút bộ lọc (Shopee Style)
            ViewBag.CountAll = allDanhGias.Count;
            ViewBag.Count5 = allDanhGias.Count(d => d.Sao == 5);
            ViewBag.Count4 = allDanhGias.Count(d => d.Sao == 4);
            ViewBag.Count3 = allDanhGias.Count(d => d.Sao == 3);
            ViewBag.Count2 = allDanhGias.Count(d => d.Sao == 2);
            ViewBag.Count1 = allDanhGias.Count(d => d.Sao == 1);

            // C. Tính % cho thanh Progress Bar (Giữ lại để không lỗi giao diện cũ nếu có dùng)
            int total = allDanhGias.Count;
            ViewBag.RatingStats = Enumerable.Range(1, 5)
                .Select(sao => new
                {
                    Sao = sao,
                    Count = allDanhGias.Count(d => d.Sao == sao),
                    Percent = total == 0 ? 0 : (allDanhGias.Count(d => d.Sao == sao) * 100 / total)
                })
                .OrderByDescending(x => x.Sao)
                .ToList();

            // 3. XỬ LÝ LỌC VÀ PHÂN TRANG
            var query = db.Danhgias.Where(d => d.MaSp == id);

            // Nếu có chọn sao thì lọc
            if (rating.HasValue)
            {
                query = query.Where(d => d.Sao == rating.Value);
            }

            int totalReviews = await query.CountAsync(); // Tổng số review sau khi lọc

            var danhGias = await query
                .Include(d => d.Khachhang)
                .OrderByDescending(d => d.NgayDanhGia)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Truyền dữ liệu sang View
            ViewBag.DanhGias = danhGias;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalReviews / pageSize);
            ViewBag.ProductId = id;
            ViewBag.CurrentFilter = rating; // Giữ trạng thái nút lọc

            // 4. KIỂM TRA QUYỀN VÀ LẤY ID KHÁCH HIỆN TẠI
            var email = HttpContext.Session.GetString("UserEmail");
            bool daDangNhap = !string.IsNullOrEmpty(email);
            bool daMua = false;
            int? currentMakh = null;

            if (daDangNhap)
            {
                var kh = await db.Khachhangs.FirstOrDefaultAsync(k => k.Email == email);
                if (kh != null)
                {
                    currentMakh = kh.Makh; // Lấy mã khách để hiện nút Sửa/Xóa

                    // Kiểm tra đã mua và nhận hàng chưa
                    daMua = await db.Donhangs
                        .Where(dh => dh.Makh == kh.Makh && (dh.Trangthai == "ĐÃ NHẬN" || dh.Trangthai == "HOÀN THÀNH"))
                        .Join(db.ChitietDonhangs, dh => dh.Madh, ct => ct.Madh, (dh, ct) => ct)
                        .Join(db.ChitietSanphams, ct => ct.Mactsp, sp2 => sp2.Mactsp, (ct, sp2) => sp2)
                        .AnyAsync(sp2 => sp2.Masp == id);
                }
            }

            ViewBag.DaDangNhap = daDangNhap;
            ViewBag.CoTheDanhGia = daDangNhap && daMua;
            ViewBag.CurrentUserMakh = currentMakh; // Quan trọng cho chức năng Sửa/Xóa

            return View(sp);
        }

        // ===== ACTION THÊM ĐÁNH GIÁ =====
        [HttpPost]
        public async Task<IActionResult> AddReview(int MaSp, int Sao, string NoiDung)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "Khachhang");

            var kh = await db.Khachhangs.FirstOrDefaultAsync(k => k.Email == email);
            if (kh == null) return RedirectToAction("Login", "Khachhang");

            // Check quyền mua hàng
            bool daMua = await db.Donhangs
                .Where(dh => dh.Makh == kh.Makh && (dh.Trangthai == "ĐÃ NHẬN" || dh.Trangthai == "HOÀN THÀNH"))
                .Join(db.ChitietDonhangs, dh => dh.Madh, ct => ct.Madh, (dh, ct) => ct)
                .Join(db.ChitietSanphams, ct => ct.Mactsp, sp => sp.Mactsp, (ct, sp) => sp)
                .AnyAsync(sp => sp.Masp == MaSp);

            if (!daMua)
            {
                TempData["ErrorReview"] = "Bạn phải mua và nhận hàng thành công mới được đánh giá.";
                return RedirectToAction("Chitiet", new { id = MaSp });
            }

            // Check đã đánh giá chưa (mỗi người 1 lần/sp)
            bool daDanhGia = await db.Danhgias.AnyAsync(d => d.MaKh == kh.Makh && d.MaSp == MaSp);
            if (daDanhGia)
            {
                TempData["ErrorReview"] = "Bạn đã đánh giá sản phẩm này rồi.";
                return RedirectToAction("Chitiet", new { id = MaSp });
            }

            if (Sao < 1 || Sao > 5) Sao = 5;

            var dg = new Danhgia
            {
                MaKh = kh.Makh,
                MaSp = MaSp,
                Sao = Sao,
                NoiDung = NoiDung,
                NgayDanhGia = DateTime.Now,
                CoTheDanhGia = true
            };

            db.Danhgias.Add(dg);
            await db.SaveChangesAsync();

            TempData["SuccessReview"] = "Gửi đánh giá thành công!";
            return RedirectToAction("Chitiet", new { id = MaSp });
        }

        // ===== ACTION SỬA ĐÁNH GIÁ (MỚI) =====
        [HttpPost]
        public async Task<IActionResult> EditReview(int MaDg, int MaSp, int Sao, string NoiDung)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var kh = await db.Khachhangs.FirstOrDefaultAsync(k => k.Email == email);

            if (kh == null) return RedirectToAction("Login", "Khachhang");

            var review = await db.Danhgias.FirstOrDefaultAsync(d => d.MaDanhGia == MaDg);

            // Kiểm tra tồn tại và quyền sở hữu (chính chủ mới được sửa)
            if (review != null && review.MaKh == kh.Makh)
            {
                if (Sao < 1 || Sao > 5) Sao = 5;

                review.Sao = Sao;
                review.NoiDung = NoiDung;
                // review.NgayDanhGia = DateTime.Now; // Uncomment nếu muốn cập nhật ngày sửa thành ngày mới nhất

                await db.SaveChangesAsync();
                TempData["SuccessReview"] = "Cập nhật đánh giá thành công!";
            }
            else
            {
                TempData["ErrorReview"] = "Bạn không có quyền sửa đánh giá này.";
            }

            return RedirectToAction("Chitiet", new { id = MaSp });
        }

        // ===== ACTION XÓA ĐÁNH GIÁ (MỚI) =====
        [HttpPost]
        public async Task<IActionResult> DeleteReview(int MaDg, int MaSp)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var kh = await db.Khachhangs.FirstOrDefaultAsync(k => k.Email == email);

            if (kh == null) return RedirectToAction("Login", "Khachhang");

            var review = await db.Danhgias.FirstOrDefaultAsync(d => d.MaDanhGia == MaDg);

            // Kiểm tra tồn tại và quyền sở hữu
            if (review != null && review.MaKh == kh.Makh)
            {
                db.Danhgias.Remove(review);
                await db.SaveChangesAsync();
                TempData["SuccessReview"] = "Đã xóa đánh giá.";
            }
            else
            {
                TempData["ErrorReview"] = "Bạn không có quyền xóa đánh giá này.";
            }

            return RedirectToAction("Chitiet", new { id = MaSp });
        }
    }
}