using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanGiay.Migrations
{
    public partial class ThemCotPhanHoi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgayPhanHoi",
                table: "Danhgias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhanHoi",
                table: "Danhgias",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgayPhanHoi",
                table: "Danhgias");

            migrationBuilder.DropColumn(
                name: "PhanHoi",
                table: "Danhgias");
        }
    }
}
