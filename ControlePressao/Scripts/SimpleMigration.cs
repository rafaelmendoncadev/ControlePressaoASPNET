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
            Console.WriteLine("=== Migração para SQL Server ===");
            
            try
            {
                var connectionString = "Server=rafaelmendoncadev.database.windows.net,1433;Initial Catalog=ControlePressao;Persist Security Info=False;User ID=CloudSAc2b09158;Password=#Jlm312317;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(connectionString)
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