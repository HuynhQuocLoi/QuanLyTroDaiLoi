
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace QuanLyTroDaiLoi.Models
    {
        public class CauHinhThue
        {
            [Key]
            public int Id { get; set; } // luôn 1 record duy nhất
        [Required(ErrorMessage = "Giá thuê phòng không được để trống")]
        [Range(100000, 10000000)]

        [Display(Name = "Giá thuê phòng (VNĐ/tháng)")]
            [Column(TypeName = "decimal(18,2)")]
            public decimal GiaThuePhong { get; set; } = 2000000;

        [Required(ErrorMessage = "Giá thuê nhà không được để trống")]
        [Range(500000, 20000000)]
        [Display(Name = "Giá thuê nhà (VNĐ/tháng)")]
            [Column(TypeName = "decimal(18,2)")]
            public decimal GiaThueNha { get; set; } = 5000000;
        }
    }
