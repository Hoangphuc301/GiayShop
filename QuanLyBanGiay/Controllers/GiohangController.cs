using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuanLyBanGiay.Models;
using QuanLyBanGiay.Models.ViewModels;

namespace QuanLyBanGiay.Controllers
{
    public class GiohangController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        private const string CART_KEY = "CartSession";

        private List<CartItem> GetCart()
        {
            var sessionData = HttpContext.Session.GetString(CART_KEY);
            if (string.IsNullOrEmpty(sessionData))
                return new List<CartItem>();

            return JsonConvert.DeserializeObject<List<CartItem>>(sessionData);
        }

        private void SaveCart(List<CartItem> cart)
        {
            var json = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CART_KEY, json);
        }

        public IActionResult Index()
        {
            var cart = GetCart();

            // Lấy thông tin khách từ session/email
            var email = HttpContext.Session.GetString("UserEmail");
            var taikhoan = db.Taikhoans.FirstOrDefault(t => t.Email == email);
            var khach = taikhoan != null
                        ? db.Khachhangs.FirstOrDefault(k => k.Matk == taikhoan.Matk)
                        : null;

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

        public IActionResult MiniCart()
        {
            var cart = GetCart();
            return PartialView("MiniCart", cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int mactsp, int sl = 1)
        {
            var cart = GetCart();

            var ctsp = db.ChitietSanphams
                         .Include(c => c.MamauNavigation)
                         .Include(c => c.MasizeNavigation)
                         .FirstOrDefault(p => p.Mactsp == mactsp);

            if (ctsp == null)
                return NotFound();

            var sanpham = db.Sanphams.FirstOrDefault(sp => sp.Masp == ctsp.Masp);
            if (sanpham == null)
                return NotFound();

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
                    Dongia = ctsp.MaspNavigation.Gia,
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
            if (item != null)
                cart.Remove(item);

            SaveCart(cart);
            return RedirectToAction("Index");
        }
    }
}
