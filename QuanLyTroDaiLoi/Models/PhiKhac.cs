using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTroDaiLoi.Models
{
    public class PhiKhac
    {
        [Key]
        public int PhiKhacId { get; set; }

        public int DonViId { get; set; }
        [ForeignKey(nameof(DonViId))]
        public DonVi DonVi { get; set; } = null!;

        // ✅ Thêm khoá ngoại tới HoaDon
        public int HoaDonId { get; set; }
        [ForeignKey(nameof(HoaDonId))]
        public HoaDon HoaDon { get; set; } = null!;
        [Required(ErrorMessage = "Tên phí không được để trống")]
        public string TenPhi { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        [Range(0, 100000000)]
        public decimal DonGia { get; set; }

        [Required]
        [Range(1, 1000)]
        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        [Range(0, 100000000)]
        public decimal ThanhTien { get; set; }
    }
}
