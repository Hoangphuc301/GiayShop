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
            var cart = GetCart();

            var email = HttpContext.Session.GetString("UserEmail");
            var taikhoan = !string.IsNullOrEmpty(email) ? db.Taikhoans.FirstOrDefault(t => t.Email == email) : null;
            var khach = taikhoan != null ? db.Khachhangs.FirstOrDefault(k => k.Matk == taikhoan.Matk) : null;

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
    
        //Thanh toán
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index");

            var email = HttpContext.Session.GetString("UserEmail");
            var taikhoan = !string.IsNullOrEmpty(email) ? db.Taikhoans.FirstOrDefault(t => t.Email == email) : null;
            var khach = taikhoan != null ? db.Khachhangs.FirstOrDefault(k => k.Matk == taikhoan.Matk) : null;

            var model = new Checkout
            {
                CartItems = cart,
                Makh = khach?.Makh ?? 0,
                Tenkh = khach?.Tenkh ?? "",
                Email = taikhoan?.Email ?? "",
                Sdt = khach?.Sdt ?? "",
                Diachi = khach?.Diachi ?? ""
            };
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
            checkoutModel.CartItems = GetCart();
            var ptttList = await db.Phuongthucthanhtoans
                                   .Select(p => new SelectListItem
                                   {
                                       Value = p.Mapttt.ToString(),
                                       Text = p.Tenphuongthuc
                                   })
                                   .ToListAsync();
            ptttList.Insert(0, new SelectListItem { Value = "0", Text = "-- Chọn phương thức thanh toán --" });
            checkoutModel.PaymentMethods = ptttList;

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
                decimal tongTien = checkoutModel.CartItems.Sum(i => i.Sl * i.Dongia);
                //Xử lý khách vãng lai nếu Makh = 0, cố gắng dùng Makh = 1
                //Nếu không tồn tại, thử tự tạo 1 khách vãng lai
                int makhToSave = checkoutModel.Makh;
                if (makhToSave == 0)
                {
                    var guest = db.Khachhangs.FirstOrDefault(k => k.Makh == 1);
                    if (guest != null)
                    {
                        makhToSave = guest.Makh;
                    }
                    else
                    {
                        try
                        {
                            var newGuest = new Khachhang
                            {
                                Tenkh = "Khách vãng lai",
                                Sdt = "0000000000",
                                Diachi = "Chưa cập nhật",
                            };
                            db.Khachhangs.Add(newGuest);
                            await db.SaveChangesAsync();
                            makhToSave = newGuest.Makh;
                        }
                        catch (Exception createEx)
                        {
                            ModelState.AddModelError("", "Không tìm thấy khách vãng lai (Makh = 1). Hệ thống đã cố tạo nhưng thất bại. " 
                                + "Vui lòng tạo 1 khách vãng lai trong bảng Khachhang (Makh = 1) hoặc đăng nhập trước khi đặt hàng. Chi tiết: " 
                                + (createEx.InnerException?.Message ?? createEx.Message));
                            return View(checkoutModel);
                        }
                    }
                }

                var donHang = new Donhang
                {
                    Makh = makhToSave,
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
                await db.SaveChangesAsync(); // lưu để sinh Madh

                // Lưu chi tiết
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

        //Xoá khỏi giỏ hàng
        [HttpPost]
        public IActionResult Remove(int mactsp)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.Mactsp == mactsp);
            if (item != null) cart.Remove(item);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        //Giỏ hàng nhỏ
        public IActionResult MiniCart()
        {
            var cart = GetCart();
            return PartialView("MiniCart", cart);
        }
    }
}
