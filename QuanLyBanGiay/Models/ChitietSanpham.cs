using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class ChitietSanpham
    {
        public ChitietSanpham()
        {
            ChitietDonhangs = new HashSet<ChitietDonhang>();
        }

        public int Mactsp { get; set; }
        public int Masp { get; set; }
        public int Mamau { get; set; }
        public int Masize { get; set; }
        public int? Slton { get; set; }

        public virtual Mau MamauNavigation { get; set; } = null!;
        public virtual Size MasizeNavigation { get; set; } = null!;
        public virtual Sanpham MaspNavigation { get; set; } = null!;
        public virtual ICollection<ChitietDonhang> ChitietDonhangs { get; set; }
    }
}
