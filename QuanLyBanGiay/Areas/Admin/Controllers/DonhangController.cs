using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DonhangController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();

        public IActionResult Index()
        {
            var donhangs = db.Donhangs
                .Include(d => d.MakhNavigation)
                .Include(d => d.MaptttNavigation)
                .Include(d => d.MavoucherNavigation)
                .OrderByDescending(d => d.Ngaydat)
                .ToList();
            return View(donhangs);
        }

        [HttpGet]
        public IActionResult Sua(int id)
        {
            var dh = db.Donhangs
                .Include(d => d.MakhNavigation)
                .FirstOrDefault(d => d.Madh == id);

            if (dh == null)
                return RedirectToAction("Index");

            return View(dh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sua(Donhang dh)
        {
            var donhang = db.Donhangs.Find(dh.Madh);
            if (donhang == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại!";
                return RedirectToAction("Index");
            }

            donhang.Trangthai = dh.Trangthai;
            donhang.Lydohuy = dh.Trangthai == "HỦY" ? dh.Lydohuy : null;

            db.SaveChanges();

            TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction("Index");
        }

        public IActionResult XemChiTiet(int id)
        {
            var chitiets = db.ChitietDonhangs
                .Include(ct => ct.MactspNavigation)
                    .ThenInclude(ctsp => ctsp.MaspNavigation)
                .Include(ct => ct.MactspNavigation.MamauNavigation)
                .Include(ct => ct.MactspNavigation.MasizeNavigation)
                .Where(ct => ct.Madh == id)
                .ToList();

            var dh = db.Donhangs
                .Include(d => d.MakhNavigation)
                .FirstOrDefault(d => d.Madh == id);

            ViewBag.DonHang = dh;
            return View(chitiets);
        }

        [HttpGet]
        public IActionResult Xoa(int id)
        {
            var dh = db.Donhangs.Find(id);
            if (dh == null)
                return RedirectToAction("Index");

            return View(dh);
        }

        [HttpPost, ActionName("Xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanXoa(int id)
        {
            var dh = db.Donhangs.Find(id);
            if (dh != null)
            {
                db.Donhangs.Remove(dh);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa đơn hàng.";
            }
            return RedirectToAction("Index");
        }
    }
}
