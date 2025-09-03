using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTroDaiLoi.Models
{
    public class CauHinh
    {
        [Key]
        public int Id { get; set; } // luôn 1 record duy nhất
        [Required(ErrorMessage = "Đơn giá điện không được để trống")]
        [Range(100, 1000000, ErrorMessage = "Đơn giá điện phải từ 100 đến 1,000,000 VNĐ/kWh")]
        [Display(Name = "Đơn giá điện (VNĐ/kWh)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaDien { get; set; } = 3500;
        [Required(ErrorMessage = "Đơn giá nước không được để trống")]
        [Range(1000, 1000000, ErrorMessage = "Đơn giá nước phải từ 1,000 đến 1,000,000 VNĐ/m³")]
        [Display(Name = "Đơn giá nước (VNĐ/m³)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaNuoc { get; set; } = 10000;
    }

}
