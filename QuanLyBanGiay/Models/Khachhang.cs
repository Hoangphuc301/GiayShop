using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class Khachhang
    {
        public Khachhang()
        {
            ChitietGiohangs = new HashSet<ChitietGiohang>();
            Donhangs = new HashSet<Donhang>();
        }

        public int Makh { get; set; }
        public string Tenkh { get; set; } = null!;
        public int Matk { get; set; }
        public string? Sdt { get; set; }
        public string? Diachi { get; set; }

        public virtual Taikhoan MatkNavigation { get; set; } = null!;
        public virtual ICollection<ChitietGiohang> ChitietGiohangs { get; set; }
        public virtual ICollection<Donhang> Donhangs { get; set; }
    }
}
