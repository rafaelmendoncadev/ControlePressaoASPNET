using ControlePressao.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System.Globalization;

namespace ControlePressao.Services
{
    public class PdfService
    {
        private readonly PdfFont _titleFont;
        private readonly PdfFont _subtitleFont;
        private readonly PdfFont _normalFont;
        private readonly PdfFont _boldFont;
        private readonly PdfFont _smallFont;

        public PdfService()
        {
            // Configuração das fontes
            _titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            _subtitleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            _normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            _boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            _smallFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        }

        public byte[] GerarRelatorioPdf(RelatorioResultViewModel relatorio)
        {
            try
            {
                Console.WriteLine("Iniciando geração do PDF...");
                
                using (var memoryStream = new MemoryStream())
                {
                    Console.WriteLine("MemoryStream criado");
                    
                    // Configuração do documento A4
                    var writer = new PdfWriter(memoryStream);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);
                    
                    Console.WriteLine("Documento PDF criado");

                    // Cabeçalho
                    AdicionarCabecalho(document, relatorio);
                    
                    // Dados do paciente
                    AdicionarDadosPaciente(document, relatorio);
                    
                    // Período do relatório
                    AdicionarPeriodoRelatorio(document, relatorio);

                    // Tabelas de referência
                    AdicionarTabelasReferencia(document, relatorio.TabelasReferencia);

                    // Análises
                    if (relatorio.Analise.AnalisesPressao != null)
                    {
                        AdicionarAnalisesPressao(document, relatorio.Analise.AnalisesPressao, relatorio.Pressoes);
                    }

                    if (relatorio.Analise.AnalisesGlicose != null)
                    {
                        AdicionarAnalisesGlicose(document, relatorio.Analise.AnalisesGlicose, relatorio.Glicoses);
                    }

                    if (relatorio.Analise.AnalisesPeso != null)
                    {
                        AdicionarAnalisesPeso(document, relatorio.Analise.AnalisesPeso, relatorio.Pesos);
                    }

                    // Observações médicas
                    AdicionarObservacoesMedicas(document, relatorio.ObservacoesMedicas);

                    // Alertas críticos
                    if (relatorio.Analise.AlertasCriticos.Any())
                    {
                        AdicionarAlertasCriticos(document, relatorio.Analise.AlertasCriticos);
                    }

                    // Recomendações
                    AdicionarRecomendacoes(document, relatorio.Analise.RecomendacoesGerais);

                    // Disclaimer médico
                    AdicionarDisclaimerMedico(document, relatorio.DisclaimerMedico);

                    // Rodapé
                    AdicionarRodape(document);
                    Console.WriteLine("Rodapé adicionado");

                    document.Close();
                    Console.WriteLine("Documento fechado");
                    
                    var pdfBytes = memoryStream.ToArray();
                    Console.WriteLine($"PDF convertido para bytes. Tamanho: {pdfBytes.Length}");
                    
                    return pdfBytes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no PdfService: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private void AdicionarCabecalho(Document document, RelatorioResultViewModel relatorio)
        {
            var table = new Table(2).UseAllAvailableWidth();
            table.SetMarginBottom(20);

            // Logo/Título
            var cellTitulo = new Cell()
                .Add(new Paragraph("RELATÓRIO DE SAÚDE\nCONTROLE DE PRESSÃO E GLICOSE")
                    .SetFont(_titleFont)
                    .SetFontSize(18))
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetPadding(10);

            // Data de geração
            var cellData = new Cell()
                .Add(new Paragraph($"Gerado em:\n{DateTime.Now:dd/MM/yyyy HH:mm}")
                    .SetFont(_normalFont)
                    .SetFontSize(10))
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetPadding(10);

            table.AddCell(cellTitulo);
            table.AddCell(cellData);

            document.Add(table);
            document.Add(new Paragraph(" "));
        }

        private void AdicionarDadosPaciente(Document document, RelatorioResultViewModel relatorio)
        {
            document.Add(new Paragraph("DADOS DO PACIENTE")
                .SetFont(_subtitleFont)
                .SetFontSize(14));
            document.Add(new Paragraph(" "));

            var table = new Table(2).UseAllAvailableWidth();
            table.SetMarginTop(5).SetMarginBottom(15);

            AdicionarCelulaDados(table, "Nome:", relatorio.Usuario.Nome);
            AdicionarCelulaDados(table, "Email:", relatorio.Usuario.Email);
            if (relatorio.Usuario.DataNascimento.HasValue)
            {
                var idade = DateTime.Today.Year - relatorio.Usuario.DataNascimento.Value.Year;
                if (relatorio.Usuario.DataNascimento.Value.Date > DateTime.Today.AddYears(-idade)) idade--;
                AdicionarCelulaDados(table, "Data de Nascimento:", $"{relatorio.Usuario.DataNascimento.Value:dd/MM/yyyy} ({idade} anos)");
            }
            AdicionarCelulaDados(table, "Cadastrado em:", relatorio.Usuario.DataCadastro.ToString("dd/MM/yyyy"));

            document.Add(table);
            document.Add(new Paragraph(" "));
        }

        private void AdicionarPeriodoRelatorio(Document document, RelatorioResultViewModel relatorio)
        {
            var paragrafo = new Paragraph($"PERÍODO ANALISADO: {relatorio.DataInicial:dd/MM/yyyy} a {relatorio.DataFinal:dd/MM/yyyy}")
                .SetFont(_boldFont)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(10);
            document.Add(paragrafo);
        }

        private void AdicionarTabelasReferencia(Document document, TabelasReferenciaViewModel tabelas)
        {
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph("VALORES DE REFERÊNCIA")
                .SetFont(_subtitleFont)
                .SetFontSize(14));
            document.Add(new Paragraph(" "));

            // Tabela de Pressão Arterial
            document.Add(new Paragraph("Pressão Arterial (mmHg):")
                .SetFont(_boldFont)
                .SetFontSize(10));
            document.Add(new Paragraph(" "));
            
            var tablePressao = new Table(3).UseAllAvailableWidth();
            tablePressao.SetMarginTop(5).SetMarginBottom(10);

            // Cabeçalho
            AdicionarCelulaCabecalho(tablePressao, "Classificação");
            AdicionarCelulaCabecalho(tablePressao, "Sistólica");
            AdicionarCelulaCabecalho(tablePressao, "Diastólica");

            foreach (var referencia in tabelas.ReferenciaPressao)
            {
                AdicionarCelulaTabela(tablePressao, referencia.Classificacao);
                AdicionarCelulaTabela(tablePressao, referencia.ValorSistolica);
                AdicionarCelulaTabela(tablePressao, referencia.ValorDiastolica);
            }

            document.Add(tablePressao);
            document.Add(new Paragraph(" "));

            // Tabela de Glicose
            document.Add(new Paragraph("Glicose (mg/dL):")
                .SetFont(_boldFont)
                .SetFontSize(10));
            document.Add(new Paragraph(" "));
            
            var tableGlicose = new Table(4).UseAllAvailableWidth();
            tableGlicose.SetMarginTop(5).SetMarginBottom(10);

            // Cabeçalho
            AdicionarCelulaCabecalho(tableGlicose, "Período");
            AdicionarCelulaCabecalho(tableGlicose, "Normal");
            AdicionarCelulaCabecalho(tableGlicose, "Pré-diabetes");
            AdicionarCelulaCabecalho(tableGlicose, "Diabetes");

            foreach (var referencia in tabelas.ReferenciaGlicose)
            {
                AdicionarCelulaTabela(tableGlicose, referencia.Periodo);
                AdicionarCelulaTabela(tableGlicose, referencia.Normal);
                AdicionarCelulaTabela(tableGlicose, referencia.PreDiabetes);
                AdicionarCelulaTabela(tableGlicose, referencia.Diabetes);
            }

            document.Add(tableGlicose);
            document.Add(new Paragraph(" "));

            // Tabela de IMC
            document.Add(new Paragraph("Índice de Massa Corporal (IMC):")
                .SetFont(_boldFont)
                .SetFontSize(10));
            document.Add(new Paragraph(" "));
            
            var tableIMC = new Table(3).UseAllAvailableWidth();
            tableIMC.SetMarginTop(5).SetMarginBottom(10);

            // Cabeçalho
            AdicionarCelulaCabecalho(tableIMC, "Classificação");
            AdicionarCelulaCabecalho(tableIMC, "Faixa IMC");
            AdicionarCelulaCabecalho(tableIMC, "Risco");

            foreach (var referencia in tabelas.ReferenciaIMC)
            {
                AdicionarCelulaTabela(tableIMC, referencia.Classificacao);
                AdicionarCelulaTabela(tableIMC, referencia.FaixaIMC);
                AdicionarCelulaTabela(tableIMC, referencia.ClasseRisco);
            }

            document.Add(tableIMC);
            document.Add(new Paragraph(" "));
        }

        private void AdicionarAnalisesPressao(Document document, AnalisesPressaoViewModel analise, List<Pressao> pressoes)
        {
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph("ANÁLISE DA PRESSÃO ARTERIAL")
                .SetFont(_subtitleFont)
                .SetFontSize(14));
            document.Add(new Paragraph(" "));

            // Resumo estatístico
            var tableResumo = new Table(2).UseAllAvailableWidth();
            tableResumo.SetMarginTop(5).SetMarginBottom(10);

            AdicionarCelulaDados(tableResumo, "Total de medições:", analise.TotalMedicoes.ToString());
            AdicionarCelulaDados(tableResumo, "Média sistólica:", $"{analise.MediaSistolica} mmHg");
            AdicionarCelulaDados(tableResumo, "Média diastólica:", $"{analise.MediaDiastolica} mmHg");
            AdicionarCelulaDados(tableResumo, "Média FC:", $"{analise.MediaFrequenciaCardiaca} bpm");
            AdicionarCelulaDados(tableResumo, "Classificação predominante:", analise.ClassificacaoPredominate);
            AdicionarCelulaDados(tableResumo, "% Hipertensão:", $"{analise.PercentualHipertensao}%");

            document.Add(tableResumo);
            document.Add(new Paragraph(" "));

            // Tendência
            document.Add(new Paragraph($"Tendência: {analise.TendenciaGeral}")
                .SetFont(_boldFont)
                .SetFontSize(10));
            document.Add(new Paragraph(" "));

            // Histórico das últimas 10 medições
            if (pressoes.Any())
            {
                document.Add(new Paragraph("Histórico Recente (últimas medições):")
                    .SetFont(_boldFont)
                    .SetFontSize(10));
                document.Add(new Paragraph(" "));
                
                var tableHistorico = new Table(4).UseAllAvailableWidth();
                tableHistorico.SetMarginTop(5).SetMarginBottom(10);

                AdicionarCelulaCabecalho(tableHistorico, "Data/Hora");
                AdicionarCelulaCabecalho(tableHistorico, "Sistólica");
                AdicionarCelulaCabecalho(tableHistorico, "Diastólica");
                AdicionarCelulaCabecalho(tableHistorico, "Classificação");

                foreach (var pressao in pressoes.OrderByDescending(p => p.DataHora).Take(10))
                {
                    AdicionarCelulaTabela(tableHistorico, pressao.DataHora.ToString("dd/MM/yyyy HH:mm"));
                    AdicionarCelulaTabela(tableHistorico, pressao.Sistolica.ToString());
                    AdicionarCelulaTabela(tableHistorico, pressao.Diastolica.ToString());
                    AdicionarCelulaTabela(tableHistorico, pressao.ClassificacaoPressao);
                }

                document.Add(tableHistorico);
            }

            document.Add(new Paragraph(" "));
        }

        private void AdicionarAnalisesGlicose(Document document, AnalisesGlicoseViewModel analise, List<Glicose> glicoses)
        {
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph("ANÁLISE DA GLICOSE")
                .SetFont(_subtitleFont)
                .SetFontSize(14));
            document.Add(new Paragraph(" "));

            // Resumo estatístico
            var tableResumo = new Table(2).UseAllAvailableWidth();
            tableResumo.SetMarginTop(5).SetMarginBottom(10);

            AdicionarCelulaDados(tableResumo, "Total de medições:", analise.TotalMedicoes.ToString());
            if (analise.MediaJejum > 0)
                AdicionarCelulaDados(tableResumo, "Média jejum:", $"{analise.MediaJejum} mg/dL");
            if (analise.MediaPosRefeicao > 0)
                AdicionarCelulaDados(tableResumo, "Média pós-refeição:", $"{analise.MediaPosRefeicao} mg/dL");
            AdicionarCelulaDados(tableResumo, "Classificação predominante:", analise.ClassificacaoPredominate);
            AdicionarCelulaDados(tableResumo, "% Valores alterados:", $"{analise.PercentualAlteradas}%");
            AdicionarCelulaDados(tableResumo, "Controle glicêmico:", analise.ControleGlicemico);

            document.Add(tableResumo);
            document.Add(new Paragraph(" "));

            // Histórico das últimas 10 medições
            if (glicoses.Any())
            {
                document.Add(new Paragraph("Histórico Recente (últimas medições):")
                    .SetFont(_boldFont)
                    .SetFontSize(10));
                document.Add(new Paragraph(" "));
                
                var tableHistorico = new Table(4).UseAllAvailableWidth();
                tableHistorico.SetMarginTop(5).SetMarginBottom(10);

                AdicionarCelulaCabecalho(tableHistorico, "Data/Hora");
                AdicionarCelulaCabecalho(tableHistorico, "Valor");
                AdicionarCelulaCabecalho(tableHistorico, "Período");
                AdicionarCelulaCabecalho(tableHistorico, "Classificação");

                foreach (var glicose in glicoses.OrderByDescending(g => g.DataHora).Take(10))
                {
                    AdicionarCelulaTabela(tableHistorico, glicose.DataHora.ToString("dd/MM/yyyy HH:mm"));
                    AdicionarCelulaTabela(tableHistorico, $"{glicose.Valor} mg/dL");
                    AdicionarCelulaTabela(tableHistorico, glicose.Periodo.ToString());
                    AdicionarCelulaTabela(tableHistorico, glicose.ClassificacaoGlicose);
                }

                document.Add(tableHistorico);
            }

            document.Add(new Paragraph(" "));
        }

        private void AdicionarAnalisesPeso(Document document, AnalisesPesoViewModel analise, List<Peso> pesos)
        {
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph("ANÁLISE DE PESO E IMC")
                .SetFont(_subtitleFont)
                .SetFontSize(14));
            document.Add(new Paragraph(" "));

            // Resumo estatístico
            var tableResumo = new Table(2).UseAllAvailableWidth();
            tableResumo.SetMarginTop(5).SetMarginBottom(10);

            AdicionarCelulaDados(tableResumo, "Total de medições:", analise.TotalMedicoes.ToString());
            AdicionarCelulaDados(tableResumo, "Peso médio:", $"{analise.MediaPeso} kg");
            AdicionarCelulaDados(tableResumo, "Altura média:", $"{analise.MediaAltura} cm");
            AdicionarCelulaDados(tableResumo, "IMC médio:", analise.MediaIMC.ToString());
            AdicionarCelulaDados(tableResumo, "Peso ideal médio:", $"{analise.PesoIdealMedio} kg");
            AdicionarCelulaDados(tableResumo, "Classificação predominante:", analise.ClassificacaoIMCPredominante);
            AdicionarCelulaDados(tableResumo, "% Peso normal:", $"{analise.PercentualPesoNormal}%");

            document.Add(tableResumo);
            document.Add(new Paragraph(" "));

            // Status e tendência
            document.Add(new Paragraph($"Status: {analise.StatusPesoGeral}")
                .SetFont(_boldFont)
                .SetFontSize(10));
            document.Add(new Paragraph($"Tendência: {analise.TendenciaPeso}")
                .SetFont(_boldFont)
                .SetFontSize(10));
            document.Add(new Paragraph(" "));

            // Histórico das últimas 10 medições
            if (pesos.Any())
            {
                document.Add(new Paragraph("Histórico Recente (últimas medições):")
                    .SetFont(_boldFont)
                    .SetFontSize(10));
                document.Add(new Paragraph(" "));
                
                var tableHistorico = new Table(5).UseAllAvailableWidth();
                tableHistorico.SetMarginTop(5).SetMarginBottom(10);

                AdicionarCelulaCabecalho(tableHistorico, "Data/Hora");
                AdicionarCelulaCabecalho(tableHistorico, "Peso (kg)");
                AdicionarCelulaCabecalho(tableHistorico, "Altura (cm)");
                AdicionarCelulaCabecalho(tableHistorico, "IMC");
                AdicionarCelulaCabecalho(tableHistorico, "Classificação");

                foreach (var peso in pesos.OrderByDescending(p => p.DataHora).Take(10))
                {
                    AdicionarCelulaTabela(tableHistorico, peso.DataHora.ToString("dd/MM/yyyy HH:mm"));
                    AdicionarCelulaTabela(tableHistorico, peso.PesoKg.ToString());
                    AdicionarCelulaTabela(tableHistorico, peso.Altura.ToString());
                    AdicionarCelulaTabela(tableHistorico, peso.IMC.ToString());
                    AdicionarCelulaTabela(tableHistorico, peso.ClassificacaoIMC);
                }

                document.Add(tableHistorico);
            }

            document.Add(new Paragraph(" "));
        }

        private void AdicionarObservacoesMedicas(Document document, string observacoes)
        {
            if (!string.IsNullOrEmpty(observacoes))
            {
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("OBSERVAÇÕES MÉDICAS")
                    .SetFont(_subtitleFont)
                    .SetFontSize(14));
                document.Add(new Paragraph(" "));

                var paragrafo = new Paragraph(observacoes)
                    .SetFont(_normalFont)
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.JUSTIFIED)
                    .SetMarginBottom(10);
                document.Add(paragrafo);
                document.Add(new Paragraph(" "));
            }
        }

        private void AdicionarAlertasCriticos(Document document, List<string> alertas)
        {
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph("ALERTAS CRÍTICOS")
                .SetFont(_subtitleFont)
                .SetFontSize(14));
            document.Add(new Paragraph(" "));

            foreach (var alerta in alertas)
            {
                var paragrafo = new Paragraph($"• {alerta}")
                    .SetFont(_normalFont)
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.RED)
                    .SetMarginBottom(5);
                document.Add(paragrafo);
            }
            document.Add(new Paragraph(" "));
        }

        private void AdicionarRecomendacoes(Document document, string recomendacoes)
        {
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph("RECOMENDAÇÕES GERAIS")
                .SetFont(_subtitleFont)
                .SetFontSize(14));
            document.Add(new Paragraph(" "));

            var paragrafo = new Paragraph(recomendacoes)
                .SetFont(_normalFont)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.JUSTIFIED)
                .SetMarginBottom(10);
            document.Add(paragrafo);
            document.Add(new Paragraph(" "));
        }

        private void AdicionarDisclaimerMedico(Document document, string disclaimer)
        {
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph("IMPORTANTE:")
                .SetFont(_boldFont)
                .SetFontSize(10));
            document.Add(new Paragraph(" "));
            
            var paragrafo = new Paragraph(disclaimer)
                .SetFont(_normalFont)
                .SetFontSize(9)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetTextAlignment(TextAlignment.JUSTIFIED)
                .SetMarginTop(20)
                .SetMarginBottom(10);
            document.Add(paragrafo);
            document.Add(new Paragraph(" "));
        }

        private void AdicionarRodape(Document document)
        {
            var rodape = new Paragraph($"Relatório gerado automaticamente em {DateTime.Now:dd/MM/yyyy HH:mm} - ControlePressao System")
                .SetFont(_smallFont)
                .SetFontSize(8)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(10);
            document.Add(rodape);
        }

        // Métodos auxiliares para formatação de células
        private void AdicionarCelulaDados(Table table, string label, string value)
        {
            table.AddCell(new Cell()
                .Add(new Paragraph(label).SetFont(_boldFont).SetFontSize(10))
                .SetPadding(5)
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            table.AddCell(new Cell()
                .Add(new Paragraph(value).SetFont(_normalFont).SetFontSize(10))
                .SetPadding(5)
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        }

        private void AdicionarCelulaCabecalho(Table table, string text)
        {
            var cell = new Cell()
                .Add(new Paragraph(text).SetFont(_boldFont).SetFontSize(10))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(5)
                .SetTextAlignment(TextAlignment.CENTER);
            table.AddCell(cell);
        }

        private void AdicionarCelulaTabela(Table table, string text)
        {
            var cell = new Cell()
                .Add(new Paragraph(text).SetFont(_normalFont).SetFontSize(10))
                .SetPadding(5)
                .SetTextAlignment(TextAlignment.CENTER);
            table.AddCell(cell);
        }
    }
}