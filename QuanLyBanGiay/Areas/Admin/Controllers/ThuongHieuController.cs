using Microsoft.AspNetCore.Mvc;
using NuGet.ProjectModel;
using QuanLyBanGiay.Models;

namespace QuanLyBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThuongHieuController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();
        public IActionResult Index()
        {
            ViewBag.ThuongHieu = db.Thuonghieus;
            return View();
        }

        //Thêm
        [HttpGet]
        public ActionResult them()
        {
            return View();
        }
        [HttpPost]
        public ActionResult them(Thuonghieu th, IFormFile LogoFile)
        {
            if (LogoFile != null && LogoFile.Length > 0)
            {
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Logo");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var fileName = Path.GetFileName(LogoFile.FileName);

                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    LogoFile.CopyTo(stream);
                }
                th.Logo = fileName;
            }

            db.Thuonghieus.Add(th);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //Sửa
        [HttpGet]
        public ActionResult sua(int id)
        {
            var th = db.Thuonghieus.Find(id);
            if (th == null)
            {
                return RedirectToAction("Index");
			}

			ViewBag.ThuongHieu = th;
            return View(th);
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult sua(Thuonghieu th, IFormFile? LogoFile)
		{
			var thuonghieu = db.Thuonghieus.Find(th.Math);
			if (thuonghieu == null)
			{
				ModelState.AddModelError("", "Sản phẩm này không tồn tại hoặc đã bị xóa.");
				return View(th);
			}

			if (LogoFile != null && LogoFile.Length > 0)
			{
				var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Logo");
				if (!Directory.Exists(uploadFolder))
					Directory.CreateDirectory(uploadFolder);

				var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(LogoFile.FileName)}";
				var filePath = Path.Combine(uploadFolder, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					LogoFile.CopyTo(stream);
				}

				thuonghieu.Logo = fileName;
			}

			thuonghieu.Tenth = th.Tenth;
			thuonghieu.Mota = th.Mota;
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		//Xóa
		[HttpGet]
        public IActionResult xoa(int id)
        {
            var th = db.Thuonghieus.Find(id);
            if (th == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.ThuongHieu = th;

            return View();
        }
        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult xoa_Post(int Math)
        {
            var th = db.Thuonghieus.Find(Math);
            if (th == null)
            {
                return RedirectToAction("Index");
            }

            db.Thuonghieus.Remove(th);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
