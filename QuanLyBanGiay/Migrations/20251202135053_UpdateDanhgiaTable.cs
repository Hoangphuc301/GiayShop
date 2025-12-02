using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanGiay.Migrations
{
    public partial class UpdateDanhgiaTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DANHGIA_KH",
                table: "DANHGIA");

            migrationBuilder.DropForeignKey(
                name: "FK_DANHGIA_SP",
                table: "DANHGIA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DANHGIA",
                table: "DANHGIA");

            migrationBuilder.RenameTable(
                name: "DANHGIA",
                newName: "Danhgias");

            migrationBuilder.RenameIndex(
                name: "IX_DANHGIA_MaSp",
                table: "Danhgias",
                newName: "IX_Danhgias_MaSp");

            migrationBuilder.RenameIndex(
                name: "IX_DANHGIA_MaKh",
                table: "Danhgias",
                newName: "IX_Danhgias_MaKh");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayDanhGia",
                table: "Danhgias",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Danhgias",
                table: "Danhgias",
                column: "MaDanhGia");

            migrationBuilder.AddForeignKey(
                name: "FK_Danhgias_KHACHHANG_MaKh",
                table: "Danhgias",
                column: "MaKh",
                principalTable: "KHACHHANG",
                principalColumn: "MAKH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Danhgias_SANPHAM_MaSp",
                table: "Danhgias",
                column: "MaSp",
                principalTable: "SANPHAM",
                principalColumn: "MASP",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Danhgias_KHACHHANG_MaKh",
                table: "Danhgias");

            migrationBuilder.DropForeignKey(
                name: "FK_Danhgias_SANPHAM_MaSp",
                table: "Danhgias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Danhgias",
                table: "Danhgias");

            migrationBuilder.RenameTable(
                name: "Danhgias",
                newName: "DANHGIA");

            migrationBuilder.RenameIndex(
                name: "IX_Danhgias_MaSp",
                table: "DANHGIA",
                newName: "IX_DANHGIA_MaSp");

            migrationBuilder.RenameIndex(
                name: "IX_Danhgias_MaKh",
                table: "DANHGIA",
                newName: "IX_DANHGIA_MaKh");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayDanhGia",
                table: "DANHGIA",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DANHGIA",
                table: "DANHGIA",
                column: "MaDanhGia");

            migrationBuilder.AddForeignKey(
                name: "FK_DANHGIA_KH",
                table: "DANHGIA",
                column: "MaKh",
                principalTable: "KHACHHANG",
                principalColumn: "MAKH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DANHGIA_SP",
                table: "DANHGIA",
                column: "MaSp",
                principalTable: "SANPHAM",
                principalColumn: "MASP",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
