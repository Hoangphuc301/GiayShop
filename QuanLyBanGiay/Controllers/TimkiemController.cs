using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Controllers
{
    public class TimkiemController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        [HttpGet]
        public IActionResult Index(string q)
        {
            if(string.IsNullOrWhiteSpace(q))
            {
                ViewBag.Message = "Vui lòng nhập từ khóa tìm kiếm.";
                return View(new List<Sanpham>());
            }
            var kq = db.Sanphams
                .Include(sp => sp.MadmNavigation)
                .Include(sp => sp.MathNavigation)
                .Where(sp => 
                    EF.Functions.Like(sp.Tensp, $"%{q}%") ||
                    EF.Functions.Like(sp.MadmNavigation.Tendm, $"%{q}%") ||
                    EF.Functions.Like(sp.MathNavigation.Tenth, $"%{q}%")).ToList();
            ViewBag.Keyword = q;
            return View(kq);
        }
    }
}
