using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyTroDaiLoi.Migrations
{
    public partial class Loi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 15);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayTao",
                table: "HoaDons",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.InsertData(
                table: "DonVis",
                columns: new[] { "DonViId", "GhiChu", "GiaThue", "LoaiDonVi", "SoDienCu", "SoDienMoi", "SoNuocCu", "SoNuocMoi", "TenDonVi", "TienDien", "TienNuoc" },
                values: new object[,]
                {
                    { 22, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 22", 0m, 0m },
                    { 24, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 24", 0m, 0m },
                    { 25, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 25", 0m, 0m },
                    { 26, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 26", 0m, 0m },
                    { 27, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 27", 0m, 0m },
                    { 111, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 11", 0m, 0m },
                    { 112, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 12", 0m, 0m },
                    { 115, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 15", 0m, 0m },
                    { 116, "", 5000000m, "Nha", 0, null, 0, null, "Nhà 16", 0m, 0m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "DonVis",
                keyColumn: "DonViId",
                keyValue: 116);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayTao",
                table: "HoaDons",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.InsertData(
                table: "DonVis",
                columns: new[] { "DonViId", "GhiChu", "GiaThue", "LoaiDonVi", "SoDienCu", "SoDienMoi", "SoNuocCu", "SoNuocMoi", "TenDonVi", "TienDien", "TienNuoc" },
                values: new object[,]
                {
                    { 5, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 5", 0m, 0m },
                    { 11, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 11", 0m, 0m },
                    { 13, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 13", 0m, 0m },
                    { 14, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 14", 0m, 0m },
                    { 15, "", 1500000m, "Phong", 0, null, 0, null, "Phòng 15", 0m, 0m }
                });
        }
    }
}
