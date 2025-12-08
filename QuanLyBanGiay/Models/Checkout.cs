using QuanLyBanGiay.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public int? Mavoucher { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal PhiShip { get; set; }
        public int Madh { get; set; }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<SelectListItem> PaymentMethods { get; set; } = new List<SelectListItem>();
    }
}