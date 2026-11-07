using System;
using System.Collections.Generic;

namespace QLSanBongWeb.Models;

public partial class tblDatSan
{
    public string MaDatSan { get; set; } = null!;

    public string MaSan { get; set; } = null!;

    public string MaKH { get; set; } = null!;

    public DateTime GioBatDau { get; set; }

    public DateTime GioKetThuc { get; set; }

    public decimal DonGiaApDung { get; set; }

    public string TrangThaiDat { get; set; } = null!;

    public virtual tblKhachHang MaKHNavigation { get; set; } = null!;

    public virtual tblSan MaSanNavigation { get; set; } = null!;

    public virtual tblHoaDon? tblHoaDon { get; set; }
}
