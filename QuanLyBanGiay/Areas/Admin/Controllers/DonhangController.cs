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

        //Sửa
        [HttpGet]
        public IActionResult Sua(int id)
        {
            var dh = db.Donhangs
                      .Include(d => d.MakhNavigation)
                      .FirstOrDefault(d => d.Madh == id);

            if (dh == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Donhang = dh;
            return View(dh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sua(Donhang dh)
        {
            var donhang = db.Donhangs.Find(dh.Madh);
            if (donhang == null)
            {
                ModelState.AddModelError("", "Đơn hàng này không tồn tại hoặc đã bị xóa.");
                return View(dh);
            }

            donhang.Trangthai = dh.Trangthai;
            donhang.Lydohuy = dh.Trangthai == "HỦY" ? dh.Lydohuy : null;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Xem chi tiết
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

        //Hủy
        [HttpGet]
        public IActionResult Huy(int id)
        {
            var dh = db.Donhangs
                           .Include(d => d.MakhNavigation)
                           .FirstOrDefault(d => d.Madh == id);

            if (dh == null)
                return RedirectToAction("Index");

            if (dh.Trangthai == "HỦY" || dh.Trangthai == "ĐÃ NHẬN")
            {
                return RedirectToAction("Index");
            }
            return View(dh);
        }

        [HttpPost, ActionName("Huy")]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanHuy(int Madh, string Lydohuy)
        {
            var dh = db.Donhangs.Find(Madh);

            if (dh == null)
            {
                return RedirectToAction("Index");
            }
            dh.Trangthai = "HỦY";
            dh.Lydohuy = Lydohuy ?? "Khách hàng yêu cầu hủy đơn.";

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
