using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;
using System.Linq;
using System.Collections.Generic;

namespace QuanLyBanGiay.Controllers
{
    public class TimkiemController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        [HttpGet]
        public IActionResult Index(string q,int[] madm, int[] math, int[] mamau, int[] masize, int? gia)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                ViewBag.Message = "Vui lòng nhập từ khóa tìm kiếm.";
                LoadFilterData(); // load dữ liệu filter để View không bị lỗi
                return View(new List<Sanpham>());
            }

            var sp = db.Sanphams
                .Include(sp => sp.MadmNavigation)
                .Include(sp => sp.MathNavigation)
                .Include(sp => sp.ChitietSanphams)
                    .ThenInclude(ct => ct.MamauNavigation)
                .Include(sp => sp.ChitietSanphams)
                    .ThenInclude(ct => ct.MasizeNavigation)
                .Where(sp =>
                    EF.Functions.Like(sp.Tensp, $"%{q}%") ||
                    EF.Functions.Like(sp.MadmNavigation.Tendm, $"%{q}%") ||
                    EF.Functions.Like(sp.MathNavigation.Tenth, $"%{q}%"))
                .AsQueryable();

            // ====== Áp dụng bộ lọc (Filter) ======

            // --- Lọc theo danh mục ---
            if (madm != null && madm.Any())
                sp = sp.Where(x => madm.Contains(x.Madm));

            // --- Lọc theo thương hiệu ---
            if (math != null && math.Any())
                sp = sp.Where(x => math.Contains(x.Math));

            // --- Lọc theo màu sắc ---
            if (mamau != null && mamau.Any())
                sp = sp.Where(x => x.ChitietSanphams.Any(ct => mamau.Contains(ct.Mamau)));

            // --- Lọc theo kích cỡ ---
            if (masize != null && masize.Any())
                sp = sp.Where(x => x.ChitietSanphams.Any(ct => masize.Contains(ct.Masize)));

            // --- Lọc theo giá ---
            if (gia != null)
            {
                switch (gia)
                {
                    case 1:
                        sp = sp.Where(x => x.Gia < 1000000);
                        break;
                    case 2:
                        sp = sp.Where(x => x.Gia >= 1000000 && x.Gia <= 2000000);
                        break;
                    case 3:
                        sp = sp.Where(x => x.Gia > 2000000 && x.Gia <= 3000000);
                        break;
                    case 4:
                        sp = sp.Where(x => x.Gia > 3000000);
                        break;
                }
            }

            // ====== 4. Kết quả cuối ======
            var kq = sp.ToList();

            // ====== 5. Truyền dữ liệu sang View ======
            ViewBag.Keyword = q;
            LoadFilterData();

            return View(kq);
        }
        private void LoadFilterData()
        {
            ViewBag.DanhMuc = db.Danhmucs.ToList();
            ViewBag.ThuongHieu = db.Thuonghieus.ToList();
            ViewBag.Mau = db.Maus.ToList();
            ViewBag.Size = db.Sizes.ToList();
        }
    }
}
