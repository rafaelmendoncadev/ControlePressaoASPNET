using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ControlePressao.Data;
using ControlePressao.Models;
using ControlePressao.Services;
using System.Security.Claims;

namespace ControlePressao.Controllers
{
    [Authorize]
    public class RelatoriosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AnaliseService _analiseService;
        private readonly PdfService _pdfService;
        private readonly ExcelService _excelService;

        public RelatoriosController(ApplicationDbContext context, AnaliseService analiseService, PdfService pdfService, ExcelService excelService)
        {
            _context = context;
            _analiseService = analiseService;
            _pdfService = pdfService;
            _excelService = excelService;
        }

        // GET: Relatorios
        public IActionResult Index()
        {
            var model = new RelatorioFilterViewModel();
            return View(model);
        }

        // POST: Relatorios/Gerar
        [HttpPost]
        public async Task<IActionResult> Gerar(RelatorioFilterViewModel filtros)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", filtros);
            }

            try
            {
                var userId = GetCurrentUserId();
                var usuario = await _context.Users.FindAsync(userId);
                
                if (usuario == null)
                {
                    return NotFound("Usuário não encontrado.");
                }

                var dataInicial = filtros.DataInicial ?? DateTime.Today.AddDays(-30);
                var dataFinal = filtros.DataFinal ?? DateTime.Today;

                // Validar datas
                if (dataInicial > dataFinal)
                {
                    ModelState.AddModelError("", "A data inicial não pode ser maior que a data final.");
                    return View("Index", filtros);
                }

                if ((dataFinal - dataInicial).Days > 365)
                {
                    ModelState.AddModelError("", "O período do relatório não pode ser maior que 1 ano.");
                    return View("Index", filtros);
                }

                // Buscar dados conforme o tipo de relatório
                var pressoes = new List<Pressao>();
                var glicoses = new List<Glicose>();
                var pesos = new List<Peso>();

                if (filtros.TipoRelatorio == TipoRelatorio.Pressao || filtros.TipoRelatorio == TipoRelatorio.Consolidado)
                {
                    pressoes = await _context.Pressao
                        .Where(p => p.UserId == userId && p.DataHora >= dataInicial && p.DataHora <= dataFinal.AddDays(1))
                        .OrderByDescending(p => p.DataHora)
                        .ToListAsync();
                }

                if (filtros.TipoRelatorio == TipoRelatorio.Glicose || filtros.TipoRelatorio == TipoRelatorio.Consolidado)
                {
                    glicoses = await _context.Glicose
                        .Where(g => g.UserId == userId && g.DataHora >= dataInicial && g.DataHora <= dataFinal.AddDays(1))
                        .OrderByDescending(g => g.DataHora)
                        .ToListAsync();
                }

                // Buscar dados de peso para relatórios de peso ou consolidados
                if (filtros.TipoRelatorio == TipoRelatorio.Peso || filtros.TipoRelatorio == TipoRelatorio.Consolidado)
                {
                    pesos = await _context.Peso
                        .Where(p => p.UserId == userId && p.DataHora >= dataInicial && p.DataHora <= dataFinal.AddDays(1))
                        .OrderByDescending(p => p.DataHora)
                        .ToListAsync();
                }

                // Verificar se há dados
                if (!pressoes.Any() && !glicoses.Any() && !pesos.Any())
                {
                    ModelState.AddModelError("", "Não foram encontrados dados para o período selecionado.");
                    return View("Index", filtros);
                }

                // Gerar análises
                var analise = _analiseService.AnalisarDados(pressoes, glicoses, pesos, dataInicial, dataFinal);
                
                // Criar modelo do relatório
                var relatorio = new RelatorioResultViewModel
                {
                    DataInicial = dataInicial,
                    DataFinal = dataFinal,
                    TipoRelatorio = filtros.TipoRelatorio,
                    Usuario = usuario,
                    Pressoes = pressoes,
                    Glicoses = glicoses,
                    Pesos = pesos,
                    Analise = analise,
                    TabelasReferencia = new TabelasReferenciaViewModel(),
                    ObservacoesMedicas = _analiseService.GerarObservacoesMedicasPersonalizadas(analise, dataInicial, dataFinal)
                };

                // Exportar conforme formato selecionado
                return filtros.FormatoExportacao switch
                {
                    FormatoExportacao.PDF => ExportarPdf(relatorio),
                    FormatoExportacao.Excel => ExportarExcel(relatorio),
                    _ => BadRequest("Formato de exportação inválido.")
                };
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao gerar relatório: {ex.Message}");
                return View("Index", filtros);
            }
        }

        // GET: Relatorios/GerarRapido
        public async Task<IActionResult> GerarRapido(TipoRelatorio tipo = TipoRelatorio.Consolidado, FormatoExportacao formato = FormatoExportacao.PDF)
        {
            var filtros = new RelatorioFilterViewModel
            {
                TipoRelatorio = tipo,
                FormatoExportacao = formato,
                DataInicial = DateTime.Today.AddDays(-30),
                DataFinal = DateTime.Today
            };

            return await Gerar(filtros);
        }

        private IActionResult ExportarPdf(RelatorioResultViewModel relatorio)
        {
            try
            {
                // Log para debug
                Console.WriteLine($"Iniciando geração de PDF para usuário: {relatorio.Usuario?.Nome}");
                Console.WriteLine($"Período: {relatorio.DataInicial:dd/MM/yyyy} a {relatorio.DataFinal:dd/MM/yyyy}");
                Console.WriteLine($"Pressões: {relatorio.Pressoes?.Count ?? 0}, Glicoses: {relatorio.Glicoses?.Count ?? 0}");
                
                var pdfBytes = _pdfService.GerarRelatorioPdf(relatorio);
                
                Console.WriteLine($"PDF gerado com sucesso. Tamanho: {pdfBytes?.Length ?? 0} bytes");
                
                var nomeArquivo = GerarNomeArquivo(relatorio, "pdf");
                
                return File(pdfBytes ?? Array.Empty<byte>(), "application/pdf", nomeArquivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar PDF: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"Erro ao gerar PDF: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        private IActionResult ExportarExcel(RelatorioResultViewModel relatorio)
        {
            try
            {
                var excelBytes = _excelService.GerarRelatorioExcel(relatorio);
                var nomeArquivo = GerarNomeArquivo(relatorio, "xlsx");
                
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nomeArquivo);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao gerar Excel: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        private string GerarNomeArquivo(RelatorioResultViewModel relatorio, string extensao)
        {
            var tipoRelatorio = relatorio.TipoRelatorio switch
            {
                TipoRelatorio.Pressao => "Pressao",
                TipoRelatorio.Glicose => "Glicose",
                TipoRelatorio.Peso => "Peso",
                TipoRelatorio.Consolidado => "Consolidado",
                _ => "Relatorio"
            };

            var periodo = $"{relatorio.DataInicial:yyyyMMdd}-{relatorio.DataFinal:yyyyMMdd}";
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            
            return $"Relatorio_{tipoRelatorio}_{periodo}_{timestamp}.{extensao}";
        }

        // GET: Relatorios/Historico
        public async Task<IActionResult> Historico()
        {
            var userId = GetCurrentUserId();
            
            // Buscar últimas medições para exibir resumo
            var ultimasPressoes = await _context.Pressao
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DataHora)
                .Take(5)
                .ToListAsync();

            var ultimasGlicoses = await _context.Glicose
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.DataHora)
                .Take(5)
                .ToListAsync();

            var ultimosPesos = await _context.Peso
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DataHora)
                .Take(5)
                .ToListAsync();

            var model = new HistoricoViewModel
            {
                UltimasPressoes = ultimasPressoes,
                UltimasGlicoses = ultimasGlicoses,
                UltimosPesos = ultimosPesos,
                TotalPressoes = await _context.Pressao.CountAsync(p => p.UserId == userId),
                TotalGlicoses = await _context.Glicose.CountAsync(g => g.UserId == userId),
                TotalPesos = await _context.Peso.CountAsync(p => p.UserId == userId)
            };

            return View(model);
        }

        // GET: Relatorios/Visualizar
        public async Task<IActionResult> Visualizar(RelatorioFilterViewModel filtros)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", filtros);
            }

            var userId = GetCurrentUserId();
            var usuario = await _context.Users.FindAsync(userId);
            
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var dataInicial = filtros.DataInicial ?? DateTime.Today.AddDays(-30);
            var dataFinal = filtros.DataFinal ?? DateTime.Today;

            // Buscar dados
            var pressoes = new List<Pressao>();
            var glicoses = new List<Glicose>();
            var pesos = new List<Peso>();

            if (filtros.TipoRelatorio == TipoRelatorio.Pressao || filtros.TipoRelatorio == TipoRelatorio.Consolidado)
            {
                pressoes = await _context.Pressao
                    .Where(p => p.UserId == userId && p.DataHora >= dataInicial && p.DataHora <= dataFinal.AddDays(1))
                    .OrderByDescending(p => p.DataHora)
                    .ToListAsync();
            }

            if (filtros.TipoRelatorio == TipoRelatorio.Glicose || filtros.TipoRelatorio == TipoRelatorio.Consolidado)
            {
                glicoses = await _context.Glicose
                    .Where(g => g.UserId == userId && g.DataHora >= dataInicial && g.DataHora <= dataFinal.AddDays(1))
                    .OrderByDescending(g => g.DataHora)
                    .ToListAsync();
            }

            if (filtros.TipoRelatorio == TipoRelatorio.Peso || filtros.TipoRelatorio == TipoRelatorio.Consolidado)
            {
                pesos = await _context.Peso
                    .Where(p => p.UserId == userId && p.DataHora >= dataInicial && p.DataHora <= dataFinal.AddDays(1))
                    .OrderByDescending(p => p.DataHora)
                    .ToListAsync();
            }

            if (!pressoes.Any() && !glicoses.Any() && !pesos.Any())
            {
                TempData["InfoMessage"] = "Não foram encontrados dados para o período selecionado.";
                return RedirectToAction("Index");
            }

            // Gerar análises
            var analise = _analiseService.AnalisarDados(pressoes, glicoses, pesos, dataInicial, dataFinal);
            
            var relatorio = new RelatorioResultViewModel
            {
                DataInicial = dataInicial,
                DataFinal = dataFinal,
                TipoRelatorio = filtros.TipoRelatorio,
                Usuario = usuario,
                Pressoes = pressoes,
                Glicoses = glicoses,
                Pesos = pesos,
                Analise = analise,
                TabelasReferencia = new TabelasReferenciaViewModel(),
                ObservacoesMedicas = _analiseService.GerarObservacoesMedicasPersonalizadas(analise, dataInicial, dataFinal)
            };

            return View(relatorio);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        // Método de teste para PDF
        [HttpGet]
        [AllowAnonymous]
        public IActionResult TestePdf()
        {
            try
            {
                // Criar usuário fictício para teste
                var usuario = new User
                {
                    Id = 1,
                    Nome = "Usuário Teste",
                    Email = "teste@teste.com",
                    Senha = "123456", // Campo obrigatório
                    DataNascimento = DateTime.Today.AddYears(-30)
                };

                // Criar dados de teste
                var relatorio = new RelatorioResultViewModel
                {
                    DataInicial = DateTime.Today.AddDays(-7),
                    DataFinal = DateTime.Today,
                    TipoRelatorio = TipoRelatorio.Consolidado,
                    Usuario = usuario,
                    Pressoes = new List<Pressao>
                    {
                        new Pressao { DataHora = DateTime.Now, Sistolica = 120, Diastolica = 80, FrequenciaCardiaca = 70, Observacoes = "Teste" }
                    },
                    Glicoses = new List<Glicose>
                    {
                        new Glicose { DataHora = DateTime.Now, Valor = 90, Periodo = PeriodoTeste.Jejum, Observacoes = "Teste" }
                    },
                    Pesos = new List<Peso>
                    {
                        new Peso { DataHora = DateTime.Now, PesoKg = 70, Altura = 175, Observacoes = "Teste" }
                    },
                    Analise = new AnaliseRelatorioViewModel
                    {
                        AnalisesPressao = new AnalisesPressaoViewModel
                        {
                            TotalMedicoes = 1,
                            MediaSistolica = 120,
                            MediaDiastolica = 80,
                            MediaFrequenciaCardiaca = 70,
                            ClassificacaoPredominate = "Normal",
                            PercentualHipertensao = 0,
                            TendenciaGeral = "Estável"
                        },
                        AnalisesGlicose = new AnalisesGlicoseViewModel
                        {
                            TotalMedicoes = 1,
                            MediaJejum = 90,
                            MediaPosRefeicao = 0,
                            ClassificacaoPredominate = "Normal",
                            PercentualAlteradas = 0,
                            ControleGlicemico = "Bom"
                        },
                        AnalisesPeso = new AnalisesPesoViewModel
                        {
                            TotalMedicoes = 1,
                            MediaPeso = 70,
                            MediaAltura = 175,
                            MediaIMC = 22.9,
                            PesoIdealMedio = 68.8,
                            ClassificacaoIMCPredominante = "Peso normal",
                            PercentualPesoNormal = 100,
                            StatusPesoGeral = "Peso adequado",
                            TendenciaPeso = "Estável"
                        },
                        AlertasCriticos = new List<string>(),
                        RecomendacoesGerais = "Manter hábitos saudáveis e monitoramento regular"
                    },
                    TabelasReferencia = new TabelasReferenciaViewModel(),
                    ObservacoesMedicas = "Teste de geração de PDF - Dados fictícios para validação do sistema"
                };

                return ExportarPdf(relatorio);
            }
            catch (Exception ex)
            {
                return Content($"Erro no teste de PDF: {ex.Message}\n\nStack trace: {ex.StackTrace}");
            }
        }
    }
}

// ViewModel adicional para histórico
public class HistoricoViewModel
{
    public List<Pressao> UltimasPressoes { get; set; } = new();
    public List<Glicose> UltimasGlicoses { get; set; } = new();
    public List<Peso> UltimosPesos { get; set; } = new();
    public int TotalPressoes { get; set; }
    public int TotalGlicoses { get; set; }
    public int TotalPesos { get; set; }
}