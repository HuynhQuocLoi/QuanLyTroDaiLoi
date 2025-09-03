using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyTroDaiLoi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CauHinhs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DonGiaDien = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DonGiaNuoc = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CauHinhThues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GiaThuePhong = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    GiaThueNha = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhThues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DonVis",
                columns: table => new
                {
                    DonViId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenDonVi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LoaiDonVi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GiaThue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SoDienCu = table.Column<int>(type: "integer", nullable: false),
                    SoNuocCu = table.Column<int>(type: "integer", nullable: false),
                    SoDienMoi = table.Column<int>(type: "integer", nullable: true),
                    SoNuocMoi = table.Column<int>(type: "integer", nullable: true),
                    TienDien = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TienNuoc = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonVis", x => x.DonViId);
                });

            migrationBuilder.CreateTable(
                name: "DonViThangs",
                columns: table => new
                {
                    DonViThangId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DonViId = table.Column<int>(type: "integer", nullable: false),
                    Thang = table.Column<int>(type: "integer", nullable: false),
                    Nam = table.Column<int>(type: "integer", nullable: false),
                    SoDienCu = table.Column<int>(type: "integer", nullable: false),
                    SoDienMoi = table.Column<int>(type: "integer", nullable: false),
                    SoNuocCu = table.Column<int>(type: "integer", nullable: false),
                    SoNuocMoi = table.Column<int>(type: "integer", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    TienDien = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TienNuoc = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonViThangs", x => x.DonViThangId);
                    table.ForeignKey(
                        name: "FK_DonViThangs_DonVis_DonViId",
                        column: x => x.DonViId,
                        principalTable: "DonVis",
                        principalColumn: "DonViId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    HoaDonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DonViId = table.Column<int>(type: "integer", nullable: false),
                    Thang = table.Column<int>(type: "integer", nullable: false),
                    Nam = table.Column<int>(type: "integer", nullable: false),
                    DienCu = table.Column<int>(type: "integer", nullable: false),
                    DienMoi = table.Column<int>(type: "integer", nullable: false),
                    NuocCu = table.Column<int>(type: "integer", nullable: false),
                    NuocMoi = table.Column<int>(type: "integer", nullable: false),
                    DonGiaDien = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DonGiaNuoc = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    TienPhong = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.HoaDonId);
                    table.ForeignKey(
                        name: "FK_HoaDons_DonVis_DonViId",
                        column: x => x.DonViId,
                        principalTable: "DonVis",
                        principalColumn: "DonViId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NguoiThues",
                columns: table => new
                {
                    NguoiThueId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DonViId = table.Column<int>(type: "integer", nullable: false),
                    HoTen = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CCCD = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    SDT = table.Column<string>(type: "text", nullable: false),
                    DiaChi = table.Column<string>(type: "text", nullable: false),
                    NgayVao = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiThues", x => x.NguoiThueId);
                    table.ForeignKey(
                        name: "FK_NguoiThues_DonVis_DonViId",
                        column: x => x.DonViId,
                        principalTable: "DonVis",
                        principalColumn: "DonViId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhiKhacs",
                columns: table => new
                {
                    PhiKhacId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DonViId = table.Column<int>(type: "integer", nullable: false),
                    HoaDonId = table.Column<int>(type: "integer", nullable: false),
                    TenPhi = table.Column<string>(type: "text", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DonViThangId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhiKhacs", x => x.PhiKhacId);
                    table.ForeignKey(
                        name: "FK_PhiKhacs_DonViThangs_DonViThangId",
                        column: x => x.DonViThangId,
                        principalTable: "DonViThangs",
                        principalColumn: "DonViThangId");
                    table.ForeignKey(
                        name: "FK_PhiKhacs_DonVis_DonViId",
                        column: x => x.DonViId,
                        principalTable: "DonVis",
                        principalColumn: "DonViId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhiKhacs_HoaDons_HoaDonId",
                        column: x => x.HoaDonId,
                        principalTable: "HoaDons",
                        principalColumn: "HoaDonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CauHinhs",
                columns: new[] { "Id", "DonGiaDien", "DonGiaNuoc" },
                values: new object[] { 1, 3500m, 10000m });

            migrationBuilder.InsertData(
                table: "DonVis",
                columns: new[] { "DonViId", "GhiChu", "GiaThue", "LoaiDonVi", "SoDienCu", "SoDienMoi", "SoNuocCu", "SoNuocMoi", "TenDonVi", "TienDien", "TienNuoc" },
                values: new object[,]
                {
                    { 1, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 1", 0m, 0m },
                    { 2, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 2", 0m, 0m },
                    { 3, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 3", 0m, 0m },
                    { 4, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 4", 0m, 0m },
                    { 5, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 5", 0m, 0m },
                    { 6, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 6", 0m, 0m },
                    { 7, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 7", 0m, 0m },
                    { 8, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 8", 0m, 0m },
                    { 9, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 9", 0m, 0m },
                    { 10, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 10", 0m, 0m },
                    { 11, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 11", 0m, 0m },
                    { 12, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 12", 0m, 0m },
                    { 13, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 13", 0m, 0m },
                    { 14, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 14", 0m, 0m },
                    { 15, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 15", 0m, 0m },
                    { 16, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 16", 0m, 0m },
                    { 17, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 17", 0m, 0m },
                    { 18, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 18", 0m, 0m },
                    { 19, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 19", 0m, 0m },
                    { 20, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 20", 0m, 0m },
                    { 101, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 1", 0m, 0m },
                    { 102, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 2", 0m, 0m },
                    { 103, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 3", 0m, 0m },
                    { 104, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 4", 0m, 0m },
                    { 105, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 5", 0m, 0m },
                    { 106, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 6", 0m, 0m },
                    { 107, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 7", 0m, 0m },
                    { 108, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 8", 0m, 0m },
                    { 109, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 9", 0m, 0m },
                    { 110, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 10", 0m, 0m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonViThangs_DonViId",
                table: "DonViThangs",
                column: "DonViId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_DonViId",
                table: "HoaDons",
                column: "DonViId");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiThues_DonViId",
                table: "NguoiThues",
                column: "DonViId");

            migrationBuilder.CreateIndex(
                name: "IX_PhiKhacs_DonViId",
                table: "PhiKhacs",
                column: "DonViId");

            migrationBuilder.CreateIndex(
                name: "IX_PhiKhacs_DonViThangId",
                table: "PhiKhacs",
                column: "DonViThangId");

            migrationBuilder.CreateIndex(
                name: "IX_PhiKhacs_HoaDonId",
                table: "PhiKhacs",
                column: "HoaDonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHinhs");

            migrationBuilder.DropTable(
                name: "CauHinhThues");

            migrationBuilder.DropTable(
                name: "NguoiThues");

            migrationBuilder.DropTable(
                name: "PhiKhacs");

            migrationBuilder.DropTable(
                name: "DonViThangs");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "DonVis");
        }
    }
}
