using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Models;

namespace QuanLySanBong.Data 
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<San> Sans { get; set; } = null!;
        public DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public DbSet<DatSan> DatSans { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<San>().Property(s => s.MaSan).IsUnicode(false);
            modelBuilder.Entity<KhachHang>().Property(k => k.MaKH).IsUnicode(false);
            modelBuilder.Entity<KhachHang>().Property(k => k.SoDT).IsUnicode(false);
            modelBuilder.Entity<DatSan>().Property(d => d.MaDatSan).IsUnicode(false);
            modelBuilder.Entity<DatSan>().Property(d => d.MaSan).IsUnicode(false);
            modelBuilder.Entity<DatSan>().Property(d => d.MaKH).IsUnicode(false);
            modelBuilder.Entity<DatSan>().Property(d => d.GioBatDau).HasPrecision(0);
            modelBuilder.Entity<DatSan>().Property(d => d.GioKetThuc).HasPrecision(0);
            modelBuilder.Entity<San>().ToTable("tblSan");
            modelBuilder.Entity<KhachHang>().ToTable("tblKhachHang");
            modelBuilder.Entity<DatSan>().ToTable("tblDatSan");
        }
    }
}