using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class ChitietGiohang
    {
        public int Makh { get; set; }
        public int Mactsp { get; set; }
        public int? Sl { get; set; }
        public decimal Dongia { get; set; }

        public virtual ChitietSanpham MactspNavigation { get; set; } = null!;
        public virtual Khachhang MakhNavigation { get; set; } = null!;
    }
}
