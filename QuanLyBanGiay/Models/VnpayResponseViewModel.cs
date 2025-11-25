namespace QuanLyBanGiay.Models
{
    public class VnpayResponseViewModel
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty; 
        public string VnpayTransactionId { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
    }
}