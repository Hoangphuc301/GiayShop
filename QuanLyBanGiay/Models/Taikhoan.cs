using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class Taikhoan
    {
        public Taikhoan()
        {
            Khachhangs = new HashSet<Khachhang>();
        }

        public int Matk { get; set; }
        public string Email { get; set; } = null!;
        public string Matkhau { get; set; } = null!;
        public string Loaitk { get; set; } = null!;
        public bool? Trangthai { get; set; }

        public virtual ICollection<Khachhang> Khachhangs { get; set; }
    }
}
