using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanGiay.Migrations
{
    public partial class ThemCotDaNopTienCODChoDonHang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                    name: "DaNopTienCOD",
                    table: "Donhang",
                    type: "bit",
                    nullable: false, 
                    defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
          name: "DaNopTienCOD",
          table: "Donhang");
        }
    }
}
