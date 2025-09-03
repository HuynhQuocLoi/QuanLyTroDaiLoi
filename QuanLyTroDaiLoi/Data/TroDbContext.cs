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
            for (int i = 1; i <= 20; i++)
            {
                modelBuilder.Entity<DonVi>().HasData(new DonVi
                {
                    DonViId = i,
                    TenDonVi = $"Phòng {i}",
                    GiaThue = 1500000m,
                    LoaiDonVi = "Phong",
                    SoDienCu = 0,
                    SoNuocCu = 0,
                    GhiChu = ""
                });
            }

            // Seed 10 nhà mặc định
            for (int i = 1; i <= 10; i++)
            {
                modelBuilder.Entity<DonVi>().HasData(new DonVi
                {
                    DonViId = 100 + i, // tránh trùng ID với phòng
                    TenDonVi = $"Nhà {i}",
                    GiaThue = 5000000m, // giá thuê mặc định cho nhà
                    LoaiDonVi = "Nha",
                    SoDienCu = 0,
                    SoNuocCu = 0,
                    GhiChu = ""
                });
            }
        }
    }
}
