using System.ComponentModel.DataAnnotations;

namespace QuanLyBanGiay.Models
{
    public class Phancong
    {
        public int Madh { get; set; }
        [Display(Name = "Khách hàng")]
        public string? TenKhachHang { get; set; }
        [Display(Name = "Trạng thái")]
        public string? TrangThaiHienTai { get; set; }
        public string? TenNhanVienHienTai { get; set; }
        [Display(Name = "Nhân viên giao hàng")]
        [Required(ErrorMessage = "Bạn phải chọn nhân viên giao hàng")]
        public int? MaNvg { get; set; }
    }
}