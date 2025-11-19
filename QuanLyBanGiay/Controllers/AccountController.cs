using Microsoft.AspNetCore.Mvc;
using QuanLyBanGiay.Models;
using System;
using System.Linq;
using BCrypt.Net;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;


namespace QuanLyBanGiay.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private QL_GiayContext db = new QL_GiayContext();

        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        //Đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string Email, string Password, bool RememberMe)
        {
            //Tìm kiếm user theo email
            var user = db.Khachhangs.FirstOrDefault(u => u.Email == Email && u.Trangthai == true);

            // Kiểm tra người dùng và mật khẩu
            if (user != null && BCrypt.Net.BCrypt.Verify(Password, user.Matkhau))
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Loaitk);
                HttpContext.Session.SetInt32("UserId", user.Makh);

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

        private void SendOtpEmail(string toEmail, string subject, string htmlBody)
        {
            var host = _config["EmailSmtp:Host"];
            var port = int.Parse(_config["EmailSmtp:Port"]);
            var user = _config["EmailSmtp:Username"];
            var pass = _config["EmailSmtp:Password"];
            var fromEmail = _config["EmailSmtp:FromEmail"];
            var fromName = _config["EmailSmtp:FromName"];
            // Tạo 
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();
            // Gửi email
            using var smtp = new SmtpClient();
            smtp.Connect(host, port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(user, pass);
            smtp.Send(message);
            smtp.Disconnect(true);
        }

        //Đăng ký
        [HttpGet]
        public IActionResult FormRegister()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FormRegister(string Email,string Tenkh ,string Matkhau, string NhapLaiMatkhau, string Otp)
        {
            bool hasError = false;

            if (string.IsNullOrEmpty(Otp))
            {
                //Kiểm tra dữ liệu nhập vào
                if (string.IsNullOrWhiteSpace(Email))
                {
                    ViewBag.EmailError = "Vui lòng nhập email";
                    hasError = true;
                }
                else if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(Email))
                {
                    ViewBag.EmailError = "Email không hợp lệ";
                    hasError = true;
                }
                else if (db.Khachhangs.Any(u => u.Email.Trim().ToLower() == Email.Trim().ToLower()))
                {
                    ViewBag.EmailError = "Email đã được sử dụng";
                    hasError = true;
                }

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

                if (string.IsNullOrWhiteSpace(NhapLaiMatkhau) || Matkhau != NhapLaiMatkhau)
                {
                    ViewBag.ConfirmPasswordError = "Mật khẩu nhập lại không khớp";
                    hasError = true;
                }
                // Nếu có lỗi, trả về view với thông báo lỗi
                if (hasError)
                {
                    ViewBag.EmailValue = Email;
                    return View();
                }

                //Tạo OTP
                string otp = new Random().Next(100000, 999999).ToString();
                TempData["Reg_Email"] = Email.Trim();
                TempData["Reg_Tenkh"] = Tenkh?.Trim();
                TempData["Reg_Password"] = Matkhau;
                TempData["Reg_Otp"] = otp;
                TempData["Reg_Time"] = DateTime.Now;
                TempData.Keep();
                //Gửi OTP
                try
                {
                    string subject = "Mã xác nhận đăng ký tài khoản SOFM";
                    string body = $"<p>Mã xác nhận của bạn là: <b>{otp}</b></p><p>Mã có hiệu lực trong 5 phút.</p>";
                    SendOtpEmail(Email, subject, body);

                    ViewBag.ShowOtpInput = true;
                    ViewBag.EmailValue = Email;
                    ViewBag.Success = "Mã xác nhận đã được gửi đến email của bạn!";
                }
                catch (Exception ex)
                {
                    ViewBag.EmailError = "Gửi email thất bại: " + ex.Message;
                }

                return View();
            }
            // Xác thực OTP
            var storedOtp = TempData["Reg_Otp"] as string;
            var storedEmail = TempData["Reg_Email"] as string;
            var storedPassword = TempData["Reg_Password"] as string;
            var storedTime = TempData["Reg_Time"] as DateTime?;

            if (storedOtp == null || storedEmail == null || storedPassword == null || storedTime == null)
            {
                ViewBag.EmailError = "Phiên đăng ký đã hết hạn, vui lòng đăng ký lại.";
                return View();
            }

            if ((DateTime.Now - storedTime.Value).TotalMinutes > 5)
            {
                ViewBag.OtpError = "Mã xác nhận đã hết hạn. Vui lòng đăng ký lại.";
                return View();
            }
            // Kiểm tra OTP
            if (Otp == storedOtp && string.Equals(Email?.Trim(), storedEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                var newUser = new Khachhang
                {
                    Email = storedEmail,
                    Matkhau = BCrypt.Net.BCrypt.HashPassword(storedPassword),
                    Tenkh = TempData["Reg_Tenkh"] as string, // thêm dòng này
                    Loaitk = "USER",
                    Trangthai = true
                };

                db.Khachhangs.Add(newUser);
                db.SaveChanges();

                TempData.Clear();
                ViewBag.Success = "Đăng ký thành công! Bạn có thể đăng nhập.";
                return View();
            }
            else
            {
                // OTP không đúng
                ViewBag.ShowOtpInput = true;
                ViewBag.EmailValue = storedEmail;
                ViewBag.OtpError = "Mã xác nhận không đúng.";
                TempData.Keep();
                return View();
            }
        }

        //Đăng xuất
        public IActionResult Logout()
        {
            // Xóa session khi đăng xuất
            HttpContext.Session.Clear();
            // Xóa cookie
            Response.Cookies.Delete("UserEmail");
            return RedirectToAction("Index", "Home");
        }

        //Quên mật khẩu
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(string Email, string Otp, string NewPassword)
        {
            // Nếu chưa nhập OTP -> Gửi mã
            if (string.IsNullOrEmpty(Otp))
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    ViewBag.Error = "Vui lòng nhập email của bạn.";
                    return View();
                }
                var user = db.Khachhangs.FirstOrDefault(u => u.Email == Email && u.Trangthai == true);
                if (user == null)
                {
                    ViewBag.Error = "Không tìm thấy tài khoản với email này.";
                    return View();
                }

                // Tạo OTP
                string otp = new Random().Next(100000, 999999).ToString();
                TempData["Reset_Email"] = Email;
                TempData["Reset_Otp"] = otp;
                TempData["Reset_Time"] = DateTime.Now;
                TempData.Keep();

                try
                {
                    string subject = "Mã xác nhận đặt lại mật khẩu - SOFM Shoes";
                    string body = $"<p>Mã xác nhận của bạn là: <b>{otp}</b></p><p>Mã có hiệu lực trong 5 phút.</p>";
                    SendOtpEmail(Email, subject, body);

                    ViewBag.Success = "Mã xác nhận đã được gửi đến email của bạn!";
                    ViewBag.ShowOtpInput = true;
                    ViewBag.EmailValue = Email;
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Không thể gửi email: " + ex.Message;
                }

                return View();
            }

            //Nhập OTP và mật khẩu mới
            var storedOtp = TempData["Reset_Otp"] as string;
            var storedEmail = TempData["Reset_Email"] as string;
            var storedTime = TempData["Reset_Time"] as DateTime?;
            // Kiểm tra OTP
            if (storedOtp == null || storedEmail == null || storedTime == null)
            {
                ViewBag.Error = "Phiên khôi phục đã hết hạn, vui lòng thử lại.";
                return View();
            }

            if ((DateTime.Now - storedTime.Value).TotalMinutes > 5)
            {
                ViewBag.Error = "Mã xác nhận đã hết hạn, vui lòng gửi lại.";
                return View();
            }

            if (Otp != storedOtp)
            {
                ViewBag.OtpError = "Mã xác nhận không đúng.";
                ViewBag.ShowOtpInput = true;
                ViewBag.EmailValue = storedEmail;
                TempData.Keep();
                return View();
            }
            // Kiểm tra mật khẩu mới
            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 5)
            {
                ViewBag.PasswordError = "Mật khẩu mới phải có ít nhất 5 ký tự.";
                ViewBag.ShowOtpInput = true;
                ViewBag.EmailValue = storedEmail;
                TempData.Keep();
                return View();
            }

            // Cập nhật mật khẩu mới
            var userReset = db.Khachhangs.FirstOrDefault(u => u.Email == storedEmail);
            if (userReset != null)
            {
                userReset.Matkhau = BCrypt.Net.BCrypt.HashPassword(NewPassword);
                db.SaveChanges();

                TempData.Clear();
                ViewBag.Success = "Mật khẩu đã được đổi thành công! Bạn có thể đăng nhập lại.";
                return View();
            }

            ViewBag.Error = "Có lỗi xảy ra khi cập nhật mật khẩu.";
            return View();
        }
    }
}
