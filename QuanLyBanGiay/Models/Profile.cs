using System.ComponentModel.DataAnnotations;

namespace QuanLyBanGiay.Models.ViewModels
{
    public class ProfileViewModel
    {
        public int Makh { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? Tenkh { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(11, ErrorMessage = "Số điện thoại không được vượt quá 11 số")]
        public string? Sdt { get; set; }

        public string? DiachiCuThe { get; set; }
        public string? QuanHuyen { get; set; }
        public string? TinhThanh { get; set; }

        // Ghép địa chỉ đầy đủ
        public string? Diachi
        {
            get
            {
                //Ghép các phần địa chỉ lại với nhau
                string full = "";
                if (!string.IsNullOrEmpty(DiachiCuThe)) full += DiachiCuThe;
                if (!string.IsNullOrEmpty(QuanHuyen)) full += (full.Length > 0 ? ", " : "") + QuanHuyen;
                if (!string.IsNullOrEmpty(TinhThanh)) full += (full.Length > 0 ? ", " : "") + TinhThanh;
                return full;
            }
            set
            {
                //Tách địa chỉ đầy đủ thành các phần
                if (!string.IsNullOrEmpty(value))
                {
                    //Tách địa chỉ theo dấu phẩy và bỏ khoảng trắng thừa
                    var parts = value.Split(',').Select(p => p.Trim()).ToArray();
                    if (parts.Length >= 3)
                    {
                        DiachiCuThe = parts[0];
                        QuanHuyen = parts[1];
                        TinhThanh = parts[2];
                    }
                    else if (parts.Length == 2)
                    {
                        DiachiCuThe = parts[0];
                        TinhThanh = parts[1];
                    }
                    else if (parts.Length == 1)
                    {
                        DiachiCuThe = parts[0];
                    }
                }
            }
        }
    }
}
