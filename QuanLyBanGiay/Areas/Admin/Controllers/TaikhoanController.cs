using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaikhoanController : Controller
	{
		private QL_GiayContext db = new QL_GiayContext();
		public IActionResult Index(int trang = 1)
		{
            int kichThuocTrang = 8;
            var khachhang = db.Khachhangs.AsQueryable();

            int tongSoMuc = khachhang.Count();  
            int tongSoTrang = (int)Math.Ceiling((double)tongSoMuc / kichThuocTrang);

            if(trang < 1)
                                trang = 1;
            if(trang > tongSoTrang)
                                trang = tongSoTrang;
            
            var khachhangs = khachhang
                .OrderByDescending(kh => kh.Makh)
                .Skip((trang - 1) * kichThuocTrang) 
                .Take(kichThuocTrang)
                .ToList();


			ViewBag.Khachhang = khachhangs;
            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;
			return View();
		}

        //Thêm
        [HttpGet]
        public ActionResult them()
        {
            return View();
        }
        [HttpPost]
        public ActionResult them(Khachhang tk)
        {
            db.Khachhangs.Add(tk);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
