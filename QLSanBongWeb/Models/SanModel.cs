using System.ComponentModel.DataAnnotations;

namespace QLSanBongMini.Models
{
    public class SanModel
    {
        [Required(ErrorMessage = "Mã sân không được để trống")]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z0-9_-]+$", ErrorMessage = "Mã sân chỉ gồm chữ không dấu, số, _ hoặc -")]
        public string MaSan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên sân không được để trống")]
        [StringLength(50)]
        public string TenSan { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^Sân (5|7) người$", ErrorMessage = "Chọn loại sân 5 hoặc 7 người")]
        public string LoaiSan { get; set; } = string.Empty;

        [Range(0.01, 922337203685477.58, ErrorMessage = "Giá thuê phải lớn hơn 0")]
        public decimal GiaThueGio { get; set; }

        public bool TrangThaiHoatDong { get; set; }
    }
}