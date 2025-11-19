using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class Khachhang
    {
        public Khachhang()
        {
            Donhangs = new HashSet<Donhang>();
        }

        public int Makh { get; set; }
        public string Email { get; set; } = null!;
        public string Matkhau { get; set; } = null!;
        public string Loaitk { get; set; } = null!;
        public string? Tenkh { get; set; }
        public string? Sdt { get; set; }
        public string? Diachi { get; set; }
        public bool? Trangthai { get; set; }
        public string? MaXacNhan { get; set; }
        public DateTime? ThoiGianTaoOtp { get; set; }
        public bool? DaXacNhan { get; set; }

        public virtual ICollection<Donhang> Donhangs { get; set; }
    }
}
