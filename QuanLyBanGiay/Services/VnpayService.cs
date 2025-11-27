using Microsoft.Extensions.Options;
using QuanLyBanGiay.Configuration;
using QuanLyBanGiay.Models.ViewModels;
using QuanLyBanGiay.Models;
using QuanLyBanGiay.Helpers;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;

namespace QuanLyBanGiay.Services
{
    public class VnpayService : IVnpayService
    {
        private readonly VnpaySettings _settings;

        public VnpayService(IOptions<VnpaySettings> settings)
        {
            _settings = settings.Value;
        }
		private string GenerateTxnRef()
		{
			return DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
		}

		//Tạo URL thanh toán
		public string CreatePaymentUrl(Checkout model, HttpContext context)
        {
            var vnpay = new VnPayLibrary();

            var orderId = model.Madh.ToString(); //Lấy Madh làm mã giao dịch

            string txnRef = GenerateTxnRef();

            long totalAmount = (long)Math.Round(model.TotalAmount * 100);

            if (totalAmount <= 0 || model.Madh <= 0)
            {
                return string.Empty;
            }
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }

            //Các tham số bắt buộc
            vnpay.AddRequestData("vnp_Version", _settings.Version);
            vnpay.AddRequestData("vnp_Command", _settings.Command);
            vnpay.AddRequestData("vnp_TmnCode", _settings.TmnCode);
            vnpay.AddRequestData("vnp_Amount", totalAmount.ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", ipAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
			vnpay.AddRequestData("vnp_OrderInfo", $"ThanhToanDonHang_{orderId}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _settings.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", model.Madh.ToString());


			string paymentUrl = vnpay.CreateRequestUrl(_settings.BaseUrl, _settings.HashSecret);
            return paymentUrl;
        }
        
        public VnpayResponseViewModel ProcessVnpayReturn(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            var response = new VnpayResponseViewModel();

            foreach (var key in collections.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, collections[key]);
                }
            }

            //Lấy chữ ký điện tử VNPAY gửi về
            var secureHash = vnpay.GetResponseData("vnp_SecureHash");
            //Xác thực chữ ký
            bool checkSignature = vnpay.ValidateSignature(secureHash, _settings.HashSecret);

            response.TransactionId = vnpay.GetResponseData("vnp_TxnRef");
            response.VnpayTransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            response.ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            response.Amount = vnpay.GetResponseData("vnp_Amount");

            if (!checkSignature)
            {
                response.Success = false;
                response.Message = "Sai chữ ký điện tử (Hash signature) - Giao dịch không hợp lệ.";
            }
            else if (response.ResponseCode != "00")
            {
                response.Success = false;
                response.Message = $"Thanh toán thất bại. Mã lỗi VNPAY: {response.ResponseCode}.";
            }
            else
            {
                response.Success = true;
                response.Message = "Thanh toán thành công.";
            }

            return response;
        }
    }
}