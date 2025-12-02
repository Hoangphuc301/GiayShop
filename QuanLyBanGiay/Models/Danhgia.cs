using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanGiay.Models
{
    public class Danhgia
    {
        [Key]
        public int MaDanhGia { get; set; }

        [ForeignKey("Khachhang")]
        public int MaKh { get; set; }

        [ForeignKey("Sanpham")]
        public int MaSp { get; set; }

        [Range(1, 5)]
        public int Sao { get; set; }

        public string NoiDung { get; set; } = null!;

        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        public bool CoTheDanhGia { get; set; }

        public virtual Khachhang Khachhang { get; set; } = null!;
        public virtual Sanpham Sanpham { get; set; } = null!;
    }
}
