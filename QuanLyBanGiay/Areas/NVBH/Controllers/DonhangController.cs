using Microsoft.AspNetCore.Mvc;
using QuanLyBanGiay.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyBanGiay.Areas.NVBH.Controllers
{
    [Area("NVBH")]
    public class DonhangController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        public IActionResult Index(int trang = 1, string loaiLoc = "", DateTime? from = null, DateTime? to = null)
        {
            int kichThuocTrang = 10;

            var donhang = db.Donhangs
                .Include(d => d.MakhNavigation)
                .Include(d => d.MaptttNavigation)
                .Include(d => d.MavoucherNavigation)
                .AsQueryable();

            // Lọc theo tuần
            if (loaiLoc == "week")
            {
                var today = DateTime.Today;
                // Chủ nhật là ngày đầu tuần
                int delta = today.DayOfWeek - DayOfWeek.Sunday;

                var dauTuan = today.AddDays(-delta);
                var cuoiTuan = dauTuan.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

                donhang = donhang.Where(d => d.Ngaydat >= dauTuan && d.Ngaydat <= cuoiTuan);
            }

            // Lọc theo tháng
            if (loaiLoc == "month")
            {
                var dauThang = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var cuoiThang = dauThang.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);
                donhang = donhang.Where(d => d.Ngaydat >= dauThang && d.Ngaydat <= cuoiThang);
            }

            // Lọc theo khoảng thời gian
            if (loaiLoc == "range" && from.HasValue && to.HasValue)
            {
                DateTime tuNgay = from.Value.Date;
                DateTime denNgay = to.Value.Date.AddHours(23).AddMinutes(59);

                donhang = donhang.Where(d => d.Ngaydat >= tuNgay && d.Ngaydat <= denNgay);
            }

            // Lưu thông tin lọc cho View
            ViewBag.LoaiLoc = loaiLoc;
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            // Phân trang
            int tongSoMuc = donhang.Count();
            int tongSoTrang = (int)Math.Ceiling((double)tongSoMuc / kichThuocTrang);

            if (tongSoTrang == 0)
                tongSoTrang = 1;

            if (trang < 1)
                trang = 1;
            if (trang > tongSoTrang)
                trang = tongSoTrang;

            var donhangs = donhang
                .OrderByDescending(d => d.Ngaydat)
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
            var dh = db.Donhangs
                        .Include(d => d.MakhNavigation)
                        .FirstOrDefault(d => d.Madh == id);

            if (dh == null)
            {
                return RedirectToAction("Index");
            }
            var thuTuTrangThai = new List<string> { "CHỜ XÁC NHẬN", "ĐANG GIAO", "ĐÃ NHẬN" };

            //Lấy trạng thái hiện tại 
            var trangThaiHienTai = dh.Trangthai;

            //Xác định vị trí trạng thái hiện tại trong danh sách
            var viTriHienTai = thuTuTrangThai.IndexOf(trangThaiHienTai);

            ViewBag.ThuTuTrangThai = thuTuTrangThai;
            ViewBag.ViTriHienTai = viTriHienTai;
            return View(dh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sua(Donhang dh)
        {
            if (dh.Trangthai == "HỦY")
            {
                TempData["ErrorMessage"] = "Nhân viên bán hàng không được phép chuyển trạng thái đơn hàng sang HỦY.";
                return RedirectToAction("Index");
            }

            var donhang = db.Donhangs.Find(dh.Madh);
            var statusOrder = new List<string> { "CHỜ XÁC NHẬN", "ĐANG GIAO", "ĐÃ NHẬN" };

            if (donhang == null)
            {
                ModelState.AddModelError("", "Đơn hàng này không tồn tại hoặc đã bị xóa");
                return View(dh);
            }

            //Kiểm tra tính hợp lệ của việc chuyển trạng thái 
            var currentStatus = donhang.Trangthai;
            var currentIndex = statusOrder.IndexOf(currentStatus);

            // Nếu đơn hàng đã ở trạng thái ĐÃ NHẬN thì không thể chỉnh sửa
            if (currentIndex == statusOrder.IndexOf("ĐÃ NHẬN"))
            {
                TempData["ErrorMessage"] = $"Đơn hàng {donhang.Madh} đã kết thúc ở trạng thái {currentStatus} và không thể chỉnh sửa";
                return RedirectToAction("Index");
            }

            donhang.Trangthai = dh.Trangthai;
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
    }
}
