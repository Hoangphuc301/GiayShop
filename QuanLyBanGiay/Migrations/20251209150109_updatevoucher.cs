using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanGiay.Migrations
{
    public partial class updatevoucher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DaSuDung",
                table: "VOUCHER",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GiaTriPhamVi",
                table: "VOUCHER",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GioiHanMoiKhach",
                table: "VOUCHER",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "GioiHanSuDung",
                table: "VOUCHER",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiKhuyenMai",
                table: "VOUCHER",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NhomKhachHang",
                table: "VOUCHER",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PhamViApDung",
                table: "VOUCHER",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaSuDung",
                table: "VOUCHER");

            migrationBuilder.DropColumn(
                name: "GiaTriPhamVi",
                table: "VOUCHER");

            migrationBuilder.DropColumn(
                name: "GioiHanMoiKhach",
                table: "VOUCHER");

            migrationBuilder.DropColumn(
                name: "GioiHanSuDung",
                table: "VOUCHER");

            migrationBuilder.DropColumn(
                name: "LoaiKhuyenMai",
                table: "VOUCHER");

            migrationBuilder.DropColumn(
                name: "NhomKhachHang",
                table: "VOUCHER");

            migrationBuilder.DropColumn(
                name: "PhamViApDung",
                table: "VOUCHER");
        }
    }
}
