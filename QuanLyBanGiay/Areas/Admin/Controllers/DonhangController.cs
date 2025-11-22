using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DonhangController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();

        public IActionResult Index(int trang = 1)
        {
            int kichThuocTrang = 10;

            var donhang = db.Donhangs
                .Include(d => d.MakhNavigation)
                .Include(d => d.MaptttNavigation)
                .Include(d => d.MavoucherNavigation)
                .OrderByDescending(d => d.Ngaydat)
                .AsQueryable();

            int tongSoMuc = donhang.Count();
            int tongSoTrang = (int)Math.Ceiling((double)tongSoMuc / kichThuocTrang);

            if(trang < 1)
                trang = 1;
            if (trang > tongSoTrang)
                trang = tongSoTrang;    

            var donhangs = donhang
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToList();

            ViewBag.Donhang = donhangs;
            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;
            return View();
        }

        // Sửa 
        [HttpGet]
        public IActionResult Sua(int id)
        {
            //Truy vấn và lấy đơn hàng theo madh
            var dh = db.Donhangs
                        .Include(d => d.MakhNavigation)
                        .FirstOrDefault(d => d.Madh == id);

            if (dh == null)
            {
                return RedirectToAction("Index");
            }
            //Tạo danh sách thứ tự trạng thái
            var thuTuTrangThai = new List<string> { "CHỜ XÁC NHẬN", "ĐANG GIAO", "ĐÃ NHẬN", "HỦY" };

            //Lấy trạng thái hiện tại 
            var trangThaiHienTai = dh.Trangthai;

            //Xác định vị trí trạng thái hiện tại trong danh sách (IndexOf là tìm vị trí phần tử cụ thể)
            var viTriHienTai = thuTuTrangThai.IndexOf(trangThaiHienTai);

            ViewBag.ThuTuTrangThai = thuTuTrangThai; 
            ViewBag.ViTriHienTai = viTriHienTai;
            return View(dh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sua(Donhang dh)
        {
            var donhang = db.Donhangs.Find(dh.Madh);
            //Xác định thứ tự trạng thái để kiểm tra tính hợp lệ
            var statusOrder = new List<string> { "CHỜ XÁC NHẬN", "ĐANG GIAO", "ĐÃ NHẬN", "HỦY" };
            if (donhang == null)
            {
                ModelState.AddModelError("", "Đơn hàng này không tồn tại hoặc đã bị xóa");
                return View(dh);
            }
            //Kiểm tra tính hợp lệ của việc chuyển trạng thái 
            var currentStatus = donhang.Trangthai;
            var currentIndex = statusOrder.IndexOf(currentStatus);
            //Nếu đơn hàng đã ở trạng thái ĐÃ NHẬN hoặc HỦY thì không thể chỉnh sửa
            if (currentIndex == statusOrder.IndexOf("ĐÃ NHẬN") || currentIndex == statusOrder.IndexOf("HỦY"))
            {
                TempData["ErrorMessage"] = $"Đơn hàng {donhang.Madh} đã kết thúc ở trạng thái {currentStatus} và không thể chỉnh sửa";
                return RedirectToAction("Index");
            }
            //Cập nhật trạng thái mới 
            donhang.Trangthai = dh.Trangthai;
            donhang.Lydohuy = dh.Trangthai == "HỦY" ? dh.Lydohuy : null;

            db.SaveChanges();
            TempData["SuccessMessage"] = $"Cập nhật trạng thái đơn hàng {donhang.Madh} thành công";
            return RedirectToAction("Index");
        }

        //Xem chi tiết
        public IActionResult XemChiTiet(int id)
        {
            var chitiets = db.ChitietDonhangs
                .Include(ct => ct.MactspNavigation)
                    .ThenInclude(ctsp => ctsp.MaspNavigation)
                .Include(ct => ct.MactspNavigation.MamauNavigation)
                .Include(ct => ct.MactspNavigation.MasizeNavigation)
                .Where(ct => ct.Madh == id)
                .ToList();

            var dh = db.Donhangs
                .Include(d => d.MakhNavigation)
                .FirstOrDefault(d => d.Madh == id);

            ViewBag.DonHang = dh;
            return View(chitiets);
        }

        //Hủy
        [HttpGet]
        public IActionResult Huy(int id)
        {
            var dh = db.Donhangs
                           .Include(d => d.MakhNavigation)
                           .FirstOrDefault(d => d.Madh == id);

            if (dh == null)
                return RedirectToAction("Index");

            if (dh.Trangthai == "HỦY" || dh.Trangthai == "ĐÃ NHẬN")
            {
                return RedirectToAction("Index");
            }
            return View(dh);
        }

        [HttpPost, ActionName("Huy")]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanHuy(int Madh, string Lydohuy)
        {
            var dh = db.Donhangs.Find(Madh);

            if (dh == null)
            {
                return RedirectToAction("Index");
            }
            dh.Trangthai = "HỦY";
            dh.Lydohuy = Lydohuy ?? "Khách hàng yêu cầu hủy đơn.";

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
