using System;
using System.Collections.Generic;

namespace QLSanBongWeb.Models;

public partial class tblKhachHang
{
    public string MaKH { get; set; } = null!;

    public string TenKH { get; set; } = null!;

    public string SoDT { get; set; } = null!;

    public virtual ICollection<tblDatSan> tblDatSans { get; set; } = new List<tblDatSan>();
}
