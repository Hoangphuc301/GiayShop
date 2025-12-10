using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;
using System.Linq;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VoucherController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();

        public IActionResult Index()
        {
            return View(db.Vouchers.OrderByDescending(v => v.Mavoucher).ToList());
        }

        // --- THÊM VOUCHER ---
        [HttpGet]
        public ActionResult them()
        {
            ViewBag.Danhmucs = db.Danhmucs.ToList();
            ViewBag.Sanphams = db.Sanphams.Select(s => new { s.Masp, s.Tensp }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult them(Voucher voucher, string[] SelectedIds)
        {

            if (voucher.LoaiKhuyenMai == 3)
            {
                voucher.Giatri = 0;
                if (ModelState.ContainsKey("Giatri")) ModelState["Giatri"].Errors.Clear();
            }
            else if (voucher.LoaiKhuyenMai == 1)
            {
                if (voucher.Giatri == null || voucher.Giatri < 0 || voucher.Giatri > 100)
                {
                    ModelState.AddModelError("Giatri", "Giảm giá theo % chỉ được nhập từ 0 đến 100.");
                }
            }
            else if (voucher.LoaiKhuyenMai == 2)
            {
                if (voucher.Giatri == null || voucher.Giatri < 1000)
                {
                    ModelState.AddModelError("Giatri", "Số tiền giảm giá quá nhỏ (tối thiểu 1.000đ).");
                }
            }

            if (string.IsNullOrEmpty(voucher.Tenvoucher))
                ModelState.AddModelError("Tenvoucher", "Bạn chưa nhập Tên voucher!");

            if (string.IsNullOrEmpty(voucher.Magiamgia))
                ModelState.AddModelError("Magiamgia", "Bạn chưa nhập Mã giảm giá!");

            if (voucher.Ngaykt <= voucher.Ngaybd)
                ModelState.AddModelError("Ngaykt", "Ngày kết thúc phải lớn hơn ngày bắt đầu.");

            if (voucher.PhamViApDung == 2 || voucher.PhamViApDung == 3)
            {
                if (SelectedIds != null && SelectedIds.Length > 0)
                    voucher.GiaTriPhamVi = string.Join(",", SelectedIds);
                else
                    ModelState.AddModelError("GiaTriPhamVi", "Bạn chọn phạm vi nhưng chưa chọn danh mục/sản phẩm nào.");
            }
            else
            {
                voucher.GiaTriPhamVi = null;
            }

            if (string.IsNullOrEmpty(voucher.Trangthai)) voucher.Trangthai = "CÒN";
            if (voucher.DaSuDung == 0) voucher.DaSuDung = 0;

            ModelState.Remove("Trangthai");
            ModelState.Remove("DaSuDung");
            ModelState.Remove("GiaTriPhamVi");

            if (ModelState.IsValid)
            {
                try
                {
                    db.Vouchers.Add(voucher);
                    db.SaveChanges();
                    TempData["Success"] = "Lưu thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                    if (msg.Contains("arithmetic overflow") || msg.Contains("out of range"))
                    {
                        TempData["Error"] = "LỖI: Số tiền quá lớn so với giới hạn của Database (decimal 5,2). Hãy chạy lệnh SQL sửa cột GIATRI.";
                    }
                    else
                    {
                        TempData["Error"] = "Lỗi SQL: " + msg;
                    }
                    System.Diagnostics.Debug.WriteLine(msg);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "KHÔNG LƯU ĐƯỢC: " + string.Join(" | ", errors);
            }

            ViewBag.Danhmucs = db.Danhmucs.ToList();
            ViewBag.Sanphams = db.Sanphams.Select(s => new { s.Masp, s.Tensp }).ToList();

            return View(voucher);
        }
        // ================== SỬA VOUCHER ==================

        [HttpGet]
        public ActionResult sua(int id)
        {
            var voucher = db.Vouchers.Find(id);
            if (voucher == null)
            {
                TempData["Error"] = "Không tìm thấy voucher!";
                return RedirectToAction("Index");
            }

            ViewBag.Danhmucs = db.Danhmucs.ToList();
            ViewBag.Sanphams = db.Sanphams.Select(s => new { s.Masp, s.Tensp }).ToList();

            return View(voucher);
        }

        [HttpPost]
        public ActionResult sua(Voucher voucher, string[] SelectedIds)
        {
           
            if (voucher.PhamViApDung == 2 || voucher.PhamViApDung == 3)
            {
                if (SelectedIds != null && SelectedIds.Length > 0)
                    voucher.GiaTriPhamVi = string.Join(",", SelectedIds);
                else
                    ModelState.AddModelError("GiaTriPhamVi", "Bạn chưa chọn danh mục/sản phẩm nào.");
            }
            else
            {
                voucher.GiaTriPhamVi = null;
            }

            if (voucher.Ngaykt <= voucher.Ngaybd)
                ModelState.AddModelError("Ngaykt", "Ngày kết thúc phải lớn hơn ngày bắt đầu.");

            if (ModelState.IsValid)
            {
                try
                {
                    var voucherInDb = db.Vouchers.Find(voucher.Mavoucher);
                    if (voucherInDb == null) return NotFound();

                    voucherInDb.Tenvoucher = voucher.Tenvoucher;
                    voucherInDb.Magiamgia = voucher.Magiamgia;
                    voucherInDb.LoaiKhuyenMai = voucher.LoaiKhuyenMai;
                    voucherInDb.Giatri = voucher.Giatri;
                    voucherInDb.PhamViApDung = voucher.PhamViApDung;
                    voucherInDb.GiaTriPhamVi = voucher.GiaTriPhamVi;
                    voucherInDb.NhomKhachHang = voucher.NhomKhachHang;
                    voucherInDb.GioiHanSuDung = voucher.GioiHanSuDung;
                    voucherInDb.GioiHanMoiKhach = voucher.GioiHanMoiKhach;
                    voucherInDb.Ngaybd = voucher.Ngaybd;
                    voucherInDb.Ngaykt = voucher.Ngaykt;
                    voucherInDb.Trangthai = voucher.Trangthai; 

                    db.SaveChanges();
                    TempData["Success"] = "Cập nhật thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Lỗi SQL: " + ex.Message;
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi nhập liệu: " + string.Join(", ", errors);
            }

            ViewBag.Danhmucs = db.Danhmucs.ToList();
            ViewBag.Sanphams = db.Sanphams.Select(s => new { s.Masp, s.Tensp }).ToList();
            return View(voucher);
        }

        // ================== XÓA VOUCHER ==================
        [HttpGet]
        public ActionResult xoa(int id)
        {
            var vc = db.Vouchers.Find(id);

            if (vc == null)
            {
                TempData["Error"] = "Không tìm thấy voucher cần xóa!";
                return RedirectToAction("Index");
            }

            return View(vc);
        }

        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public ActionResult XacNhanXoa(int id)
        {
            bool daCoDonHang = db.Donhangs.Any(d => d.Mavoucher == id);
            if (daCoDonHang)
            {
                TempData["Error"] = "❌ Không thể xóa vì voucher này đã được sử dụng trong đơn hàng.";
                return RedirectToAction("Index");
            }

            var vc = db.Vouchers.Find(id);
            if (vc == null)
            {
                TempData["Error"] = "Voucher không tồn tại hoặc đã bị xóa.";
                return RedirectToAction("Index");
            }

            db.Vouchers.Remove(vc);
            db.SaveChanges();
            TempData["Success"] = "Xóa voucher thành công!";

            return RedirectToAction("Index");
        }
    }
}