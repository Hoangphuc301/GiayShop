using QuanLyBanGiay.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering; //Cần thêm namespace này cho SelectListItem

namespace QuanLyBanGiay.Models.ViewModels
{
    public class Checkout
    {
        public int Makh { get; set; }
        public string Tenkh { get; set; } = "";
        public string Email { get; set; } = "";
        public string Sdt { get; set; } = "";
        public string Diachi { get; set; } = "";
        public int Mapttt { get; set; }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<SelectListItem> PaymentMethods { get; set; } = new List<SelectListItem>();
    }
}