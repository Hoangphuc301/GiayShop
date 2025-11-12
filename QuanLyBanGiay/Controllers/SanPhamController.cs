using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;
using System.Linq;

namespace QuanLyBanGiay.Controllers
{
    public class SanPhamController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Chitiet(int id)
        {
            // Lấy sản phẩm theo ID và include các liên kết
            var sp = db.Sanphams
                .Include(x => x.MadmNavigation)
                .Include(x => x.MathNavigation)
                .Include(x => x.ChitietSanphams)
                    .ThenInclude(ct => ct.MamauNavigation)
                .Include(x => x.ChitietSanphams)
                    .ThenInclude(ct => ct.MasizeNavigation)
                .FirstOrDefault(x => x.Masp == id);

            // Nếu không tìm thấy sản phẩm
            if (sp == null)
            {
                return RedirectToAction("Index", "Home"); 
            }

            // Truyền dữ liệu sang View
            return View(sp);
        }
    }
}
