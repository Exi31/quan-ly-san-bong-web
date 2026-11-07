using System;
using System.Collections.Generic;

namespace QLSanBongWeb.Models
{
    public class HoaDonViewModel
    {
        public string? MaDat { get; set; } 
        public string? TenKhachHang { get; set; } 
        public string? TenSan { get; set; }
        public DateTime? GioBatDau { get; set; }
        public DateTime? GioKetThuc { get; set;  }
        public decimal? DonGia { get; set; }
        public decimal? TongTien { get; set; }
        public string? TrangThaiThanhToan { get; set; }
        public List<HoaDonItem> DanhSachHoaDon { get; set; }
            = new List<HoaDonItem>();
        public List<LichDatSanItem> DanhSachLichDatSan { get; set; }
    = new List<LichDatSanItem>();
    }
    public class HoaDonItem
    {
        public string? MaHD { get; set; }
        public string? MaDat { get; set; }
        public decimal? TongTien { get; set; }
        public string? TrangThai { get; set; }
        public string? TrangThaiLich { get; set; }
    }
    public class LichDatSanItem
    {
        public string? MaDatSan { get; set; }
        public string? TenKhachHang { get; set; }
        public string? TenSan { get; set; }
    }
}
