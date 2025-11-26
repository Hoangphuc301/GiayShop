using QuanLyBanGiay.Models;
using QuanLyBanGiay.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace QuanLyBanGiay.Services
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(Checkout model, HttpContext context);
        VnpayResponseViewModel ProcessVnpayReturn(IQueryCollection collections);
    }
}