using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class Phuongthucthanhtoan
    {
        public Phuongthucthanhtoan()
        {
            Donhangs = new HashSet<Donhang>();
        }

        public int Mapttt { get; set; }
        public string Tenphuongthuc { get; set; } = null!;

        public virtual ICollection<Donhang> Donhangs { get; set; }
    }
}
