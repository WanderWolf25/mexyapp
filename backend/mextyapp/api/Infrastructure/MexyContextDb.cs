
using Microsoft.EntityFrameworkCore;
using MexyApp.Models;

namespace MexyApp.Api.Domain
{
    public sealed class MexyContext : DbContext
    {
        public MexyContext(DbContextOptions<MexyContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public"); // explícito para Supabase

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("Users");
                b.HasKey(u => u.Id);

                b.Property(u => u.Username).HasMaxLength(100).IsRequired();
                b.Property(u => u.Email).HasMaxLength(256).IsRequired();
                b.HasIndex(u => u.Email).IsUnique();

                b.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();

                b.Property(u => u.Status)
                 .HasConversion<string>()
                 .HasMaxLength(20)
                 .IsRequired();
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.ToTable("UserRoles");
                b.HasKey(ur => new { ur.UserId, ur.Role });

                b.Property(ur => ur.Role)
                 .HasConversion<string>()
                 .HasMaxLength(50)
                 .IsRequired();

                b.HasOne(ur => ur.User)
                 .WithMany()
                 .HasForeignKey(ur => ur.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
