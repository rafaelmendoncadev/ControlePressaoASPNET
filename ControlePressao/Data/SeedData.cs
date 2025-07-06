using ControlePressao.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ControlePressao.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            // Garantir que o banco seja criado
            await context.Database.EnsureCreatedAsync();

            // Verificar se já existem usuários
            if (await context.Users.AnyAsync())
            {
                return; // BD já foi populado
            }

            // Criar usuário administrador
            var adminUser = new User
            {
                Nome = "Administrador",
                Email = "admin@admin.com",
                Senha = HashPassword("admin123"),
                DataCadastro = DateTime.Now,
                Ativo = true
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            // Criar alguns dados de exemplo para o admin
            var pressoesSample = new List<Pressao>
            {
                new Pressao
                {
                    UserId = adminUser.Id,
                    DataHora = DateTime.Now.AddDays(-5),
                    Sistolica = 120,
                    Diastolica = 80,
                    FrequenciaCardiaca = 72,
                    Observacoes = "Medição de rotina"
                },
                new Pressao
                {
                    UserId = adminUser.Id,
                    DataHora = DateTime.Now.AddDays(-3),
                    Sistolica = 125,
                    Diastolica = 85,
                    FrequenciaCardiaca = 75,
                    Observacoes = "Após exercício leve"
                },
                new Pressao
                {
                    UserId = adminUser.Id,
                    DataHora = DateTime.Now.AddDays(-1),
                    Sistolica = 118,
                    Diastolica = 78,
                    FrequenciaCardiaca = 68,
                    Observacoes = "Em jejum"
                }
            };

            var glicosesSample = new List<Glicose>
            {
                new Glicose
                {
                    UserId = adminUser.Id,
                    DataHora = DateTime.Now.AddDays(-4),
                    Valor = 95,
                    Periodo = PeriodoTeste.Jejum,
                    Observacoes = "Jejum de 12 horas"
                },
                new Glicose
                {
                    UserId = adminUser.Id,
                    DataHora = DateTime.Now.AddDays(-2),
                    Valor = 140,
                    Periodo = PeriodoTeste.PosRefeicao,
                    Observacoes = "2 horas após almoço"
                },
                new Glicose
                {
                    UserId = adminUser.Id,
                    DataHora = DateTime.Now.AddHours(-6),
                    Valor = 88,
                    Periodo = PeriodoTeste.Casual,
                    Observacoes = "Medição casual"
                }
            };

            context.Pressao.AddRange(pressoesSample);
            context.Glicose.AddRange(glicosesSample);
            await context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
