using System;
using System.Collections.Generic;

namespace QuanLyBanGiay.Models
{
    public partial class Mau
    {
        public Mau()
        {
            ChitietSanphams = new HashSet<ChitietSanpham>();
        }

        public int Mamau { get; set; }
        public string Tenmau { get; set; } = null!;

        public virtual ICollection<ChitietSanpham> ChitietSanphams { get; set; }
    }
}
