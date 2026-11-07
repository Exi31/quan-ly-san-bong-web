using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySanBong.Models
{
    [Table("tblSan")]
    public class San
    {
        [Key]
        [StringLength(50)]
        public string MaSan { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TenSan { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LoaiSan { get; set; } = string.Empty;

        [Column(TypeName = "money")]
        public decimal GiaThueGio { get; set; }

        public bool TrangThaiHoatDong { get; set; }
    }
}