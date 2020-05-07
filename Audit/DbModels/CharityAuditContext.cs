using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AuditProvider.DbModels
{
    public partial class CharityAuditContext : DbContext
    {
        public CharityAuditContext()
        {
        }

        public CharityAuditContext(DbContextOptions<CharityAuditContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccessLog> AccessLog { get; set; }
        public virtual DbSet<EntityAuditDetail> EntityAuditDetail { get; set; }
        public virtual DbSet<EntityAuditLog> EntityAuditLog { get; set; }
        public virtual DbSet<ExceptionLog> ExceptionLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost;Database=CharityAudit;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccessLog>(entity =>
            {
                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("URL")
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EntityAuditDetail>(entity =>
            {
                entity.Property(e => e.ColumnName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NewValue).IsRequired();

                entity.Property(e => e.OldValue).IsRequired();

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EntityAuditLog>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Pk)
                    .HasColumnName("PK")
                    .HasMaxLength(128);

                entity.Property(e => e.SerializedModel).HasColumnType("text");

                entity.Property(e => e.TableName).HasMaxLength(100);
            });

            modelBuilder.Entity<ExceptionLog>(entity =>
            {
                entity.Property(e => e.Exception).HasColumnType("text");

                entity.Property(e => e.FilePath).HasMaxLength(1000);

                entity.Property(e => e.MethodName).HasMaxLength(250);

                entity.Property(e => e.SerializedModel).HasColumnType("text");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
