using Microsoft.AspNetCore.Mvc;
using QuanLyBanGiay.Models;
using System.Linq;

namespace QuanLyBanGiay.Controllers
{
    public class ThuonghieuController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        public IActionResult Index()
        {
            var thuonghieus = db.Thuonghieus.ToList();
            return View(thuonghieus);
        }

        //Dropdown hiển thị danh sách thương hiệu
        public PartialViewResult DanhSachThuongHieu()
        {
            var thuonghieus = db.Thuonghieus.ToList();
            return PartialView("ThuongHieuMegaMenu", thuonghieus);
        }

        //Trang sản phẩm theo thương hiệu
        public IActionResult SanPhamTheoThuongHieu(int id)
        {
            var thuonghieu = db.Thuonghieus.FirstOrDefault(t => t.Math == id);
            if (thuonghieu == null)
                return NotFound();

            var sanphams = db.Sanphams.Where(s => s.Math == id).ToList();
            ViewBag.TenThuongHieu = thuonghieu.Tenth;
            return View(sanphams);
        }
    }
}
