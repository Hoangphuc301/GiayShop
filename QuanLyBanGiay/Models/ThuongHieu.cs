using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class Thuonghieu
    {
        public Thuonghieu()
        {
            Sanphams = new HashSet<Sanpham>();
        }

        public int Math { get; set; }
        public string Tenth { get; set; } = null!;
        public string? Mota { get; set; }
        public string? Logo { get; set; }

        public virtual ICollection<Sanpham> Sanphams { get; set; }
    }
}
