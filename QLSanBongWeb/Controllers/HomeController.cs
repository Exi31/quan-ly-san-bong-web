using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSanBongWeb.Data;
using QLSanBongWeb.Models;
namespace QLSanBongWeb.Controllers;

public class HomeController(QLSanBongContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        DateTime today = DateTime.Today, tomorrow = today.AddDays(1);
        ViewBag.SoSan = await context.tblSan.CountAsync(s => s.TrangThaiHoatDong);
        ViewBag.SoKhach = await context.tblKhachHang.CountAsync();
        ViewBag.LichHomNay = await context.tblDatSan.CountAsync(d => d.TrangThaiDat != "Đã hủy" && d.GioBatDau < tomorrow && d.GioKetThuc > today);
        ViewBag.HoaDonCho = await context.tblHoaDon.CountAsync(h => !h.TrangThaiThanhToan && h.MaDatSanNavigation.TrangThaiDat != "Đã hủy");
        return View();
    }
    [AllowAnonymous, ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
