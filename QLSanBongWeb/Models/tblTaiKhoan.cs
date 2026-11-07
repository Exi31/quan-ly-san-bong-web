using System;
using System.Collections.Generic;

namespace QLSanBongWeb.Models;

public partial class tblTaiKhoan
{
    public string MaTK { get; set; } = null!;

    public string TenDangNhap { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public bool TrangThaiHoatDong { get; set; }
}
