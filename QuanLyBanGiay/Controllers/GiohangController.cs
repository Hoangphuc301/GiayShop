using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuanLyBanGiay.Models;
using QuanLyBanGiay.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuanLyBanGiay.Services;
using Microsoft.AspNetCore.Http;

namespace QuanLyBanGiay.Controllers
{
    public class GiohangController : Controller
    {
        private readonly QL_GiayContext db = new QL_GiayContext();
        private const string CART_KEY = "CartSession";

        // Sử dụng Service VNPAY
        private IVnpayService VnpayService => HttpContext.RequestServices.GetRequiredService<IVnpayService>();

        private readonly ILogger<GiohangController> _logger;

        // Nếu controller chưa có constructor, thêm constructor để inject logger (vẫn giữ db như hiện tại).
        public GiohangController(ILogger<GiohangController> logger)
        {
            _logger = logger;
        }
        // Lấy giỏ hàng từ session
        private List<CartItem> GetCart()
        {
            var sessionData = HttpContext.Session.GetString(CART_KEY);
            if (string.IsNullOrEmpty(sessionData))
                return new List<CartItem>();
            return JsonConvert.DeserializeObject<List<CartItem>>(sessionData) ?? new List<CartItem>();
        }

        // Lưu giỏ hàng vào session
        private void SaveCart(List<CartItem> cart)
        {
            var json = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CART_KEY, json);
        }

        // Xóa giỏ hàng khỏi session
        private void ClearCart()
        {
            HttpContext.Session.Remove(CART_KEY);
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            var email = HttpContext.Session.GetString("UserEmail");
            Khachhang khach = null;
            if (!string.IsNullOrEmpty(email))
            {
                var taikhoan = db.Khachhangs.FirstOrDefault(t => t.Email == email);
                khach = taikhoan != null ? db.Khachhangs.FirstOrDefault(k => k.Makh == taikhoan.Makh) : null;
            }

            var model = new Checkout
            {
                CartItems = cart,
                Makh = khach?.Makh ?? 0,
                Tenkh = khach?.Tenkh ?? "",
                Email = khach?.Email ?? "",
                Sdt = khach?.Sdt ?? "",
                Diachi = khach?.Diachi ?? ""
            };

            return View(model);
        }

        public async Task<IActionResult> Checkout()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                TempData["Message"] = "Bạn cần đăng nhập trước khi xem giỏ hàng và đặt hàng!";
                return RedirectToAction("Login", "Account");
            }

            var khach = await db.Khachhangs.FirstOrDefaultAsync(k => k.Email == email);
            if (khach == null)
            {
                TempData["Message"] = "Không tìm thấy tài khoản của bạn. Vui lòng đăng nhập lại!";
                return RedirectToAction("Login", "Khachhang");
            }

            var cart = GetCart();

            var model = new Checkout
            {
                CartItems = cart,
                Makh = khach.Makh,
                Tenkh = khach.Tenkh,
                Email = khach.Email,
                Sdt = khach.Sdt,
                Diachi = khach.Diachi
            };

            // LẤY MADH CŨ (khi thanh toán lại)
            var madhCu = HttpContext.Session.GetInt32("ThanhToanDonCu_Madh");
            if (madhCu.HasValue)
            {
                model.Madh = madhCu.Value;
            }

            // VOUCHER
            var vouchers = await db.Vouchers
                .Where(v => v.Trangthai == "CÒN" && v.Ngaybd <= DateTime.Now && v.Ngaykt >= DateTime.Now)
                .Select(v => new SelectListItem
                {
                    Value = v.Mavoucher.ToString(),
                    Text = $"{v.Tenvoucher} - Giảm {(v.Giatri ?? 0)}%"
                })
                .ToListAsync();

            vouchers.Insert(0, new SelectListItem { Value = "0", Text = "-- Không dùng voucher --" });
            ViewBag.Vouchers = vouchers;

            // PHƯƠNG THỨC THANH TOÁN
            var ptttList = await db.Phuongthucthanhtoans
                .Select(p => new SelectListItem
                {
                    Value = p.Mapttt.ToString(),
                    Text = p.Tenphuongthuc
                })
                .ToListAsync();

            ptttList.Insert(0, new SelectListItem { Value = "0", Text = "-- Chọn phương thức thanh toán --" });
            model.PaymentMethods = ptttList;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Checkout checkoutModel)
        {
            // VALIDATION ▼
            var emailSession = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(emailSession))
                return RedirectToAction("Login", "Khachhang");

            var khach = await db.Khachhangs.FirstOrDefaultAsync(k => k.Email == emailSession);
            if (khach == null)
                return RedirectToAction("Login", "Khachhang");

            checkoutModel.CartItems = GetCart();

            if (!checkoutModel.CartItems.Any())
                ModelState.AddModelError("", "Giỏ hàng trống.");

            if (checkoutModel.Mapttt == 0)
                ModelState.AddModelError("Mapttt", "Hãy chọn phương thức thanh toán.");

            if (!ModelState.IsValid)
                return View(checkoutModel);

            // TÍNH TIỀN ▼
            decimal tongTien = checkoutModel.CartItems.Sum(i => i.Sl * i.Dongia);
            decimal tongTienGiam = 0m;

            if (checkoutModel.Mavoucher != 0)
            {
                var voucher = await db.Vouchers.FirstOrDefaultAsync(v => v.Mavoucher == checkoutModel.Mavoucher);
                if (voucher != null && voucher.Giatri.HasValue)
                    tongTienGiam = tongTien * (voucher.Giatri.Value / 100m);
            }

            decimal tongTienCuoi = tongTien - tongTienGiam;


            /
            var madhCu = HttpContext.Session.GetInt32("ThanhToanDonCu_Madh");
            bool isRePay = madhCu.HasValue;

            Donhang donHang;

            if (isRePay)
            {
                donHang = await db.Donhangs
                    .Include(d => d.ChitietDonhangs)
                    .FirstOrDefaultAsync(d => d.Madh == madhCu.Value);

                if (donHang == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy đơn hàng cũ để thanh toán.");
                    return View(checkoutModel);
                }

                // Cập nhật thông tin mới
                donHang.Ngaydat = DateTime.Now;
                donHang.Sdtgiao = checkoutModel.Sdt;
                donHang.Diachigiao = checkoutModel.Diachi;
                donHang.Tongtien = tongTien;
                donHang.Tongtiencuoi = tongTienCuoi;
                donHang.Mapttt = checkoutModel.Mapttt;

                // Xóa chi tiết cũ
                db.ChitietDonhangs.RemoveRange(donHang.ChitietDonhangs);
                await db.SaveChangesAsync();
            }
            else
            {
                // ----------------------------------
                // 🔥 2) CHECKOUT BÌNH THƯỜNG → TẠO ĐƠN MỚI
                // ----------------------------------

                donHang = new Donhang
                {
                    Makh = khach.Makh,
                    Ngaydat = DateTime.Now,
                    Sdtgiao = checkoutModel.Sdt,
                    Diachigiao = checkoutModel.Diachi,
                    Phiship = 0,
                    Tongtien = tongTien,
                    Tongtiencuoi = tongTienCuoi,
                    Mapttt = checkoutModel.Mapttt
                };

                db.Donhangs.Add(donHang);
                await db.SaveChangesAsync();
            }

            // Thêm chi tiết đơn hàng (cả re-pay + mới)
            foreach (var item in checkoutModel.CartItems)
            {
                db.ChitietDonhangs.Add(new ChitietDonhang
                {
                    Madh = donHang.Madh,
                    Mactsp = item.Mactsp,
                    Sl = item.Sl,
                    Dongia = item.Dongia,
                    Thanhtien = item.Sl * item.Dongia
                });
            }

            await db.SaveChangesAsync();


            
            var ptttVNPAY = await db.Phuongthucthanhtoans
                .FirstOrDefaultAsync(p => p.Tenphuongthuc.ToUpper().Contains("VNPAY"));

            if (checkoutModel.Mapttt == ptttVNPAY.Mapttt)
            {
                // Đặt trạng thái chờ thanh toán
                donHang.Trangthai = "CHỜ THANH TOÁN";
                await db.SaveChangesAsync();

                // Tạo URL VNPAY
                var modelVnpay = new Checkout
                {
                    Madh = donHang.Madh,
                    TotalAmount = donHang.Tongtiencuoi ?? 0
                };

                string paymentUrl = VnpayService.CreatePaymentUrl(modelVnpay, HttpContext);
                return Redirect(paymentUrl);
            }

            donHang.Trangthai = "CHỜ XÁC NHẬN";
            await db.SaveChangesAsync();

            ClearCart();
            HttpContext.Session.Remove("ThanhToanDonCu_Madh");

            return RedirectToAction("OrderConfirmation", new { id = donHang.Madh });
        }


        // VnpayReturn
        public async Task<IActionResult> VnpayReturn()
        {
            var response = VnpayService.ProcessVnpayReturn(Request.Query);

            if (response.Success)
            {
                if (!int.TryParse(response.TransactionId, out int madh))
                {
                    TempData["Message"] = "Lỗi hệ thống: Không xác định được Mã đơn hàng.";
                    return RedirectToAction("OrderSuccess", new { success = false });
                }

                var donHang = await db.Donhangs.FirstOrDefaultAsync(dh => dh.Madh == madh);

                if (donHang != null)
                {
                    donHang.Trangthai = "ĐANG GIAO"; // Cập nhật trạng thái sau khi VNPAY thành công
                    await db.SaveChangesAsync();

                    ClearCart();

                    TempData["Message"] = $"Thanh toán thành công! Mã giao dịch VNPAY: {response.VnpayTransactionId}.";
                    return RedirectToAction("OrderConfirmation", new { id = donHang.Madh });
                }
                else
                {
                    TempData["Message"] = "Thanh toán thành công nhưng không tìm thấy đơn hàng trong hệ thống.";
                    return RedirectToAction("OrderSuccess", new { success = false });
                }
            }
            else
            {
                TempData["Message"] = $"Thanh toán thất bại: {response.Message}. Vui lòng thử lại.";

                if (int.TryParse(response.TransactionId, out int madh))
                {
                    var donHang = await db.Donhangs.FirstOrDefaultAsync(dh => dh.Madh == madh);
                    if (donHang != null)
                    {
                        donHang.Trangthai = "CHỜ THANH TOÁN";
                        await db.SaveChangesAsync();
                    }
                }

				//return RedirectToAction("OrderSuccess", new { success = false });
				return Content(Request.QueryString.Value);

			}
		}

        public IActionResult OrderSuccess(bool success)
        {
            ViewBag.Success = success;
            ViewBag.Message = TempData["Message"];
            return View();
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await db.Donhangs
               .Include(dh => dh.MaptttNavigation)
               .Include(dh => dh.MakhNavigation)
               .Include(dh => dh.ChitietDonhangs)
                   .ThenInclude(ct => ct.MactspNavigation)
                       .ThenInclude(ctsp => ctsp.MaspNavigation)
               .Include(dh => dh.ChitietDonhangs)
                   .ThenInclude(ct => ct.MactspNavigation)
                       .ThenInclude(ctsp => ctsp.MamauNavigation)
               .Include(dh => dh.ChitietDonhangs)
                   .ThenInclude(ct => ct.MactspNavigation)
                       .ThenInclude(ctsp => ctsp.MasizeNavigation)
               .FirstOrDefaultAsync(dh => dh.Madh == id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        [HttpPost]
        public IActionResult AddToCart(int mactsp, int sl = 1)
        {
            var cart = GetCart();

            var ctsp = db.ChitietSanphams
                             .Include(c => c.MamauNavigation)
                             .Include(c => c.MasizeNavigation)
                             .FirstOrDefault(p => p.Mactsp == mactsp);
            if (ctsp == null) return BadRequest("Chi tiết SP không tồn tại");

            var sanpham = db.Sanphams.FirstOrDefault(sp => sp.Masp == ctsp.Masp);
            if (sanpham == null) return BadRequest("Sản phẩm không tồn tại");

            var item = cart.FirstOrDefault(c => c.Mactsp == mactsp);
            if (item != null)
            {
                item.Sl += sl;
            }
            else
            {
                cart.Add(new CartItem
                {
                    Mactsp = mactsp,
                    Sl = sl,
                    Tensp = sanpham.Tensp,
                    Hinhanh = sanpham.Hinhdaidien,
                    Dongia = sanpham.Gia,
                    Mau = ctsp.MamauNavigation?.Tenmau ?? "Chưa chọn",
                    Size = ctsp.MasizeNavigation?.Tensize ?? "Chưa chọn"
                });
            }
            SaveCart(cart);
            return PartialView("MiniCart", cart);
        }

        [HttpPost]
        public IActionResult Remove(int mactsp)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.Mactsp == mactsp);
            if (item != null) cart.Remove(item);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public IActionResult MiniCart()
        {
            var cart = GetCart();
            return PartialView("MiniCart", cart);
        }
    }
}