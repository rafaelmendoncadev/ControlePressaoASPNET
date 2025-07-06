using ControlePressao.Data;
using ControlePressao.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlePressao.Services
{
    public interface ISaudeService
    {
        Task<DashboardViewModel> ObterDadosDashboardAsync(int userId);
        Task<List<AlertaSaude>> ObterAlertasAsync(int userId);
        Task<EstatisticasSaude> ObterEstatisticasAsync(int userId);
    }

    public class SaudeService : ISaudeService
    {
        private readonly ApplicationDbContext _context;

        public SaudeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> ObterDadosDashboardAsync(int userId)
        {
            var ultimasPressoes = await _context.Pressao
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DataHora)
                .Take(10)
                .ToListAsync();

            var ultimasGlicoses = await _context.Glicose
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.DataHora)
                .Take(10)
                .ToListAsync();

            var estatisticas = await ObterEstatisticasAsync(userId);
            var alertas = await ObterAlertasAsync(userId);

            return new DashboardViewModel
            {
                UltimasPressoes = ultimasPressoes,
                UltimasGlicoses = ultimasGlicoses,
                Estatisticas = estatisticas,
                Alertas = alertas
            };
        }

        public async Task<List<AlertaSaude>> ObterAlertasAsync(int userId)
        {
            var alertas = new List<AlertaSaude>();

            // Verificar pressão alta recente
            var pressaoRecente = await _context.Pressao
                .Where(p => p.UserId == userId && p.DataHora >= DateTime.Now.AddDays(-7))
                .OrderByDescending(p => p.DataHora)
                .FirstOrDefaultAsync();

            if (pressaoRecente != null)
            {
                if (pressaoRecente.Sistolica >= 140 || pressaoRecente.Diastolica >= 90)
                {
                    alertas.Add(new AlertaSaude
                    {
                        Tipo = "Pressão",
                        Mensagem = $"Pressão alta detectada: {pressaoRecente.Sistolica}/{pressaoRecente.Diastolica} mmHg",
                        Classe = "alert-danger",
                        DataHora = pressaoRecente.DataHora
                    });
                }
            }

            // Verificar glicose alta recente
            var glicoseRecente = await _context.Glicose
                .Where(g => g.UserId == userId && g.DataHora >= DateTime.Now.AddDays(-7))
                .OrderByDescending(g => g.DataHora)
                .FirstOrDefaultAsync();

            if (glicoseRecente != null)
            {
                if (glicoseRecente.ClassificacaoGlicose == "Diabetes" || glicoseRecente.ClassificacaoGlicose == "Suspeita de Diabetes")
                {
                    alertas.Add(new AlertaSaude
                    {
                        Tipo = "Glicose",
                        Mensagem = $"Glicose alterada: {glicoseRecente.Valor} mg/dL ({glicoseRecente.ClassificacaoGlicose})",
                        Classe = "alert-warning",
                        DataHora = glicoseRecente.DataHora
                    });
                }
                else if (glicoseRecente.ClassificacaoGlicose == "Hipoglicemia")
                {
                    alertas.Add(new AlertaSaude
                    {
                        Tipo = "Glicose",
                        Mensagem = $"Hipoglicemia detectada: {glicoseRecente.Valor} mg/dL",
                        Classe = "alert-danger",
                        DataHora = glicoseRecente.DataHora
                    });
                }
            }

            // Verificar se não há medições recentes
            var ultimaMedicao = await ObterUltimaMedicaoAsync(userId);
            if (ultimaMedicao.HasValue && ultimaMedicao.Value <= DateTime.Now.AddDays(-7))
            {
                alertas.Add(new AlertaSaude
                {
                    Tipo = "Lembrete",
                    Mensagem = "Há mais de 7 dias sem medições. Considere fazer novas medições.",
                    Classe = "alert-info",
                    DataHora = DateTime.Now
                });
            }

            return alertas.OrderByDescending(a => a.DataHora).ToList();
        }

        public async Task<EstatisticasSaude> ObterEstatisticasAsync(int userId)
        {
            var pressoes = await _context.Pressao
                .Where(p => p.UserId == userId)
                .ToListAsync();
            var glicoses = await _context.Glicose
                .Where(g => g.UserId == userId)
                .ToListAsync();

            var estatisticas = new EstatisticasSaude
            {
                TotalMedicoesPressao = pressoes.Count,
                TotalMedicoesGlicose = glicoses.Count
            };

            if (pressoes.Any())
            {
                estatisticas.PressaoSistolicaMedia = pressoes.Average(p => p.Sistolica);
                estatisticas.PressaoDiastolicaMedia = pressoes.Average(p => p.Diastolica);
                estatisticas.FrequenciaCardiacaMedia = pressoes.Average(p => p.FrequenciaCardiaca);
            }

            if (glicoses.Any())
            {
                estatisticas.GlicoseMedia = glicoses.Average(g => g.Valor);
            }

            estatisticas.UltimaMedicao = await ObterUltimaMedicaoAsync(userId);

            return estatisticas;
        }

        private async Task<DateTime?> ObterUltimaMedicaoAsync(int userId)
        {
            var ultimaPressao = await _context.Pressao
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DataHora)
                .Select(p => p.DataHora)
                .FirstOrDefaultAsync();

            var ultimaGlicose = await _context.Glicose
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.DataHora)
                .Select(g => g.DataHora)
                .FirstOrDefaultAsync();

            if (ultimaPressao == default && ultimaGlicose == default)
                return null;

            if (ultimaPressao == default)
                return ultimaGlicose;

            if (ultimaGlicose == default)
                return ultimaPressao;

            return ultimaPressao > ultimaGlicose ? ultimaPressao : ultimaGlicose;
        }
    }
}
