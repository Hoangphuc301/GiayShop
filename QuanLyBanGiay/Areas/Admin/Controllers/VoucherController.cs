using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VoucherController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();

        // --- Hiển thị danh sách voucher ---
        public IActionResult Index()
        {
            ViewBag.Vouchers = db.Vouchers.ToList();
            return View();
        }

        // --- Thêm ---
        [HttpGet]
        public ActionResult them()
        {
            return View();
        }

        [HttpPost]
        public ActionResult them(Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                db.Vouchers.Add(voucher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(voucher);
        }

        // --- Sửa ---
        [HttpGet]
        public ActionResult sua(int id)
        {
            var vc = db.Vouchers.Find(id);
            if (vc == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Voucher = vc; // ✅ gửi voucher qua ViewBag
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sua(Voucher vc)
        {
            if (ModelState.IsValid)
            {
                var voucher = db.Vouchers.Find(vc.Mavoucher);
                if (voucher == null)
                {
                    ModelState.AddModelError("", "Voucher không tồn tại hoặc đã bị xóa.");
                    return View();
                }

                voucher.Tenvoucher = vc.Tenvoucher;
                voucher.Magiamgia = vc.Magiamgia;
                voucher.Giatri = vc.Giatri;
                voucher.Ngaybd = vc.Ngaybd;
                voucher.Ngaykt = vc.Ngaykt;
                voucher.Trangthai = vc.Trangthai;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vc);
        }


        // --- Xóa ---
        [HttpGet]
        public IActionResult xoa(int id)
        {
            var vc = db.Vouchers.Find(id);
            if (vc == null)
            {
                return RedirectToAction("Index");
            }
            return View(vc);
        }

        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult xoa_Post(int Mavoucher)
        {
            var vc = db.Vouchers.Find(Mavoucher);
            if (vc == null)
            {
                return RedirectToAction("Index");
            }

            db.Vouchers.Remove(vc);
            db.SaveChanges();

            TempData["Success"] = "Xóa voucher thành công!";
            return RedirectToAction("Index");
        }
    }
}
