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

            // Configurações específicas das entidades (removidas as configurações de relacionamento com Usuario)
        }
    }
}
