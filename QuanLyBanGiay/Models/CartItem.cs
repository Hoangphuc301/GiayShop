namespace QuanLyBanGiay.Models
{
    public class CartItem
    {
        public int Mactsp { get; set; }
        public string Tensp { get; set; } = string.Empty;
        public string? Hinhanh { get; set; }
        public decimal Dongia { get; set; }
        public int Sl { get; set; }
        public string Mau { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
    }
}
