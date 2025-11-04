using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanGiay.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace QuanLyBanGiay.Controllers
{
    public class HomeController : Controller
    {
        private QL_GiayContext db = new QL_GiayContext();


        [HttpGet]
        public IActionResult Index(int[] madm, int[] math, int[] mamau, int[] masize, int? gia)
        {
            try
            {
                var danhMucSanPham = db.Danhmucs
                    .Include(dm => dm.Sanphams)
                        .ThenInclude(sp => sp.MathNavigation)
                    .ToList();

                var spQuery = db.Sanphams
                    .Include(s => s.MadmNavigation)
                    .Include(s => s.MathNavigation)
                    .Include(s => s.ChitietSanphams)
                        .ThenInclude(ct => ct.MamauNavigation)
                    .Include(s => s.ChitietSanphams)
                        .ThenInclude(ct => ct.MasizeNavigation)
                    .AsQueryable();

                if (madm != null && madm.Any())
                    spQuery = spQuery.Where(x => madm.Contains(x.Madm));

                if (math != null && math.Any())
                    spQuery = spQuery.Where(x => math.Contains(x.Math));

                if (mamau != null && mamau.Any())
                    spQuery = spQuery.Where(x => x.ChitietSanphams.Any(ct => mamau.Contains(ct.Mamau)));

                if (masize != null && masize.Any())
                    spQuery = spQuery.Where(x => x.ChitietSanphams.Any(ct => masize.Contains(ct.Masize)));

                if (gia != null)
                {
                    switch (gia)
                    {
                        case 1: spQuery = spQuery.Where(x => x.Gia < 1000000); break;
                        case 2: spQuery = spQuery.Where(x => x.Gia >= 1000000 && x.Gia <= 2000000); break;
                        case 3: spQuery = spQuery.Where(x => x.Gia > 2000000 && x.Gia <= 3000000); break;
                        case 4: spQuery = spQuery.Where(x => x.Gia > 3000000); break;
                    }
                }

                var sanPhamDaLoc = spQuery.ToList();

                if ((madm != null && madm.Any()) || (math != null && math.Any()) ||
                    (mamau != null && mamau.Any()) || (masize != null && masize.Any()) || gia != null)
                {
                    danhMucSanPham = danhMucSanPham
                        .Where(dm => sanPhamDaLoc.Any(sp => sp.Madm == dm.Madm)) 
                        .Select(dm =>
                        {
                            dm.Sanphams = sanPhamDaLoc.Where(sp => sp.Madm == dm.Madm).ToList();
                            return dm;
                        })
                        .ToList();
                }

                ViewBag.DanhMucSanPham = danhMucSanPham;
                LoadFilterData();

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi HomeController : " + ex.Message);

                ViewBag.DanhMucSanPham = new List<Danhmuc>();
                LoadFilterData();
                return View();
            }
        }

        private void LoadFilterData()
        {
            ViewBag.DanhMuc = db.Danhmucs.ToList();
            ViewBag.ThuongHieu = db.Thuonghieus.ToList();
            ViewBag.Mau = db.Maus.ToList();
            ViewBag.Size = db.Sizes.ToList();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
