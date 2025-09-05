using Microsoft.EntityFrameworkCore;
using QuanLyTroDaiLoi.Models;

namespace QuanLyTroDaiLoi.Data
{
    public class TroDbContext : DbContext
    {
        public TroDbContext(DbContextOptions<TroDbContext> options) : base(options) { }

        public DbSet<DonVi> DonVis { get; set; }
        public DbSet<CauHinh> CauHinhs { get; set; }
        public DbSet<PhiKhac> PhiKhacs { get; set; }
        public DbSet<DonViThang> DonViThangs { get; set; }
        public DbSet<NguoiThue> NguoiThues { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<CauHinhThue> CauHinhThues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Chỉ có 1 record CauHinh
            modelBuilder.Entity<CauHinh>().HasData(new CauHinh
            {
                Id = 1,
                DonGiaDien = 3500m,
                DonGiaNuoc = 10000m
            });

            // Seed 20 phòng mặc định
            var danhSachPhong = new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 12, 16, 17, 18, 19, 20, 22, 24, 25, 26, 27 };
            foreach (var phong in danhSachPhong)
            {
                modelBuilder.Entity<DonVi>().HasData(new DonVi
                {
                    DonViId = phong,
                    TenDonVi = $"Phòng {phong}",
                    GiaThue = 1500000m,
                    LoaiDonVi = "Phong",
                    SoDienCu = 0,
                    SoNuocCu = 0,
                    GhiChu = ""
                });
            }

            var danhSachNha = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 15, 16 };
            foreach (var nha in danhSachNha)
            {
                modelBuilder.Entity<DonVi>().HasData(new DonVi
                {
                    DonViId = 100 + nha, // tránh trùng ID với phòng
                    TenDonVi = $"Nhà {nha}",
                    GiaThue = 5000000m,
                    LoaiDonVi = "Nha",
                    SoDienCu = 0,
                    SoNuocCu = 0,
                    GhiChu = ""
                });
            }
        }
    }
}
