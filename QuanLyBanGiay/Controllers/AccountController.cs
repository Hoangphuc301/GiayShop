using Microsoft.AspNetCore.Mvc;
using QuanLyBanGiay.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.CodeAnalysis.Scripting;
using BCrypt.Net;

namespace QuanLyBanGiay.Controllers
{
    public class AccountController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string Email, string Password, bool RememberMe)
        {
            //Tìm kiếm user theo email
            var user = db.Taikhoans.FirstOrDefault(u => u.Email == Email && u.Trangthai == true);

            // Kiểm tra người dùng và mật khẩu
            if (user != null && BCrypt.Net.BCrypt.Verify(Password, user.Matkhau))
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Loaitk);
                HttpContext.Session.SetInt32("UserId", user.Matk);

                // Lưu cookie nếu chọn "Remember Me"
                if (RememberMe)
                {
                    Response.Cookies.Append("UserEmail", user.Email, new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7)
                    });
                }

                // Chuyển hướng theo loại tài khoản
                if (user.Loaitk == "ADMIN")
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                else
                    return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng!";
            return View();
        }

        [HttpGet]
        public IActionResult FormRegister()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo vệ chống tấn công CSRF (Cross site request forgery)
        public IActionResult FormRegister(string Email, string Matkhau, string NhapLaiMatkhau)
        {
            bool hasError = false; //Đặt cờ kiểm tra lỗi

            // Kiểm tra email
            if (string.IsNullOrWhiteSpace(Email))
            {
                ViewBag.EmailError = "Vui lòng nhập email";
                hasError = true;
            }
            else if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(Email))
            {
                ViewBag.EmailError = "Địa chỉ email không hợp lệ";
                hasError = true;
            }
            else if (db.Taikhoans.Any(u => u.Email == Email)) //Trong linq .Any() kiểm tra tồn tại
            {
                ViewBag.EmailError = "Email đã được sử dụng";
                hasError = true;
            }

            // Kiểm tra mật khẩu
            if (string.IsNullOrWhiteSpace(Matkhau))
            {
                ViewBag.PasswordError = "Vui lòng nhập mật khẩu";
                hasError = true;
            }
            else if (Matkhau.Length < 5)
            {
                ViewBag.PasswordError = "Mật khẩu phải có ít nhất 5 ký tự";
                hasError = true;
            }

            // Kiểm tra nhập lại mật khẩu
            if (string.IsNullOrWhiteSpace(NhapLaiMatkhau))
            {
                ViewBag.ConfirmPasswordError = "Vui lòng nhập lại mật khẩu";
                hasError = true;
            }
            else if (Matkhau != NhapLaiMatkhau)
            {
                ViewBag.ConfirmPasswordError = "Mật khẩu nhập lại không khớp";
                hasError = true;
            }

            if (hasError)
            {
                ViewBag.EmailValue = Email; // giữ lại email và gửi lên form
                return View();
            }

            // Mã hóa mật khẩu
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Matkhau);

            // Tạo tài khoản mới
            var newUser = new Taikhoan
            {
                Email = Email,
                Matkhau = hashedPassword,
                Loaitk = "USER",
                Trangthai = true
            };

            db.Taikhoans.Add(newUser);
            db.SaveChanges();

            ViewBag.Success = "Đăng ký thành công! Giờ bạn có thể đăng nhập.";
            return View();
        }

        public IActionResult Logout()
        {
            // Xóa session khi đăng xuất
            HttpContext.Session.Clear();
            // Xóa cookie
            Response.Cookies.Delete("UserEmail");
            return RedirectToAction("Index", "Home");
        }
    }
}
