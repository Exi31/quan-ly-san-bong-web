using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QLSanBongWeb.Models;

namespace QLSanBongWeb.Data;

public partial class QLSanBongContext : DbContext
{
    public QLSanBongContext(DbContextOptions<QLSanBongContext> options)
        : base(options)
    {
    }

    public virtual DbSet<tblDatSan> tblDatSan { get; set; }

    public virtual DbSet<tblHoaDon> tblHoaDon { get; set; }

    public virtual DbSet<tblKhachHang> tblKhachHang { get; set; }

    public virtual DbSet<tblSan> tblSan { get; set; }

    public virtual DbSet<tblTaiKhoan> tblTaiKhoan { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<tblDatSan>(entity =>
        {
            entity.HasKey(e => e.MaDatSan);

            entity.ToTable("tblDatSan");

            entity.Property(e => e.MaDatSan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DonGiaApDung).HasColumnType("money");
            entity.Property(e => e.GioBatDau).HasPrecision(0);
            entity.Property(e => e.GioKetThuc).HasPrecision(0);
            entity.Property(e => e.MaKH)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaSan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TrangThaiDat).HasMaxLength(50);

            entity.HasOne(d => d.MaKHNavigation).WithMany(p => p.tblDatSans)
                .HasForeignKey(d => d.MaKH)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblDatSan_tblKhachHang");

            entity.HasOne(d => d.MaSanNavigation).WithMany(p => p.tblDatSans)
                .HasForeignKey(d => d.MaSan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblDatSan_tblSan");
        });

        modelBuilder.Entity<tblHoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHD);

            entity.ToTable("tblHoaDon");

            entity.HasIndex(e => e.MaDatSan, "uq_tblHoaDon_MaDatSan").IsUnique();

            entity.Property(e => e.MaHD)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaDatSan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgayLap).HasPrecision(0);
            entity.Property(e => e.ThoiDiemThanhToan).HasPrecision(0);
            entity.Property(e => e.TongTien).HasColumnType("money");

            entity.HasOne(d => d.MaDatSanNavigation).WithOne(p => p.tblHoaDon)
                .HasForeignKey<tblHoaDon>(d => d.MaDatSan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblHoaDon_tblDatSan");
        });

        modelBuilder.Entity<tblKhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKH);

            entity.ToTable("tblKhachHang");

            entity.Property(e => e.MaKH)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SoDT)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenKH).HasMaxLength(50);
        });

        modelBuilder.Entity<tblSan>(entity =>
        {
            entity.HasKey(e => e.MaSan);

            entity.ToTable("tblSan");

            entity.Property(e => e.MaSan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GiaThueGio).HasColumnType("money");
            entity.Property(e => e.LoaiSan).HasMaxLength(50);
            entity.Property(e => e.TenSan).HasMaxLength(50);
        });

        modelBuilder.Entity<tblTaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTK);

            entity.ToTable("tblTaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "uq_tblTaiKhoan_TenDangNhap").IsUnique();

            entity.Property(e => e.MaTK)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MatKhauHash)
                .HasMaxLength(512)
                .IsUnicode(false);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.VaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
