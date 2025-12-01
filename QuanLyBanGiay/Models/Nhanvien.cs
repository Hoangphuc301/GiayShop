namespace QuanLyBanGiay.Models
{
    public partial class Nhanvien
    {
        public int Manv { get; set; }
        public string Tennv { get; set; }
        public string Email { get; set; }
        public string Matkhau { get; set; }
        public string Loainv { get; set; } // ADMIN / NVGH / NVBH
        public bool Trangthai { get; set; }
    }

}
