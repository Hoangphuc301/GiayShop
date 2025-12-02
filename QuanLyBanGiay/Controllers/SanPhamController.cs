using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyBanGiay.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly QL_GiayContext db = new QL_GiayContext();

        public IActionResult Index() => View();

        public async Task<IActionResult> Chitiet(int id)
        {
            var sp = await db.Sanphams
                .Include(x => x.MadmNavigation)
                .Include(x => x.MathNavigation)
                .Include(x => x.ChitietSanphams)
                    .ThenInclude(ct => ct.MamauNavigation)
                .Include(x => x.ChitietSanphams)
                    .ThenInclude(ct => ct.MasizeNavigation)
                .FirstOrDefaultAsync(x => x.Masp == id);

            if (sp == null) return RedirectToAction("Index", "Home");

            // Lấy đánh giá
            var danhGias = await db.Danhgias
                .Where(d => d.MaSp == id)
                .Include(d => d.Khachhang)
                .OrderByDescending(d => d.NgayDanhGia)
                .ToListAsync();

            ViewBag.AverageRating = danhGias.Any() ? danhGias.Average(d => d.Sao) : 0;
            ViewBag.DanhGias = danhGias;

            // Kiểm tra khách đã mua
            var email = HttpContext.Session.GetString("UserEmail");
            bool coTheDanhGia = false;
            if (!string.IsNullOrEmpty(email))
            {
                var kh = await db.Khachhangs.FirstOrDefaultAsync(k => k.Email == email);
                if (kh != null)
                {
                    coTheDanhGia = await db.Donhangs
                        .Where(dh => dh.Makh == kh.Makh &&
                                     (dh.Trangthai == "ĐÃ NHẬN" || dh.Trangthai == "HOÀN THÀNH"))
                        .SelectMany(dh => dh.ChitietDonhangs)
                        .AnyAsync(ct => ct.MactspNavigation.Masp == id);
                }
            }
            ViewBag.CoTheDanhGia = coTheDanhGia;

            return View(sp);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int MaSp, int Sao, string NoiDung)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "Khachhang");

            var kh = await db.Khachhangs.FirstOrDefaultAsync(k => k.Email == email);
            if (kh == null) return RedirectToAction("Login", "Khachhang");

            // Kiểm tra đã mua sản phẩm
            bool daMua = await db.Donhangs
                .Where(dh => dh.Makh == kh.Makh &&
                             (dh.Trangthai == "ĐÃ NHẬN" || dh.Trangthai == "HOÀN THÀNH"))
                .SelectMany(dh => dh.ChitietDonhangs)
                .AnyAsync(ct => ct.MactspNavigation.Masp == MaSp);

            if (!daMua)
            {
                TempData["ErrorReview"] = "Bạn cần mua sản phẩm trước khi đánh giá";
                return RedirectToAction("Chitiet", new { id = MaSp });
            }

            // Kiểm tra đã đánh giá chưa
            bool daDanhGia = await db.Danhgias.AnyAsync(d => d.MaKh == kh.Makh && d.MaSp == MaSp);
            if (daDanhGia)
            {
                TempData["ErrorReview"] = "Bạn chỉ được đánh giá 1 lần cho sản phẩm này";
                return RedirectToAction("Chitiet", new { id = MaSp });
            }

            var dg = new Danhgia
            {
                MaKh = kh.Makh,
                MaSp = MaSp,
                Sao = Sao,
                NoiDung = NoiDung,
                CoTheDanhGia = true
            };

            db.Danhgias.Add(dg);
            await db.SaveChangesAsync();

            return RedirectToAction("Chitiet", new { id = MaSp });
        }
    }
}
