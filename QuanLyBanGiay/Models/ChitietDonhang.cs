using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class ChitietDonhang
    {
        public int Madh { get; set; }
        public int Mactsp { get; set; }
        public int? Sl { get; set; }
        public decimal? Dongia { get; set; }
        public decimal? Thanhtien { get; set; }

        public virtual ChitietSanpham MactspNavigation { get; set; } = null!;
        public virtual Donhang MadhNavigation { get; set; } = null!;
    }
}
