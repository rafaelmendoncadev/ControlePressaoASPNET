
using ControlePressao.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlePressao.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Pressao> Pressao { get; set; }
        public DbSet<Glicose> Glicose { get; set; }
    }
}
