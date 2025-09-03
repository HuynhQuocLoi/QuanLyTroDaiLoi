using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLyTroDaiLoi.Models
{
    public class DonViThang
    {
        [Key]
        public int DonViThangId { get; set; }

        public int DonViId { get; set; }
        [ForeignKey(nameof(DonViId))]
        public DonVi DonVi { get; set; } = null!;

        public int Thang { get; set; }
        public int Nam { get; set; }

        public int SoDienCu { get; set; }
        public int SoDienMoi { get; set; }

        public int SoNuocCu { get; set; }
        public int SoNuocMoi { get; set; }

        public string? GhiChu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienDien { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienNuoc { get; set; }

        public decimal GetTienDien(CauHinh config) => (SoDienMoi - SoDienCu) * config.DonGiaDien;
        public decimal GetTienNuoc(CauHinh config) => (SoNuocMoi - SoNuocCu) * config.DonGiaNuoc;

        public List<PhiKhac> PhiKhacs { get; set; } = new();
        public bool IsClosed { get; set; } = false;
    }

}
