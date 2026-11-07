using System;
using System.Collections.Generic;

namespace QLSanBongWeb.Models;

public partial class tblSan
{
    public string MaSan { get; set; } = null!;

    public string TenSan { get; set; } = null!;

    public string LoaiSan { get; set; } = null!;

    public decimal GiaThueGio { get; set; }

    public bool TrangThaiHoatDong { get; set; }

    public virtual ICollection<tblDatSan> tblDatSans { get; set; } = new List<tblDatSan>();
}
