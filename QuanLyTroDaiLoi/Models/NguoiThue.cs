using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTroDaiLoi.Models
{
    public class NguoiThue
    {
        [Key]
        public int NguoiThueId { get; set; }

        [Required]
        public int DonViId { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;
        [Required(ErrorMessage = "CCCD không được để trống")]
        [StringLength(12, MinimumLength = 9)]
        public string CCCD { get; set; } = string.Empty;
        [Required(ErrorMessage = "SĐT không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SDT { get; set; } = string.Empty;
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string DiaChi { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ngày vào không được để trống")]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime NgayVao { get; set; } = DateTime.Now;


        [ForeignKey(nameof(DonViId))]
        public DonVi? DonVi { get; set; }
    }
}
