using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
using QuanLySanBong.Models;

namespace QuanLySanBong.Controllers;
public class KhachHangController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;
    public IActionResult Index(string? searchString)
    {
        var query = _context.KhachHangs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchString))
            query = query.Where(k => k.TenKH.Contains(searchString) || k.SoDT.Contains(searchString) || k.MaKH.Contains(searchString));
        ViewBag.SearchString = searchString;
        ViewBag.NextMaKH = TaoMaKhach();
        return View(query.OrderBy(k => k.MaKH).ToList());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Save(KhachHang khachHang, string? actionType)
    {
        khachHang.TenKH = khachHang.TenKH?.Trim() ?? "";
        khachHang.SoDT = khachHang.SoDT?.Trim() ?? "";
        ModelState.Clear();
        TryValidateModel(khachHang);
        if (actionType != "Add" && actionType != "Edit") ModelState.AddModelError("", "Thao tác không hợp lệ.");
        if (!ModelState.IsValid)
        {
            ViewBag.SearchString = "";
            ViewBag.NextMaKH = TaoMaKhach();
            ViewBag.KhachNhap = khachHang;
            ViewBag.CheDo = actionType;
            return View("Index", _context.KhachHangs.AsNoTracking().OrderBy(k => k.MaKH).ToList());
        }
        if (actionType == "Add")
        {
            // Khóa bảng ngắn trong lúc sinh mã KHxxx và thêm; giữ kiểu mã của người 3.
            using var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable);
            _context.Database.ExecuteSqlRaw("SELECT 1 FROM dbo.tblKhachHang WITH (TABLOCKX,HOLDLOCK)");
            khachHang.MaKH = TaoMaKhach();
            _context.KhachHangs.Add(khachHang);
            _context.SaveChanges();
            transaction.Commit();
        }
        else
        {
            var existing = _context.KhachHangs.Find(khachHang.MaKH);
            if (existing == null) { TempData["Error"] = "Không tìm thấy khách hàng cần sửa."; return RedirectToAction(nameof(Index)); }
            existing.TenKH = khachHang.TenKH; existing.SoDT = khachHang.SoDT;
            _context.SaveChanges();
        }
        TempData["Success"] = "Đã lưu thông tin khách hàng.";
        return RedirectToAction(nameof(Index));
    }

    private string TaoMaKhach()
    {
        var codes = _context.KhachHangs.AsNoTracking().Select(k => k.MaKH).ToList();
        int max = codes.Where(c => c.StartsWith("KH") && int.TryParse(c[2..], out int n) && n >= 0 && n < int.MaxValue)
            .Select(c => int.Parse(c[2..])).DefaultIfEmpty(0).Max();
        return "KH" + (max + 1).ToString("D3");
    }
}
