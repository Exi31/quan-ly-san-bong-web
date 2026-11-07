using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QLSanBongWeb.Services;

namespace QLSanBongMini.Controllers;

// Giữ tên action và giao diện người 2; bổ sung xác minh hash và cookie.
public class AccountController(IConfiguration configuration) : Controller
{
    [AllowAnonymous, HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
        ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : null;
        return View();
    }
    [AllowAnonymous, HttpPost]
    public async Task<IActionResult> Login(string? tenDangNhap, string? matKhau, string? returnUrl)
    {
        ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : null;
        ViewBag.TenDangNhap = tenDangNhap;
        if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrEmpty(matKhau) || tenDangNhap.Length > 50 || matKhau.Length > 512)
        {
            ModelState.AddModelError("", "Vui lòng nhập tên đăng nhập (tối đa 50 ký tự) và mật khẩu hợp lệ.");
            return View();
        }
        using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        using var command = new SqlCommand("SELECT MaTK,TenDangNhap,MatKhauHash,VaiTro,TrangThaiHoatDong FROM dbo.tblTaiKhoan WHERE TenDangNhap=@ten", connection);
        command.Parameters.Add("@ten", System.Data.SqlDbType.NVarChar, 50).Value = tenDangNhap.Trim();
        using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync() || !BamMatKhau.KiemTra(matKhau, reader.GetString(2)))
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác.");
            return View();
        }
        if (!reader.GetBoolean(4)) { ModelState.AddModelError("", "Tài khoản đã bị khóa."); return View(); }
        string role = reader.GetString(3).Trim();
        if (role != "Quản lý" && role != "Nhân viên")
        {
            ModelState.AddModelError("", "Vai trò tài khoản chưa được cấu hình hợp lệ."); return View();
        }
        string username = reader.GetString(1);
        var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, reader.GetString(0)),
            new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, role)
        }, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        HttpContext.Session.Clear();
        HttpContext.Session.SetString("User", username);
        HttpContext.Session.SetString("Role", role);
        return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction("Index", "Home");
    }
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
    public IActionResult AccessDenied() => StatusCode(403, "Bạn không có quyền thực hiện chức năng này.");
}
