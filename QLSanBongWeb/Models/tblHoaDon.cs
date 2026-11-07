using System;
using System.Collections.Generic;

namespace QLSanBongWeb.Models;

public partial class tblHoaDon
{
    public string MaHD { get; set; } = null!;

    public string MaDatSan { get; set; } = null!;

    public DateTime NgayLap { get; set; }

    public decimal TongTien { get; set; }

    public bool TrangThaiThanhToan { get; set; }

    public DateTime? ThoiDiemThanhToan { get; set; }

    public virtual tblDatSan MaDatSanNavigation { get; set; } = null!;
}
