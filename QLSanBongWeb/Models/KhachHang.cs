using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySanBong.Models
{
    [Table("tblKhachHang")]
    public class KhachHang
    {
        [Key]
        [Required]
        [RegularExpression(@"^[A-Za-z0-9_-]+$", ErrorMessage = "Mã khách chỉ gồm chữ không dấu, số, _ hoặc -")]
        [StringLength(50)]
        public string MaKH { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        [StringLength(50)]
        public string TenKH { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(50)]
        [RegularExpression(@"^\+?[0-9]{9,20}$", ErrorMessage = "Số điện thoại gồm 9–20 chữ số, có thể bắt đầu bằng +")]
        public string SoDT { get; set; } = string.Empty;
    }
}