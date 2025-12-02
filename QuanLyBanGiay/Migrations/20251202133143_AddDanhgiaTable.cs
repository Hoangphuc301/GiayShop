using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanGiay.Migrations
{
    public partial class AddDanhgiaTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DANHGIA",
                columns: table => new
                {
                    MaDanhGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKh = table.Column<int>(type: "int", nullable: false),
                    MaSp = table.Column<int>(type: "int", nullable: false),
                    Sao = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime", nullable: false),
                    CoTheDanhGia = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANHGIA", x => x.MaDanhGia);
                    table.ForeignKey(
                        name: "FK_DANHGIA_KH",
                        column: x => x.MaKh,
                        principalTable: "KHACHHANG",
                        principalColumn: "MAKH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DANHGIA_SP",
                        column: x => x.MaSp,
                        principalTable: "SANPHAM",
                        principalColumn: "MASP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DANHGIA_MaKh",
                table: "DANHGIA",
                column: "MaKh");

            migrationBuilder.CreateIndex(
                name: "IX_DANHGIA_MaSp",
                table: "DANHGIA",
                column: "MaSp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DANHGIA");
        }
    }
}
