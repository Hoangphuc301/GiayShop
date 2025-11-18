using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChitietsanphamController : Controller
    {
        private QL_GiayContext db  = new QL_GiayContext();
        public IActionResult Index(int trang = 1)
        {
            int kichThuocTrang = 10;

            var chitietspQuery = db.ChitietSanphams
                .Include(c => c.MaspNavigation)
                .Include(c => c.MamauNavigation)
                .Include(c => c.MasizeNavigation)
                .AsQueryable(); 

            int tongSoMuc = chitietspQuery.Count();
            int tongSoTrang = (int)Math.Ceiling((double)tongSoMuc / kichThuocTrang);

            if (trang < 1) trang = 1;
            if (trang > tongSoTrang) trang = tongSoTrang;

            var chitietsp = chitietspQuery
                .OrderByDescending(c => c.Mactsp) 
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToList();

            ViewBag.Chitietsanpham = chitietsp;
            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;
            return View();
        }

        //thêm
        [HttpGet]
        public ActionResult them()
        {
            ViewBag.SanPhamList = db.Sanphams.ToList();
            ViewBag.MauList = db.Maus.ToList();
            ViewBag.SizeList = db.Sizes.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult them(ChitietSanpham ctsp)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (ctsp.Masp == 0 || ctsp.Mamau == 0 || ctsp.Masize == 0)
                {
                    TempData["Error"] = "Vui lòng chọn đầy đủ sản phẩm, màu và size!";
                    ViewBag.SanPhamList = db.Sanphams.ToList();
                    ViewBag.MauList = db.Maus.ToList();
                    ViewBag.SizeList = db.Sizes.ToList();
                    return View(ctsp);
                }

                db.ChitietSanphams.Add(ctsp);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Khi có lỗi, load lại dropdown để hiển thị lại view
                TempData["Error"] = "Lỗi khi thêm chi tiết sản phẩm: " + ex.Message;
                ViewBag.SanPhamList = db.Sanphams.ToList();
                ViewBag.MauList = db.Maus.ToList();
                ViewBag.SizeList = db.Sizes.ToList();
                return View(ctsp);
            }
        }

        //sửa
        [HttpGet]
        public ActionResult sua(int id)
        {
            var ctsp = db.ChitietSanphams.Find(id);
            if (ctsp == null)
            {
                TempData["Error"] = "Không tìm thấy chi tiết sản phẩm.";
                return RedirectToAction("Index");
            }

            ViewBag.SanphamList = db.Sanphams.ToList();
            ViewBag.MauList = db.Maus.ToList();
            ViewBag.SizeList = db.Sizes.ToList();
            ViewBag.ChiTietSP = ctsp;

            return View(ctsp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sua(ChitietSanpham ctsp)
        {
            System.Diagnostics.Debug.WriteLine("Mactsp POST: " + ctsp.Mactsp);

            var csp = db.ChitietSanphams.Find(ctsp.Mactsp);
            if (csp == null)
            {
                TempData["Error"] = "Sản phẩm này không tồn tại hoặc đã bị xóa.";
                return RedirectToAction("Index");
            }

            csp.Masp = ctsp.Masp;
            csp.Mamau = ctsp.Mamau;
            csp.Masize = ctsp.Masize;
            csp.Slton = ctsp.Slton;

            db.SaveChanges();
            TempData["Success"] = "Cập nhật chi tiết sản phẩm thành công!";
            return RedirectToAction("Index");
        }


        //xóa
        [HttpGet]
        public ActionResult xoa(int id)
        {
            var ctsp = db.ChitietSanphams
                .Include(c => c.MaspNavigation)
                .Include(c => c.MamauNavigation)
                .Include(c => c.MasizeNavigation)
                .FirstOrDefault(c => c.Mactsp == id);

            if (ctsp == null)
            {
                TempData["Error"] = "Không tìm thấy chi tiết sản phẩm cần xóa!";
                return RedirectToAction("Index");
            }

            return View(ctsp);
        }
        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public ActionResult XacNhanXoa(int id)
        {
            bool daCoDonHang = db.ChitietDonhangs.Any(c => c.Mactsp == id);
            if (daCoDonHang)
            {
                TempData["Error"] = "❌ Không thể xóa vì chi tiết sản phẩm này đã được bán trong đơn hàng.";
                return RedirectToAction("Index");
            }

            var ctsp = db.ChitietSanphams.Find(id);
            if (ctsp == null)
            {
                TempData["Error"] = "Chi tiết sản phẩm không tồn tại hoặc đã bị xóa.";
                return RedirectToAction("Index");
            }

            db.ChitietSanphams.Remove(ctsp);
            db.SaveChanges();

            return RedirectToAction("Index");
        }





    }
}
