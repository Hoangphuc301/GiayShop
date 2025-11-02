using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanGiay.Models
{
    public partial class Taikhoan
    {
        public Taikhoan()
        {
            Khachhangs = new HashSet<Khachhang>();
        }

        public int Matk { get; set; }
        public string Email { get; set; } = null!;
        public string Matkhau { get; set; } = null!;
        [NotMapped]
        [Compare("Matkhau", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [Display(Name = "Nhập lại mật khẩu")]
        public string NhapLaiMatkhau { get; set; } = null!;

        public string Loaitk { get; set; } = null!;
        public bool? Trangthai { get; set; }

        public virtual ICollection<Khachhang> Khachhangs { get; set; }
    }
}
