using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySanBong.Models
{
    [Table("tblDatSan")]
    public class DatSan
    {
        [Key]
        [StringLength(50)]
        public string MaDatSan { get; set; } = string.Empty;

        [StringLength(50)]
        [Required(ErrorMessage = "Vui lòng chọn sân")]
        public string? MaSan { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public string? MaKH { get; set; }

        public DateTime GioBatDau { get; set; }
        public DateTime GioKetThuc { get; set; }

        [Column(TypeName = "money")]
        public decimal DonGiaApDung { get; set; }

        [StringLength(50)]
        public string? TrangThaiDat { get; set; }

        [ForeignKey("MaSan")]
        public virtual San? San { get; set; }

        [ForeignKey("MaKH")]
        public virtual KhachHang? KhachHang { get; set; }
    }
}