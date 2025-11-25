namespace QuanLyBanGiay.Configuration
{
    public class VnpaySettings
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;

        public string Version { get; set; }
        public string Command { get; set; }
    }
}
