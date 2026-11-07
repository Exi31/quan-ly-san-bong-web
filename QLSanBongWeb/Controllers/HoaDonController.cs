using System.Data;
using QLSanBongWeb.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSanBongWeb.Models;
using QLSanBongWeb.Data;

namespace QLSanBongWeb.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly QLSanBongContext _context;

        public HoaDonController(QLSanBongContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách hóa đơn và lịch đặt sân
        public async Task<IActionResult> Index()
        {
            var model = new HoaDonViewModel();

            model.DanhSachHoaDon = await _context.tblHoaDon.OrderByDescending(h => h.NgayLap)
                .Select(hd => new HoaDonItem
                {
                    MaHD = hd.MaHD,
                    MaDat = hd.MaDatSan,
                    TongTien = hd.TongTien,
                    TrangThaiLich = hd.MaDatSanNavigation.TrangThaiDat,
                    TrangThai = hd.TrangThaiThanhToan
                        ? "Đã thanh toán"
                        : "Chưa thanh toán"
                })
                .ToListAsync();

            model.DanhSachLichDatSan = await _context.tblDatSan
                .Where(ds => ds.tblHoaDon == null && ds.TrangThaiDat != "Đã hủy")
                .Select(ds => new LichDatSanItem
                {
                    MaDatSan = ds.MaDatSan,
                    TenKhachHang = ds.MaKHNavigation.TenKH,
                    TenSan = ds.MaSanNavigation.TenSan
                })
                .ToListAsync();

            return View(model);
        }

        // Lấy thông tin chi tiết lịch đặt sân
        public async Task<IActionResult> LayThongTinDatSan(string maDatSan)
        {
            if (string.IsNullOrEmpty(maDatSan))
                return BadRequest();

            var datSan = await _context.tblDatSan
                .Where(ds => ds.MaDatSan == maDatSan && ds.TrangThaiDat != "Đã hủy")
                .Select(ds => new
                {
                    ds.MaDatSan,
                    TenKhachHang = ds.MaKHNavigation.TenKH,
                    TenSan = ds.MaSanNavigation.TenSan,
                    ds.GioBatDau,
                    ds.GioKetThuc,
                    ds.DonGiaApDung
                })
                .FirstOrDefaultAsync();

            if (datSan == null)
                return NotFound();

            double thoiLuong =
                (datSan.GioKetThuc - datSan.GioBatDau).TotalHours;

            decimal tongTien =
                NghiepVu.TinhTien(datSan.GioBatDau, datSan.GioKetThuc, datSan.DonGiaApDung);

            return Json(new
            {
                maDatSan = datSan.MaDatSan,
                tenKhachHang = datSan.TenKhachHang,
                tenSan = datSan.TenSan,
                gioBatDau = datSan.GioBatDau.ToString("dd/MM/yyyy HH:mm"),
                gioKetThuc = datSan.GioKetThuc.ToString("dd/MM/yyyy HH:mm"),
                thoiLuong = thoiLuong,
                donGia = datSan.DonGiaApDung,
                tongTien = tongTien
            });
        }

        // Lập hóa đơn
        [HttpPost]
        public async Task<IActionResult> LapHoaDon(string maDatSan)
        {
            if (string.IsNullOrWhiteSpace(maDatSan))
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng chọn lịch đặt sân!"
                });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var datSan = await _context.tblDatSan
                .FromSql($"SELECT * FROM dbo.tblDatSan WITH (UPDLOCK,HOLDLOCK) WHERE MaDatSan={maDatSan}")
                .Include(ds => ds.tblHoaDon)
                .FirstOrDefaultAsync(ds => ds.MaDatSan == maDatSan);

            if (datSan == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Không tìm thấy lịch đặt sân!"
                });
            }

            if (datSan.TrangThaiDat == "Đã hủy")
                return Json(new { success = false, message = "Không thể lập hóa đơn cho lịch đã hủy." });

            if (datSan.tblHoaDon != null)
            {
                return Json(new
                {
                    success = false,
                    message = "Lịch đặt sân này đã có hóa đơn!"
                });
            }

            var danhSachMaHD = await _context.tblHoaDon
    .Where(hd => hd.MaHD.StartsWith("HD"))
    .Select(hd => hd.MaHD)
    .ToListAsync();

            int soTiepTheo = danhSachMaHD
                .Select(ma => int.TryParse(ma.Substring(2), out int so) ? so : 0)
                .DefaultIfEmpty(0)
                .Max() + 1;

            string maHD = $"HD{soTiepTheo:D3}";

            double thoiLuong =
                (datSan.GioKetThuc - datSan.GioBatDau).TotalHours;

            decimal tongTien =
                NghiepVu.TinhTien(datSan.GioBatDau, datSan.GioKetThuc, datSan.DonGiaApDung);

            var hoaDon = new tblHoaDon
            {
                MaHD = maHD,
                MaDatSan = datSan.MaDatSan,
                NgayLap = DateTime.Now,
                TongTien = tongTien,
                TrangThaiThanhToan = false,
                ThoiDiemThanhToan = null
            };

            _context.tblHoaDon.Add(hoaDon);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Json(new
            {
                success = true,
                message = "Lập hóa đơn thành công!",
                maHD = maHD,
                maDatSan = datSan.MaDatSan,
                tongTien = tongTien,
                trangThai = "Chưa thanh toán"
            });
        }
    }
}
