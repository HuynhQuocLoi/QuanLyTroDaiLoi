using QuanLyTroDaiLoi.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLyTroDaiLoi.Models
{
    public class HoaDon
    {
        [Key]
        public int HoaDonId { get; set; }

        public int DonViId { get; set; }
        [ForeignKey(nameof(DonViId))]
        public DonVi DonVi { get; set; } = null!;

        public int Thang { get; set; }
        public int Nam { get; set; }

        public int DienCu { get; set; }
        public int DienMoi { get; set; }
        public int NuocCu { get; set; }
        public int NuocMoi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaDien { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaNuoc { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        public DateTime NgayTao { get; set; }

        public List<PhiKhac> PhiKhacs { get; set; } = new();
        public bool IsClosed { get; set; } = false;
        [Column(TypeName = "decimal(18,2)")]
        public decimal TienPhong { get; set; }
        public bool DaDongTien { get; set; } = false;

    }
}
