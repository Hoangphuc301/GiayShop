using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            Donhangs = new HashSet<Donhang>();
        }

        public int Mavoucher { get; set; }
        public string? Tenvoucher { get; set; }
        public string? Magiamgia { get; set; }
        public decimal? Giatri { get; set; }
        public DateTime? Ngaybd { get; set; }
        public DateTime? Ngaykt { get; set; }
        public string Trangthai { get; set; } = null!;

        public virtual ICollection<Donhang> Donhangs { get; set; }
    }
}
