using ControlePressao.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlePressao.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public required DbSet<Pressao> Pressao { get; set; }
        public required DbSet<Glicose> Glicose { get; set; }
        public required DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurações de relacionamento
            builder.Entity<Pressao>()
                .HasOne(p => p.User)
                .WithMany(u => u.Pressoes)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Glicose>()
                .HasOne(g => g.User)
                .WithMany(u => u.Glicoses)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurações de índices
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
