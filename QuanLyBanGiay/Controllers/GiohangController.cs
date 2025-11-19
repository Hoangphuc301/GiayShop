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

namespace QuanLyBanGiay.Controllers
{
    public class GiohangController : Controller
    {
        private readonly QL_GiayContext db = new QL_GiayContext();
        private const string CART_KEY = "CartSession";

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
            // Kiểm tra đăng nhập
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                TempData["Message"] = "Bạn cần đăng nhập trước khi xem giỏ hàng và đặt hàng!";
                return RedirectToAction("Login", "Account");
            }

            var taikhoan = db.Khachhangs.FirstOrDefault(t => t.Email == email);
            var khach = taikhoan != null ? db.Khachhangs.FirstOrDefault(k => k.Makh == taikhoan.Makh) : null;

            var cart = GetCart();
            var model = new Checkout
            {
                CartItems = cart,
                Makh = khach?.Makh ?? 0,
                Tenkh = khach?.Tenkh ?? "",
                Email = taikhoan?.Email ?? "",
                Sdt = khach?.Sdt ?? "",
                Diachi = khach?.Diachi ?? ""
            };

            return View(model);
        }

        // GET: Checkout
        public async Task<IActionResult> Checkout()
        {
            // Kiểm tra đăng nhập
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                TempData["Message"] = "Bạn cần đăng nhập trước khi thanh toán!";
                return RedirectToAction("Login", "Khachhang");
            }

            var taikhoan = await db.Khachhangs.FirstOrDefaultAsync(t => t.Email == email);
            if (taikhoan == null)
            {
                TempData["Message"] = "Không tìm thấy tài khoản của bạn. Vui lòng đăng nhập lại!";
                return RedirectToAction("Login", "Khachhang");
            }

            var cart = GetCart();
            var khach = await db.Khachhangs.FirstOrDefaultAsync(k => k.Makh == taikhoan.Makh);

            var model = new Checkout
            {
                CartItems = cart,
                Makh = khach.Makh,
                Tenkh = khach.Tenkh,
                Email = khach.Email,
                Sdt = khach.Sdt,
                Diachi = khach.Diachi
            };

            // Load phương thức thanh toán
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

        // POST: Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Checkout checkoutModel)
        {
            // Kiểm tra đăng nhập
            var emailSession = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(emailSession))
            {
                TempData["Message"] = "Bạn cần đăng nhập trước khi thanh toán!";
                return RedirectToAction("Login", "Khachhang");
            }

            var khach = await db.Khachhangs.FirstOrDefaultAsync(k => k.Email == emailSession);
            if (khach == null)
            {
                TempData["Message"] = "Không tìm thấy tài khoản của bạn. Vui lòng đăng nhập lại!";
                return RedirectToAction("Login", "Khachhang");
            }

            // Lấy giỏ hàng
            checkoutModel.CartItems = GetCart();

            // Load phương thức thanh toán
            var ptttList = await db.Phuongthucthanhtoans
                                   .Select(p => new SelectListItem
                                   {
                                       Value = p.Mapttt.ToString(),
                                       Text = p.Tenphuongthuc
                                   })
                                   .ToListAsync();
            ptttList.Insert(0, new SelectListItem { Value = "0", Text = "-- Chọn phương thức thanh toán --" });
            checkoutModel.PaymentMethods = ptttList;

            // VALIDATION
            if (!checkoutModel.CartItems.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống.");
                return RedirectToAction("Index");
            }
            if (checkoutModel.Mapttt == 0)
                ModelState.AddModelError("Mapttt", "Vui lòng chọn phương thức thanh toán.");
            if (string.IsNullOrWhiteSpace(checkoutModel.Tenkh))
                ModelState.AddModelError("Tenkh", "Vui lòng nhập họ tên.");
            if (string.IsNullOrWhiteSpace(checkoutModel.Sdt))
                ModelState.AddModelError("Sdt", "Vui lòng nhập số điện thoại.");
            if (string.IsNullOrWhiteSpace(checkoutModel.Diachi))
                ModelState.AddModelError("Diachi", "Vui lòng nhập địa chỉ giao hàng.");

            if (!ModelState.IsValid)
                return View(checkoutModel);

            try
            {
                // Tính tổng tiền
                decimal tongTien = checkoutModel.CartItems.Sum(i => i.Sl * i.Dongia);

                // Tạo đơn hàng
                var donHang = new Donhang
                {
                    Makh = khach.Makh,
                    Mapttt = checkoutModel.Mapttt,
                    Ngaydat = DateTime.Now,
                    Tongtien = tongTien,
                    Tongtiencuoi = tongTien,
                    Trangthai = "Chờ xác nhận",
                    Diachigiao = checkoutModel.Diachi,
                    Sdtgiao = checkoutModel.Sdt,
                    Phiship = 0m,
                    Mavoucher = null,
                    Lydohuy = checkoutModel.Email
                };

                db.Donhangs.Add(donHang);
                await db.SaveChangesAsync(); // tạo Madh

                // Lưu chi tiết đơn hàng
                foreach (var item in checkoutModel.CartItems)
                {
                    var chiTiet = new ChitietDonhang
                    {
                        Madh = donHang.Madh,
                        Mactsp = item.Mactsp,
                        Sl = item.Sl,
                        Dongia = item.Dongia,
                        Thanhtien = item.Sl * item.Dongia
                    };
                    db.ChitietDonhangs.Add(chiTiet);
                }

                await db.SaveChangesAsync();

                // Xóa giỏ hàng
                ClearCart();

                return RedirectToAction("OrderConfirmation", new { id = donHang.Madh });
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError("", "Lỗi CSDL: " + (dbEx.InnerException?.Message ?? dbEx.Message));
                return View(checkoutModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + (ex.InnerException?.Message ?? ex.Message));
                return View(checkoutModel);
            }
        }

        // Xác nhận đơn hàng
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

        // Thêm vào giỏ hàng
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

        // Xóa khỏi giỏ hàng
        [HttpPost]
        public IActionResult Remove(int mactsp)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.Mactsp == mactsp);
            if (item != null) cart.Remove(item);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // Giỏ hàng nhỏ
        public IActionResult MiniCart()
        {
            var cart = GetCart();
            return PartialView("MiniCart", cart);
        }
    }
}
