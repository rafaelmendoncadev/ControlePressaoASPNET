using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ControlePressao.Data;

namespace ControlePressao.Scripts
{
    public class SimpleMigration
    {
        public static async Task ExecuteMigrationAsync()
        {
            Console.WriteLine("=== Migração para SQLite ===");
            
            try
            {
                var connectionString = "Data Source=ControlePressao.db";
                
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connectionString)
                    .Options;

                using var context = new ApplicationDbContext(options);
                
                Console.WriteLine("Aplicando migrations...");
                await context.Database.MigrateAsync();
                
                Console.WriteLine("Executando seed data...");
                await SeedData.InitializeAsync(context);
                
                Console.WriteLine("Migração concluída com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro durante a migração: {ex.Message}");
                throw;
            }
        }
    }
}