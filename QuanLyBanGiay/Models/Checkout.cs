using QuanLyBanGiay.Models;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models.ViewModels
{
    public class Checkout
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Thông tin khách hàng
        public int Makh { get; set; }
        public string Tenkh { get; set; }
        public string Email { get; set; }
        public string Sdt { get; set; }
        public string Diachi { get; set; }

        // Thông tin người nhận
        public string SdtNguoiNhan { get; set; }
    }
}
