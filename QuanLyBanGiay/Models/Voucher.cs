using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

        // 1: Theo phần trăm, 2: Theo số tiền, 3: Miễn phí vận chuyển
        public int LoaiKhuyenMai { get; set; }

        // Giá trị giảm (Nếu là %, lưu số 10, 20... Nếu là tiền, lưu số tiền VNĐ. Nếu FreeShip thì null hoặc 0)
        public decimal? Giatri { get; set; }

        public int PhamViApDung { get; set; }

        public string? GiaTriPhamVi { get; set; }

        public int NhomKhachHang { get; set; }

        public int? GioiHanSuDung { get; set; }

        public int DaSuDung { get; set; } = 0;

        public bool GioiHanMoiKhach { get; set; } = false;

        public DateTime? Ngaybd { get; set; }
        public DateTime? Ngaykt { get; set; }
        public string Trangthai { get; set; } = null!;

        public virtual ICollection<Donhang> Donhangs { get; set; }
    }
}