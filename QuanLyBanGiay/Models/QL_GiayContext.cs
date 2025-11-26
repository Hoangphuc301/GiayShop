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
        public virtual DbSet<ChitietSanpham> ChitietSanphams { get; set; } = null!;
        public virtual DbSet<Danhmuc> Danhmucs { get; set; } = null!;
        public virtual DbSet<Donhang> Donhangs { get; set; } = null!;
        public virtual DbSet<Khachhang> Khachhangs { get; set; } = null!;
        public virtual DbSet<Mau> Maus { get; set; } = null!;
        public virtual DbSet<Phuongthucthanhtoan> Phuongthucthanhtoans { get; set; } = null!;
        public virtual DbSet<Sanpham> Sanphams { get; set; } = null!;
        public virtual DbSet<Size> Sizes { get; set; } = null!;
        public virtual DbSet<Thuonghieu> Thuonghieus { get; set; } = null!;
        public virtual DbSet<Voucher> Vouchers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=LAPTOP-6OJC3FAO;Initial Catalog=QL_Giay;Integrated Security=True;Encrypt=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChitietDonhang>(entity =>
            {
                entity.HasKey(e => new { e.Madh, e.Mactsp })
                    .HasName("PK__CHITIET___2F6F1C688CD988B2");

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

            modelBuilder.Entity<ChitietSanpham>(entity =>
            {
                entity.HasKey(e => e.Mactsp)
                    .HasName("PK__CHITIET___F501C2F57E7AE6CC");

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
                    .HasConstraintName("FK__CHITIET_SA__MASP__4E88ABD4");
            });

            modelBuilder.Entity<Danhmuc>(entity =>
            {
                entity.HasKey(e => e.Madm)
                    .HasName("PK__DANHMUC__603F005C468FB43D");

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
                    .HasName("PK__DONHANG__603F004776DE4EE0");

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
                    .HasConstraintName("FK_DONHANG_KH");

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
                    .HasName("PK__KHACHHAN__603F592CE7DCE3EC");

                entity.ToTable("KHACHHANG");

                entity.HasIndex(e => e.Email, "UQ__KHACHHAN__161CF724C8915B15")
                    .IsUnique();

                entity.Property(e => e.Makh).HasColumnName("MAKH");

                entity.Property(e => e.DaXacNhan).HasDefaultValueSql("((0))");

                entity.Property(e => e.Diachi)
                    .HasMaxLength(255)
                    .HasColumnName("DIACHI");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Loaitk)
                    .HasMaxLength(10)
                    .HasColumnName("LOAITK");

                entity.Property(e => e.MaXacNhan).HasMaxLength(10);

                entity.Property(e => e.Matkhau)
                    .HasMaxLength(100)
                    .HasColumnName("MATKHAU");

                entity.Property(e => e.Sdt)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("SDT");

                entity.Property(e => e.Tenkh)
                    .HasMaxLength(100)
                    .HasColumnName("TENKH");

                entity.Property(e => e.ThoiGianTaoOtp)
                    .HasColumnType("datetime")
                    .HasColumnName("ThoiGianTaoOTP");

                entity.Property(e => e.Trangthai)
                    .HasColumnName("TRANGTHAI")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Mau>(entity =>
            {
                entity.HasKey(e => e.Mamau)
                    .HasName("PK__MAU__7B7346CF1DCCFB99");

                entity.ToTable("MAU");

                entity.Property(e => e.Mamau).HasColumnName("MAMAU");

                entity.Property(e => e.Tenmau)
                    .HasMaxLength(50)
                    .HasColumnName("TENMAU");
            });

            modelBuilder.Entity<Phuongthucthanhtoan>(entity =>
            {
                entity.HasKey(e => e.Mapttt)
                    .HasName("PK__PHUONGTH__4F6B743E047617E3");

                entity.ToTable("PHUONGTHUCTHANHTOAN");

                entity.Property(e => e.Mapttt).HasColumnName("MAPTTT");

                entity.Property(e => e.Tenphuongthuc)
                    .HasMaxLength(50)
                    .HasColumnName("TENPHUONGTHUC");
            });

            modelBuilder.Entity<Sanpham>(entity =>
            {
                entity.HasKey(e => e.Masp)
                    .HasName("PK__SANPHAM__60228A3203A8CDD7");

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
                    .HasName("PK__SIZE__3DD4402B8C8AB7DF");

                entity.ToTable("SIZE");

                entity.Property(e => e.Masize).HasColumnName("MASIZE");

                entity.Property(e => e.Tensize)
                    .HasMaxLength(50)
                    .HasColumnName("TENSIZE");
            });

            modelBuilder.Entity<Thuonghieu>(entity =>
            {
                entity.HasKey(e => e.Math)
                    .HasName("PK__THUONGHI__6023721B5000B73E");

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
                    .HasName("PK__VOUCHER__56FC9ADEC3E8C892");

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
