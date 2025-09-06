using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyTroDaiLoi.Migrations
{
    public partial class AddDaDongTienToHoaDon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DaDongTien",
                table: "HoaDons",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaDongTien",
                table: "HoaDons");
        }
    }
}
