using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class Size
    {
        public Size()
        {
            ChitietSanphams = new HashSet<ChitietSanpham>();
        }

        public int Masize { get; set; }
        public string Tensize { get; set; } = null!;

        public virtual ICollection<ChitietSanpham> ChitietSanphams { get; set; }
    }
}
