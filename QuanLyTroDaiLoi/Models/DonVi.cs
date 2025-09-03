using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTroDaiLoi.Models
{
    public class DonVi
    {
        [Key]
        public int DonViId { get; set; }

       
        [Required(ErrorMessage = "Tên phòng/nhà không được để trống")]
        [StringLength(100)]
        public string TenDonVi { get; set; } = string.Empty;
        [StringLength(50)]
        public string? LoaiDonVi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100000000)]
        public decimal GiaThue { get; set; }

        public int SoDienCu { get; set; }
        public int SoNuocCu { get; set; }
        public int? SoDienMoi { get; set; }
        public int? SoNuocMoi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienDien { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienNuoc { get; set; }
        [StringLength(1000)]
        public string? GhiChu { get; set; }

        // Navigation
        public List<NguoiThue> NguoiThues { get; set; } = new();
        public List<PhiKhac> PhiKhacs { get; set; } = new();
        public List<DonViThang> DonViThangs { get; set; } = new();

        public void KhoiTaoChiSoDau(int dien, int nuoc)
        {
            SoDienCu = dien;
            SoNuocCu = nuoc;
            SoDienMoi = dien;
            SoNuocMoi = nuoc;
        }

        public decimal GetTienDien(CauHinh config)
            => (((SoDienMoi ?? SoDienCu) - SoDienCu) * config.DonGiaDien);

        public decimal GetTienNuoc(CauHinh config)
            => (((SoNuocMoi ?? SoNuocCu) - SoNuocCu) * config.DonGiaNuoc);

        [NotMapped]
        public decimal TongTien => GiaThue + TienDien + TienNuoc + (PhiKhacs?.Sum(p => p.ThanhTien) ?? 0);
    }
}
