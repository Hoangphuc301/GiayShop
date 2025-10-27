using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace QuanLyBanGiay.Models
{
    public partial class QL_GiayContext : DbContext
    {
        public QL_GiayContext()
        {
        }

        public QL_GiayContext(DbContextOptions<QL_GiayContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChitietDonhang> ChitietDonhangs { get; set; } = null!;
        public virtual DbSet<ChitietGiohang> ChitietGiohangs { get; set; } = null!;
        public virtual DbSet<ChitietSanpham> ChitietSanphams { get; set; } = null!;
        public virtual DbSet<Danhmuc> Danhmucs { get; set; } = null!;
        public virtual DbSet<Donhang> Donhangs { get; set; } = null!;
        public virtual DbSet<Khachhang> Khachhangs { get; set; } = null!;
        public virtual DbSet<Mau> Maus { get; set; } = null!;
        public virtual DbSet<Phuongthucthanhtoan> Phuongthucthanhtoans { get; set; } = null!;
        public virtual DbSet<Sanpham> Sanphams { get; set; } = null!;
        public virtual DbSet<Size> Sizes { get; set; } = null!;
        public virtual DbSet<Taikhoan> Taikhoans { get; set; } = null!;
        public virtual DbSet<Thuonghieu> Thuonghieus { get; set; } = null!;
        public virtual DbSet<Voucher> Vouchers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-6OJC3FAO\\SQLEXPRESS01;Database=QL_Giay;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChitietDonhang>(entity =>
            {
                entity.HasKey(e => new { e.Madh, e.Mactsp })
                    .HasName("PK__CHITIET___2F6F1C68C2F9A9E8");

                entity.ToTable("CHITIET_DONHANG");

                entity.Property(e => e.Madh).HasColumnName("MADH");

                entity.Property(e => e.Mactsp).HasColumnName("MACTSP");

                entity.Property(e => e.Dongia)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DONGIA");

                entity.Property(e => e.Sl).HasColumnName("SL");

                entity.Property(e => e.Thanhtien)
                    .HasColumnType("decimal(29, 2)")
                    .HasColumnName("THANHTIEN")
                    .HasComputedColumnSql("([SL]*[DONGIA])", true);

                entity.HasOne(d => d.MactspNavigation)
                    .WithMany(p => p.ChitietDonhangs)
                    .HasForeignKey(d => d.Mactsp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CHITIET_D__MACTS__6B24EA82");

                entity.HasOne(d => d.MadhNavigation)
                    .WithMany(p => p.ChitietDonhangs)
                    .HasForeignKey(d => d.Madh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CHITIET_DO__MADH__6A30C649");
            });

            modelBuilder.Entity<ChitietGiohang>(entity =>
            {
                entity.HasKey(e => new { e.Makh, e.Mactsp })
                    .HasName("PK__CHITIET___2F6F4503F80F0C3A");

                entity.ToTable("CHITIET_GIOHANG");

                entity.Property(e => e.Makh).HasColumnName("MAKH");

                entity.Property(e => e.Mactsp).HasColumnName("MACTSP");

                entity.Property(e => e.Dongia)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DONGIA");

                entity.Property(e => e.Sl).HasColumnName("SL");

                entity.HasOne(d => d.MactspNavigation)
                    .WithMany(p => p.ChitietGiohangs)
                    .HasForeignKey(d => d.Mactsp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CHITIET_G__MACTS__5535A963");

                entity.HasOne(d => d.MakhNavigation)
                    .WithMany(p => p.ChitietGiohangs)
                    .HasForeignKey(d => d.Makh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CHITIET_GI__MAKH__5441852A");
            });

            modelBuilder.Entity<ChitietSanpham>(entity =>
            {
                entity.HasKey(e => e.Mactsp)
                    .HasName("PK__CHITIET___F501C2F52C51BFB9");

                entity.ToTable("CHITIET_SANPHAM");

                entity.Property(e => e.Mactsp).HasColumnName("MACTSP");

                entity.Property(e => e.Mamau).HasColumnName("MAMAU");

                entity.Property(e => e.Masize).HasColumnName("MASIZE");

                entity.Property(e => e.Masp).HasColumnName("MASP");

                entity.Property(e => e.Slton)
                    .HasColumnName("SLTON")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.MamauNavigation)
                    .WithMany(p => p.ChitietSanphams)
                    .HasForeignKey(d => d.Mamau)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CHITIET_S__MAMAU__4F7CD00D");

                entity.HasOne(d => d.MasizeNavigation)
                    .WithMany(p => p.ChitietSanphams)
                    .HasForeignKey(d => d.Masize)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CHITIET_S__MASIZ__5070F446");

                entity.HasOne(d => d.MaspNavigation)
                    .WithMany(p => p.ChitietSanphams)
                    .HasForeignKey(d => d.Masp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CHITIET_SA__MASP__4E88ABD4");
            });

            modelBuilder.Entity<Danhmuc>(entity =>
            {
                entity.HasKey(e => e.Madm)
                    .HasName("PK__DANHMUC__603F005CD2A8D17E");

                entity.ToTable("DANHMUC");

                entity.Property(e => e.Madm).HasColumnName("MADM");

                entity.Property(e => e.Mota)
                    .HasMaxLength(255)
                    .HasColumnName("MOTA");

                entity.Property(e => e.Tendm)
                    .HasMaxLength(100)
                    .HasColumnName("TENDM");
            });

            modelBuilder.Entity<Donhang>(entity =>
            {
                entity.HasKey(e => e.Madh)
                    .HasName("PK__DONHANG__603F0047B61BB957");

                entity.ToTable("DONHANG");

                entity.Property(e => e.Madh).HasColumnName("MADH");

                entity.Property(e => e.Diachigiao)
                    .HasMaxLength(255)
                    .HasColumnName("DIACHIGIAO");

                entity.Property(e => e.Lydohuy)
                    .HasMaxLength(255)
                    .HasColumnName("LYDOHUY");

                entity.Property(e => e.Makh).HasColumnName("MAKH");

                entity.Property(e => e.Mapttt).HasColumnName("MAPTTT");

                entity.Property(e => e.Mavoucher).HasColumnName("MAVOUCHER");

                entity.Property(e => e.Ngaydat)
                    .HasColumnType("datetime")
                    .HasColumnName("NGAYDAT")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Phiship)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("PHISHIP")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Sdtgiao)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("SDTGIAO");

                entity.Property(e => e.Tongtien)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TONGTIEN");

                entity.Property(e => e.Tongtiencuoi)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TONGTIENCUOI");

                entity.Property(e => e.Trangthai)
                    .HasMaxLength(20)
                    .HasColumnName("TRANGTHAI");

                entity.HasOne(d => d.MakhNavigation)
                    .WithMany(p => p.Donhangs)
                    .HasForeignKey(d => d.Makh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DONHANG__MAKH__619B8048");

                entity.HasOne(d => d.MaptttNavigation)
                    .WithMany(p => p.Donhangs)
                    .HasForeignKey(d => d.Mapttt)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DONHANG__MAPTTT__628FA481");

                entity.HasOne(d => d.MavoucherNavigation)
                    .WithMany(p => p.Donhangs)
                    .HasForeignKey(d => d.Mavoucher)
                    .HasConstraintName("FK__DONHANG__MAVOUCH__6383C8BA");
            });

            modelBuilder.Entity<Khachhang>(entity =>
            {
                entity.HasKey(e => e.Makh)
                    .HasName("PK__KHACHHAN__603F592C69DBB024");

                entity.ToTable("KHACHHANG");

                entity.Property(e => e.Makh).HasColumnName("MAKH");

                entity.Property(e => e.Diachi)
                    .HasMaxLength(255)
                    .HasColumnName("DIACHI");

                entity.Property(e => e.Matk).HasColumnName("MATK");

                entity.Property(e => e.Sdt)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("SDT");

                entity.Property(e => e.Tenkh)
                    .HasMaxLength(100)
                    .HasColumnName("TENKH");

                entity.HasOne(d => d.MatkNavigation)
                    .WithMany(p => p.Khachhangs)
                    .HasForeignKey(d => d.Matk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__KHACHHANG__MATK__3C69FB99");
            });

            modelBuilder.Entity<Mau>(entity =>
            {
                entity.HasKey(e => e.Mamau)
                    .HasName("PK__MAU__7B7346CFE13272E2");

                entity.ToTable("MAU");

                entity.Property(e => e.Mamau).HasColumnName("MAMAU");

                entity.Property(e => e.Tenmau)
                    .HasMaxLength(50)
                    .HasColumnName("TENMAU");
            });

            modelBuilder.Entity<Phuongthucthanhtoan>(entity =>
            {
                entity.HasKey(e => e.Mapttt)
                    .HasName("PK__PHUONGTH__4F6B743E7B453816");

                entity.ToTable("PHUONGTHUCTHANHTOAN");

                entity.Property(e => e.Mapttt).HasColumnName("MAPTTT");

                entity.Property(e => e.Tenphuongthuc)
                    .HasMaxLength(50)
                    .HasColumnName("TENPHUONGTHUC");
            });

            modelBuilder.Entity<Sanpham>(entity =>
            {
                entity.HasKey(e => e.Masp)
                    .HasName("PK__SANPHAM__60228A3217416833");

                entity.ToTable("SANPHAM");

                entity.Property(e => e.Masp).HasColumnName("MASP");

                entity.Property(e => e.Gia)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("GIA");

                entity.Property(e => e.Hinhdaidien)
                    .HasMaxLength(255)
                    .HasColumnName("HINHDAIDIEN");

                entity.Property(e => e.Madm).HasColumnName("MADM");

                entity.Property(e => e.Math).HasColumnName("MATH");

                entity.Property(e => e.Mota).HasColumnName("MOTA");

                entity.Property(e => e.Tensp)
                    .HasMaxLength(100)
                    .HasColumnName("TENSP");

                entity.Property(e => e.Trangthai)
                    .HasMaxLength(10)
                    .HasColumnName("TRANGTHAI");

                entity.HasOne(d => d.MadmNavigation)
                    .WithMany(p => p.Sanphams)
                    .HasForeignKey(d => d.Madm)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SANPHAM__MADM__48CFD27E");

                entity.HasOne(d => d.MathNavigation)
                    .WithMany(p => p.Sanphams)
                    .HasForeignKey(d => d.Math)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SANPHAM__MATH__49C3F6B7");
            });

            modelBuilder.Entity<Size>(entity =>
            {
                entity.HasKey(e => e.Masize)
                    .HasName("PK__SIZE__3DD4402BE3B3201A");

                entity.ToTable("SIZE");

                entity.Property(e => e.Masize).HasColumnName("MASIZE");

                entity.Property(e => e.Tensize)
                    .HasMaxLength(50)
                    .HasColumnName("TENSIZE");
            });

            modelBuilder.Entity<Taikhoan>(entity =>
            {
                entity.HasKey(e => e.Matk)
                    .HasName("PK__TAIKHOAN__602372163DA79437");

                entity.ToTable("TAIKHOAN");

                entity.HasIndex(e => e.Email, "UQ__TAIKHOAN__161CF72406A0E3D3")
                    .IsUnique();

                entity.Property(e => e.Matk).HasColumnName("MATK");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Loaitk)
                    .HasMaxLength(10)
                    .HasColumnName("LOAITK");

                entity.Property(e => e.Matkhau)
                    .HasMaxLength(100)
                    .HasColumnName("MATKHAU");

                entity.Property(e => e.Trangthai)
                    .HasColumnName("TRANGTHAI")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Thuonghieu>(entity =>
            {
                entity.HasKey(e => e.Math)
                    .HasName("PK__THUONGHI__6023721B9A26E65B");

                entity.ToTable("THUONGHIEU");

                entity.Property(e => e.Math).HasColumnName("MATH");

                entity.Property(e => e.Logo)
                    .HasMaxLength(255)
                    .HasColumnName("LOGO");

                entity.Property(e => e.Mota)
                    .HasMaxLength(255)
                    .HasColumnName("MOTA");

                entity.Property(e => e.Tenth)
                    .HasMaxLength(100)
                    .HasColumnName("TENTH");
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.HasKey(e => e.Mavoucher)
                    .HasName("PK__VOUCHER__56FC9ADEE4562DEA");

                entity.ToTable("VOUCHER");

                entity.Property(e => e.Mavoucher).HasColumnName("MAVOUCHER");

                entity.Property(e => e.Giatri)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("GIATRI");

                entity.Property(e => e.Magiamgia)
                    .HasMaxLength(50)
                    .HasColumnName("MAGIAMGIA");

                entity.Property(e => e.Ngaybd)
                    .HasColumnType("date")
                    .HasColumnName("NGAYBD");

                entity.Property(e => e.Ngaykt)
                    .HasColumnType("date")
                    .HasColumnName("NGAYKT");

                entity.Property(e => e.Tenvoucher)
                    .HasMaxLength(100)
                    .HasColumnName("TENVOUCHER");

                entity.Property(e => e.Trangthai)
                    .HasMaxLength(10)
                    .HasColumnName("TRANGTHAI");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
