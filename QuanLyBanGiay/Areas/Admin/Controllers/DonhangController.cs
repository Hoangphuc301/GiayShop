using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
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

            // Tránh lỗi OFFSET âm
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
