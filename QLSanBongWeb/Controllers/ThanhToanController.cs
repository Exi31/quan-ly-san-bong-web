using System.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSanBongWeb.Data;
using QLSanBongWeb.Models;

namespace QLSanBongWeb.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly QLSanBongContext _context;

        public ThanhToanController(QLSanBongContext context)
        {
            _context = context;
        }

        // Hiển thị trang thanh toán
        public async Task<IActionResult> Index(string maHD)
        {
            if (string.IsNullOrEmpty(maHD))
                return BadRequest();

            var hoaDon = await _context.tblHoaDon
                .FirstOrDefaultAsync(hd => hd.MaHD == maHD);

            if (hoaDon == null)
                return NotFound();

            if (await _context.tblDatSan.AnyAsync(d => d.MaDatSan == hoaDon.MaDatSan && d.TrangThaiDat == "Đã hủy"))
            {
                TempData["ThongBao"] = "Lịch đã hủy, không thể thanh toán.";
                return RedirectToAction("Index", "HoaDon");
            }
            if (hoaDon.TrangThaiThanhToan)
            {
                TempData["ThongBao"] = "Hóa đơn này đã được thanh toán!";
                return RedirectToAction("Index", "HoaDon");
            }

            return View(hoaDon);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanThanhToan(
    string maHD, string phuongThuc)
        {
            if (string.IsNullOrWhiteSpace(maHD))
                return BadRequest();

            if (phuongThuc != "TienMat" &&
                phuongThuc != "ChuyenKhoan")
            {
                return BadRequest("Phương thức thanh toán không hợp lệ!");
            }

            var invoice = await _context.tblHoaDon.AsNoTracking().FirstOrDefaultAsync(h => h.MaHD == maHD);
            if (invoice == null) return NotFound();
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var booking = await _context.tblDatSan.FromSql($"SELECT * FROM dbo.tblDatSan WITH (UPDLOCK,HOLDLOCK) WHERE MaDatSan={invoice.MaDatSan}").SingleOrDefaultAsync();
            if (booking == null || booking.TrangThaiDat == "Đã hủy")
            {
                TempData["ThongBao"] = "Lịch không tồn tại hoặc đã hủy, không thể thanh toán.";
                return RedirectToAction("Index", "HoaDon");
            }
            var now = DateTime.Now;
            int changed = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.tblHoaDon SET TrangThaiThanhToan=1, ThoiDiemThanhToan={now} WHERE MaHD={maHD} AND TrangThaiThanhToan=0");
            await transaction.CommitAsync();
            TempData["ThongBao"] = changed == 1 ? "Xác nhận thanh toán thành công!" : "Hóa đơn này đã được thanh toán trước đó!";
            return RedirectToAction("Index", "HoaDon");
        }
    }
}
