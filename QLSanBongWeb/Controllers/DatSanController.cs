using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Models;
using QuanLySanBong.Data;
using QLSanBongWeb.Services;

namespace QuanLySanBong.Controllers;

public class DatSanController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public IActionResult Index(DateTime? ngayXem, string? maSan)
    {
        var date = (ngayXem ?? DateTime.Today).Date;
        if (date >= DateTime.MaxValue.Date) return BadRequest();
        var next = date.AddDays(1);
        var query = _context.DatSans.AsNoTracking().Include(d => d.KhachHang).Include(d => d.San)
            .Where(d => d.TrangThaiDat != "Đã hủy" && d.GioBatDau < next && d.GioKetThuc > date);
        if (!string.IsNullOrEmpty(maSan) && maSan != "ALL") query = query.Where(d => d.MaSan == maSan);
        var sans = _context.Sans.AsNoTracking().OrderBy(s => s.MaSan).ToList();
        ViewBag.SanHienThi = sans.Where(s => string.IsNullOrEmpty(maSan) || maSan == "ALL" || s.MaSan == maSan).ToList();
        ViewBag.DanhSachSan = new SelectList(sans, "MaSan", "TenSan", maSan);
        ViewBag.NgayXem = date.ToString("yyyy-MM-dd");
        ViewBag.MaSanDangChon = maSan;
        // Giữ ô lịch đã có hóa đơn để xem, nhưng không bật nút hủy.
        ViewBag.LichCoHoaDon = _context.Database.SqlQuery<string>($"SELECT MaDatSan AS Value FROM dbo.tblHoaDon").ToList().ToHashSet();
        return View(query.OrderBy(d => d.GioBatDau).ToList());
    }

    public IActionResult Create(DateTime? ngayChon, string? maSan)
    {
        var date = (ngayChon ?? DateTime.Today).Date;
        if (!ngayChon.HasValue && date.AddHours(17) <= DateTime.Now) date = date.AddDays(1);
        var model = new DatSan { GioBatDau = date.AddHours(17), GioKetThuc = date.AddHours(18), MaSan = maSan };
        NapDanhSach(model);
        return View(model);
    }

    public IActionResult GiaSan(string? maSan)
    {
        var san = _context.Sans.AsNoTracking().FirstOrDefault(s => s.MaSan == maSan && s.TrangThaiHoatDong);
        return san == null ? NotFound() : Json(new { donGia = san.GiaThueGio });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(DatSan datSan)
    {
        if (datSan.GioKetThuc <= datSan.GioBatDau) ModelState.AddModelError("", "Giờ kết thúc phải lớn hơn giờ bắt đầu.");
        if (datSan.GioBatDau < DateTime.Now) ModelState.AddModelError("", "Không thể đặt sân trong quá khứ.");
        if ((datSan.GioKetThuc - datSan.GioBatDau).TotalHours > 24) ModelState.AddModelError("", "Mỗi lượt đặt tối đa 24 giờ.");
        if (datSan.GioBatDau.Minute % 30 != 0 || datSan.GioKetThuc.Minute % 30 != 0 || datSan.GioBatDau.Second != 0 || datSan.GioKetThuc.Second != 0)
            ModelState.AddModelError("", "Vui lòng chọn khung giờ theo bước 30 phút.");
        if (ModelState.IsValid)
        {
            using var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable);
            // Khóa cùng một sân để hai yêu cầu không cùng vượt qua bước kiểm tra trùng.
            var san = _context.Sans.FromSql($"SELECT * FROM dbo.tblSan WITH (UPDLOCK,HOLDLOCK) WHERE MaSan={datSan.MaSan}").SingleOrDefault();
            if (san == null || !san.TrangThaiHoatDong) ModelState.AddModelError("", "Sân không tồn tại hoặc đang ngừng hoạt động.");
            else if (!_context.KhachHangs.Any(k => k.MaKH == datSan.MaKH)) ModelState.AddModelError("", "Khách hàng không tồn tại.");
            else if (_context.DatSans.Any(d => d.MaSan == datSan.MaSan && d.TrangThaiDat != "Đã hủy" && datSan.GioBatDau < d.GioKetThuc && datSan.GioKetThuc > d.GioBatDau))
                ModelState.AddModelError("", "Khung giờ này đã có khách đặt trên sân này. Vui lòng chọn khung giờ khác.");
            else
            {
                // Tạo mã đặt sân theo dạng DS001, DS002...
                var danhSachMa = _context.DatSans
                    .Select(d => d.MaDatSan)
                    .ToList();

                int soMoi = danhSachMa
                    .Where(m => m != null &&
                                m.StartsWith("DS") &&
                                int.TryParse(m.Substring(2), out _))
                    .Select(m => int.Parse(m.Substring(2)))
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                datSan.MaDatSan = "DS" + soMoi.ToString("D3");

                datSan.DonGiaApDung = san.GiaThueGio;
                datSan.TrangThaiDat = "Đã đặt";
                // Không nhận navigation entities do phía trình duyệt gửi lên.
                datSan.San = null; datSan.KhachHang = null;
                _context.DatSans.Add(datSan);
                _context.SaveChanges();
                transaction.Commit();
                TempData["Success"] = "Đặt sân thành công.";
                return RedirectToAction(nameof(Index), new { ngayXem = datSan.GioBatDau.ToString("yyyy-MM-dd"), maSan = datSan.MaSan });
            }
        }
        NapDanhSach(datSan);
        return View(datSan);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult HuyLich(string? id, string? ngayXem)
    {
        string ngay = ngayXem ?? DateTime.Today.ToString("yyyy-MM-dd");
        using var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable);
        var datSan = _context.DatSans.FromSql($"SELECT * FROM dbo.tblDatSan WITH (UPDLOCK,HOLDLOCK) WHERE MaDatSan={id}").SingleOrDefault();
        if (datSan == null) TempData["Error"] = "Không tìm thấy lịch đặt.";
        else if (datSan.TrangThaiDat != "Đã đặt") TempData["Error"] = "Chỉ có thể hủy lịch đang ở trạng thái Đã đặt.";
        else if (_context.Database.SqlQuery<int>($"SELECT COUNT(*) AS Value FROM dbo.tblHoaDon WHERE MaDatSan={id}").Single() > 0)
            TempData["Error"] = "Lịch đã có hóa đơn, không thể hủy.";
        else
        {
            ngay = datSan.GioBatDau.ToString("yyyy-MM-dd");
            datSan.TrangThaiDat = "Đã hủy";
            _context.SaveChanges();
            TempData["Success"] = "Đã hủy lịch đặt.";
        }
        transaction.Commit();
        return RedirectToAction(nameof(Index), new { ngayXem = ngay });
    }

    private void NapDanhSach(DatSan model)
    {
        ViewBag.MaKH = new SelectList(_context.KhachHangs.AsNoTracking().OrderBy(k => k.TenKH), "MaKH", "TenKH", model.MaKH);
        ViewBag.MaSan = new SelectList(_context.Sans.AsNoTracking().Where(s => s.TrangThaiHoatDong).OrderBy(s => s.MaSan), "MaSan", "TenSan", model.MaSan);
    }
}
