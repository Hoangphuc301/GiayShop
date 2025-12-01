using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DonhangController : Controller
    {
        private readonly QL_GiayContext _context;

        public DonhangController(QL_GiayContext context)
        {
            _context = context;
        }

        public IActionResult Index(int trang = 1, string loaiLoc = "", DateTime? from = null, DateTime? to = null)
        {
            int kichThuocTrang = 10;
            var donhang = _context.Donhangs
                .Include(d => d.MakhNavigation)
                .Include(d => d.MaptttNavigation)
                .Include(d => d.MaNvgNavigation)
                .Include(d => d.MavoucherNavigation)
                .AsQueryable();

            if (loaiLoc == "week")
            {
                var today = DateTime.Today;
                int delta = today.DayOfWeek - DayOfWeek.Sunday;
                var dauTuan = today.AddDays(-delta);
                var cuoiTuan = dauTuan.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);
                donhang = donhang.Where(d => d.Ngaydat >= dauTuan && d.Ngaydat <= cuoiTuan);
            }
            if (loaiLoc == "month")
            {
                var dauThang = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var cuoiThang = dauThang.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);
                donhang = donhang.Where(d => d.Ngaydat >= dauThang && d.Ngaydat <= cuoiThang);
            }
            if (loaiLoc == "range" && from.HasValue && to.HasValue)
            {
                DateTime tuNgay = from.Value.Date;
                DateTime denNgay = to.Value.Date.AddHours(23).AddMinutes(59);
                donhang = donhang.Where(d => d.Ngaydat >= tuNgay && d.Ngaydat <= denNgay);
            }

            ViewBag.LoaiLoc = loaiLoc;
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            int tongSoMuc = donhang.Count();
            int tongSoTrang = (int)Math.Ceiling((double)tongSoMuc / kichThuocTrang);
            if (tongSoTrang == 0) tongSoTrang = 1;
            if (trang < 1) trang = 1;
            if (trang > tongSoTrang) trang = tongSoTrang;

            var donhangs = donhang
                .OrderByDescending(d => d.Ngaydat)
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToList();

            ViewBag.Donhang = donhangs;
            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;
            return View();
        }

        [HttpGet]
        public IActionResult Sua(int id)
        {
            var dh = _context.Donhangs
                        .Include(d => d.MakhNavigation)
                        .FirstOrDefault(d => d.Madh == id);
            if (dh == null) return RedirectToAction("Index");

            var thuTuTrangThai = new List<string> { "CHỜ XÁC NHẬN", "ĐANG GIAO", "ĐÃ NHẬN", "HỦY" };
            var viTriHienTai = thuTuTrangThai.IndexOf(dh.Trangthai);
            ViewBag.ThuTuTrangThai = thuTuTrangThai;
            ViewBag.ViTriHienTai = viTriHienTai;
            return View(dh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sua(Donhang dh)
        {
            var donhang = _context.Donhangs.Find(dh.Madh);
            var statusOrder = new List<string> { "CHỜ XÁC NHẬN", "ĐANG GIAO", "ĐÃ NHẬN", "HỦY" };

            if (donhang == null)
            {
                ModelState.AddModelError("", "Đơn hàng không tồn tại");
                return View(dh);
            }

            var currentIndex = statusOrder.IndexOf(donhang.Trangthai);
            if (currentIndex == statusOrder.IndexOf("ĐÃ NHẬN") || currentIndex == statusOrder.IndexOf("HỦY"))
            {
                TempData["ErrorMessage"] = $"Đơn hàng đã kết thúc, không thể sửa";
                return RedirectToAction("Index");
            }

            donhang.Trangthai = dh.Trangthai;
            donhang.Lydohuy = dh.Trangthai == "HỦY" ? dh.Lydohuy : null;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công";
            return RedirectToAction("Index");
        }

        public IActionResult XemChiTiet(int id)
        {
            var chitiets = _context.ChitietDonhangs
                .Include(ct => ct.MactspNavigation)
                    .ThenInclude(ctsp => ctsp.MaspNavigation)
                .Include(ct => ct.MactspNavigation.MamauNavigation)
                .Include(ct => ct.MactspNavigation.MasizeNavigation)
                .Where(ct => ct.Madh == id)
                .ToList();

            var dh = _context.Donhangs.Include(d => d.MakhNavigation).FirstOrDefault(d => d.Madh == id);
            ViewBag.DonHang = dh;
            return View(chitiets);
        }

        [HttpGet]
        public IActionResult Huy(int id)
        {
            var dh = _context.Donhangs.Include(d => d.MakhNavigation).FirstOrDefault(d => d.Madh == id);
            if (dh == null || dh.Trangthai == "HỦY" || dh.Trangthai == "ĐÃ NHẬN")
                return RedirectToAction("Index");
            return View(dh);
        }

        [HttpPost, ActionName("Huy")]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanHuy(int Madh, string Lydohuy)
        {
            var dh = _context.Donhangs.Find(Madh);
            if (dh == null) return RedirectToAction("Index");

            dh.Trangthai = "HỦY";
            dh.Lydohuy = Lydohuy ?? "Khách hàng hủy đơn";
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        private List<SelectListItem> GetNvgSelectList()
        {
            var nvghList = _context.Khachhangs
                .Where(x => x.Trangthai == true)
                .ToList() 
                .Where(x => x.Loaitk != null && x.Loaitk.Trim().ToUpper().Contains("NVGH"))
                .Select(x => new SelectListItem
                {
                    Value = x.Makh.ToString(),
                    Text = $"{(string.IsNullOrEmpty(x.Tenkh) ? "Chưa có tên" : x.Tenkh.Trim())}"
                })
                .OrderBy(x => x.Text)
                .ToList();

            return nvghList;
        }

        [HttpGet]
        public IActionResult Phancong(int id)
        {
            var dh = _context.Donhangs
                .Include(d => d.MakhNavigation)
                .Include(d => d.MaNvgNavigation)
                .FirstOrDefault(d => d.Madh == id);

            if (dh == null)
            {
                TempData["ErrorMessage"] = "Đơn hàng không tồn tại!";
                return RedirectToAction("Index");
            }

            var vm = new Phancong
            {
                Madh = dh.Madh,
                TenKhachHang = dh.MakhNavigation?.Tenkh ?? "Không xác định",
                TrangThaiHienTai = dh.Trangthai,
                MaNvg = dh.MaNvg,
                TenNhanVienHienTai = dh.MaNvg != null
                    ? _context.Khachhangs.Where(k => k.Makh == dh.MaNvg).Select(k => k.Tenkh ?? k.Email).FirstOrDefault()
                    : null
            };

            ViewData["NvgList"] = new SelectList(
                GetNvgSelectList(),
                "Value",
                "Text",
                dh.MaNvg.HasValue ? (object)dh.MaNvg.Value : null
            );

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Phancong(int id, Phancong model)
        {
            int madhToUse = id != 0 ? id : model.Madh;
            var dh = _context.Donhangs.Include(d => d.MakhNavigation).FirstOrDefault(d => d.Madh == madhToUse);
            if (dh == null)
            {
                TempData["ErrorMessage"] = "Đơn hàng không tồn tại!";
                return RedirectToAction("Index");
            }
            model.Madh = dh.Madh;

            if (!ModelState.IsValid)
            {
                ViewData["NvgList"] = new SelectList(GetNvgSelectList(), "Value", "Text", model.MaNvg.HasValue ? (object)model.MaNvg.Value : null);
                model.TenKhachHang = dh.MakhNavigation?.Tenkh;
                model.TrangThaiHienTai = dh.Trangthai;
                model.TenNhanVienHienTai = model.MaNvg != null
                    ? _context.Khachhangs.Where(k => k.Makh == model.MaNvg).Select(k => k.Tenkh ?? k.Email).FirstOrDefault()
                    : null;
                return View(model);
            }

            dh.MaNvg = model.MaNvg.Value;
            if (dh.Trangthai == "CHỜ XÁC NHẬN" || dh.Trangthai == "Chờ xác nhận")
            {
                dh.Trangthai = "ĐANG GIAO";
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = $"Phân công đơn hàng thành công";
            return RedirectToAction("Index");
        }
    }
}