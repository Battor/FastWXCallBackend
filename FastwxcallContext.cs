using System;
using FastWXCallBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace FastWXCallBackend
{
    public partial class FastwxcallContext : DbContext
    {
        //public FastwxcallContext()
        //{
        //}

        public FastwxcallContext(DbContextOptions<FastwxcallContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                //optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=fastwxcall;uid=root;pwd=dev@123", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.29-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");

                entity.Property(e => e.Id).HasMaxLength(100);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Enable)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.HeadImgId).HasColumnType("tinyint(4)");

                entity.Property(e => e.PhotoImagePath)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Wxname)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("WXName");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).HasMaxLength(100);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Enable)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.LastSignInTime).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SecretKey)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
