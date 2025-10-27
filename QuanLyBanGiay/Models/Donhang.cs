using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class Donhang
    {
        public Donhang()
        {
            ChitietDonhangs = new HashSet<ChitietDonhang>();
        }

        public int Madh { get; set; }
        public int Makh { get; set; }
        public int Mapttt { get; set; }
        public int? Mavoucher { get; set; }
        public DateTime? Ngaydat { get; set; }
        public decimal? Tongtien { get; set; }
        public string Trangthai { get; set; } = null!;
        public string? Diachigiao { get; set; }
        public string? Sdtgiao { get; set; }
        public decimal? Phiship { get; set; }
        public decimal? Tongtiencuoi { get; set; }
        public string? Lydohuy { get; set; }

        public virtual Khachhang MakhNavigation { get; set; } = null!;
        public virtual Phuongthucthanhtoan MaptttNavigation { get; set; } = null!;
        public virtual Voucher? MavoucherNavigation { get; set; }
        public virtual ICollection<ChitietDonhang> ChitietDonhangs { get; set; }
    }
}
