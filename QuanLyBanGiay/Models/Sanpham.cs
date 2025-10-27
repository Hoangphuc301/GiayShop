using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanGiay.Models
{
    public partial class Sanpham
    {
        public Sanpham()
        {
            ChitietSanphams = new HashSet<ChitietSanpham>();
        }

        public int Masp { get; set; }
        public int Madm { get; set; }
        public int Math { get; set; }
        public string Tensp { get; set; } = null!;
        public decimal Gia { get; set; }
        public string? Mota { get; set; }
        public string? Hinhdaidien { get; set; }
        public string Trangthai { get; set; } = null!;

        public virtual Danhmuc MadmNavigation { get; set; } = null!;
        public virtual Thuonghieu MathNavigation { get; set; } = null!;
        public virtual ICollection<ChitietSanpham> ChitietSanphams { get; set; }
    }
}
