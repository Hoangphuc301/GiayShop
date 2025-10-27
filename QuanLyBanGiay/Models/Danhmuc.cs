using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class Danhmuc
    {
        public Danhmuc()
        {
            Sanphams = new HashSet<Sanpham>();
        }

        public int Madm { get; set; }
        public string Tendm { get; set; } = null!;
        public string? Mota { get; set; }

        public virtual ICollection<Sanpham> Sanphams { get; set; }
    }
}
