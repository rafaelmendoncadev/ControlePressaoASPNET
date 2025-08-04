using ClosedXML.Excel;
using ControlePressao.Models;
using System.Globalization;

namespace ControlePressao.Services
{
    public class ExcelService
    {
        public byte[] GerarRelatorioExcel(RelatorioResultViewModel relatorio)
        {
            using (var workbook = new XLWorkbook())
            {
                // Aba principal com resumo
                AdicionarAbaResumo(workbook, relatorio);

                // Aba com dados da pressão
                if (relatorio.Pressoes.Any())
                {
                    AdicionarAbaPressao(workbook, relatorio.Pressoes, relatorio.Analise.AnalisesPressao);
                }

                // Aba com dados da glicose
                if (relatorio.Glicoses.Any())
                {
                    AdicionarAbaGlicose(workbook, relatorio.Glicoses, relatorio.Analise.AnalisesGlicose);
                }

                // Aba com dados de peso
                if (relatorio.Pesos.Any())
                {
                    AdicionarAbaPeso(workbook, relatorio.Pesos, relatorio.Analise.AnalisesPeso);
                }

                // Aba com tabelas de referência
                AdicionarAbaReferencias(workbook, relatorio.TabelasReferencia);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private void AdicionarAbaResumo(XLWorkbook workbook, RelatorioResultViewModel relatorio)
        {
            var worksheet = workbook.Worksheets.Add("Resumo");
            
            var linha = 1;

            // Cabeçalho
            worksheet.Cell(linha, 1).Value = "RELATÓRIO DE SAÚDE - CONTROLE DE PRESSÃO E GLICOSE";
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            worksheet.Cell(linha, 1).Style.Font.FontSize = 16;
            worksheet.Range(linha, 1, linha, 6).Merge();
            linha += 2;

            // Dados do paciente
            worksheet.Cell(linha, 1).Value = "DADOS DO PACIENTE";
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            worksheet.Cell(linha, 1).Style.Font.FontSize = 14;
            linha++;

            worksheet.Cell(linha, 1).Value = "Nome:";
            worksheet.Cell(linha, 2).Value = relatorio.Usuario.Nome;
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            linha++;

            worksheet.Cell(linha, 1).Value = "Email:";
            worksheet.Cell(linha, 2).Value = relatorio.Usuario.Email;
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            linha++;

            if (relatorio.Usuario.DataNascimento.HasValue)
            {
                var idade = DateTime.Today.Year - relatorio.Usuario.DataNascimento.Value.Year;
                if (relatorio.Usuario.DataNascimento.Value.Date > DateTime.Today.AddYears(-idade)) idade--;
                
                worksheet.Cell(linha, 1).Value = "Data de Nascimento:";
                worksheet.Cell(linha, 2).Value = $"{relatorio.Usuario.DataNascimento.Value:dd/MM/yyyy} ({idade} anos)";
                worksheet.Cell(linha, 1).Style.Font.Bold = true;
                linha++;
            }

            worksheet.Cell(linha, 1).Value = "Período do Relatório:";
            worksheet.Cell(linha, 2).Value = $"{relatorio.DataInicial:dd/MM/yyyy} a {relatorio.DataFinal:dd/MM/yyyy}";
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            linha += 2;

            // Análises de Pressão
            if (relatorio.Analise.AnalisesPressao != null)
            {
                linha = AdicionarSecaoAnalisesPressao(worksheet, linha, relatorio.Analise.AnalisesPressao);
            }

            // Análises de Glicose
            if (relatorio.Analise.AnalisesGlicose != null)
            {
                linha = AdicionarSecaoAnalisesGlicose(worksheet, linha, relatorio.Analise.AnalisesGlicose);
            }

            // Análises de Peso
            if (relatorio.Analise.AnalisesPeso != null)
            {
                linha = AdicionarSecaoAnalisesPeso(worksheet, linha, relatorio.Analise.AnalisesPeso);
            }

            // Alertas críticos
            if (relatorio.Analise.AlertasCriticos.Any())
            {
                worksheet.Cell(linha, 1).Value = "ALERTAS CRÍTICOS";
                worksheet.Cell(linha, 1).Style.Font.Bold = true;
                worksheet.Cell(linha, 1).Style.Font.FontSize = 14;
                worksheet.Cell(linha, 1).Style.Font.FontColor = XLColor.Red;
                linha++;

                foreach (var alerta in relatorio.Analise.AlertasCriticos)
                {
                    worksheet.Cell(linha, 1).Value = alerta;
                    worksheet.Cell(linha, 1).Style.Font.FontColor = XLColor.Red;
                    linha++;
                }
                linha++;
            }

            // Observações médicas
            worksheet.Cell(linha, 1).Value = "OBSERVAÇÕES MÉDICAS";
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            worksheet.Cell(linha, 1).Style.Font.FontSize = 14;
            linha++;

            worksheet.Cell(linha, 1).Value = relatorio.ObservacoesMedicas;
            worksheet.Cell(linha, 1).Style.Alignment.WrapText = true;
            worksheet.Range(linha, 1, linha, 6).Merge();
            linha += 3;

            // Recomendações
            worksheet.Cell(linha, 1).Value = "RECOMENDAÇÕES";
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            worksheet.Cell(linha, 1).Style.Font.FontSize = 14;
            linha++;

            worksheet.Cell(linha, 1).Value = relatorio.Analise.RecomendacoesGerais;
            worksheet.Cell(linha, 1).Style.Alignment.WrapText = true;
            worksheet.Range(linha, 1, linha, 6).Merge();
            linha += 2;

            // Disclaimer
            worksheet.Cell(linha, 1).Value = relatorio.DisclaimerMedico;
            worksheet.Cell(linha, 1).Style.Font.Italic = true;
            worksheet.Cell(linha, 1).Style.Font.FontColor = XLColor.DarkGray;
            worksheet.Cell(linha, 1).Style.Alignment.WrapText = true;
            worksheet.Range(linha, 1, linha, 6).Merge();

            // Ajustar largura das colunas
            worksheet.Columns().AdjustToContents();
        }

        private int AdicionarSecaoAnalisesPressao(IXLWorksheet worksheet, int linha, AnalisesPressaoViewModel analise)
        {
            worksheet.Cell(linha, 1).Value = "ANÁLISE DA PRESSÃO ARTERIAL";
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            worksheet.Cell(linha, 1).Style.Font.FontSize = 14;
            linha++;

            var dados = new (string Label, string Valor)[]
            {
                ("Total de medições:", analise.TotalMedicoes.ToString()),
                ("Média sistólica:", $"{analise.MediaSistolica} mmHg"),
                ("Média diastólica:", $"{analise.MediaDiastolica} mmHg"),
                ("Média FC:", $"{analise.MediaFrequenciaCardiaca} bpm"),
                ("Classificação predominante:", analise.ClassificacaoPredominate),
                ("% Hipertensão:", $"{analise.PercentualHipertensao}%"),
                ("Tendência:", analise.TendenciaGeral)
            };

            foreach (var (label, valor) in dados)
            {
                worksheet.Cell(linha, 1).Value = label;
                worksheet.Cell(linha, 2).Value = valor;
                worksheet.Cell(linha, 1).Style.Font.Bold = true;
                linha++;
            }

            return linha + 1;
        }

        private int AdicionarSecaoAnalisesGlicose(IXLWorksheet worksheet, int linha, AnalisesGlicoseViewModel analise)
        {
            worksheet.Cell(linha, 1).Value = "ANÁLISE DA GLICOSE";
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            worksheet.Cell(linha, 1).Style.Font.FontSize = 14;
            linha++;

            var dados = new List<(string Label, string Valor)>
            {
                ("Total de medições:", analise.TotalMedicoes.ToString())
            };

            if (analise.MediaJejum > 0)
                dados.Add(("Média jejum:", $"{analise.MediaJejum} mg/dL"));
            
            if (analise.MediaPosRefeicao > 0)
                dados.Add(("Média pós-refeição:", $"{analise.MediaPosRefeicao} mg/dL"));
            
            if (analise.MediaCasual > 0)
                dados.Add(("Média casual:", $"{analise.MediaCasual} mg/dL"));

            dados.AddRange(new[]
            {
                ("Classificação predominante:", analise.ClassificacaoPredominate),
                ("% Valores alterados:", $"{analise.PercentualAlteradas}%"),
                ("Controle glicêmico:", analise.ControleGlicemico)
            });

            foreach (var (label, valor) in dados)
            {
                worksheet.Cell(linha, 1).Value = label;
                worksheet.Cell(linha, 2).Value = valor;
                worksheet.Cell(linha, 1).Style.Font.Bold = true;
                linha++;
            }

            return linha + 1;
        }

        private int AdicionarSecaoAnalisesPeso(IXLWorksheet worksheet, int linha, AnalisesPesoViewModel analise)
        {
            worksheet.Cell(linha, 1).Value = "ANÁLISE DE PESO E IMC";
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            worksheet.Cell(linha, 1).Style.Font.FontSize = 14;
            linha++;

            var dados = new (string Label, string Valor)[]
            {
                ("Total de medições:", analise.TotalMedicoes.ToString()),
                ("Peso médio:", $"{analise.MediaPeso} kg"),
                ("Altura média:", $"{analise.MediaAltura} cm"),
                ("IMC médio:", analise.MediaIMC.ToString()),
                ("Peso ideal médio:", $"{analise.PesoIdealMedio} kg"),
                ("Classificação predominante:", analise.ClassificacaoIMCPredominante),
                ("% Peso normal:", $"{analise.PercentualPesoNormal}%"),
                ("Status:", analise.StatusPesoGeral),
                ("Tendência:", analise.TendenciaPeso)
            };

            foreach (var (label, valor) in dados)
            {
                worksheet.Cell(linha, 1).Value = label;
                worksheet.Cell(linha, 2).Value = valor;
                worksheet.Cell(linha, 1).Style.Font.Bold = true;
                linha++;
            }

            return linha + 1;
        }

        private void AdicionarAbaPressao(XLWorkbook workbook, List<Pressao> pressoes, AnalisesPressaoViewModel? analise)
        {
            var worksheet = workbook.Worksheets.Add("Pressão Arterial");
            
            // Cabeçalhos
            var colunas = new[] { "Data/Hora", "Sistólica (mmHg)", "Diastólica (mmHg)", "FC (bpm)", "Classificação", "Risco", "Observações" };
            
            for (int i = 0; i < colunas.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = colunas[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            // Dados
            var linha = 2;
            foreach (var pressao in pressoes.OrderByDescending(p => p.DataHora))
            {
                worksheet.Cell(linha, 1).Value = pressao.DataHora.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(linha, 2).Value = pressao.Sistolica;
                worksheet.Cell(linha, 3).Value = pressao.Diastolica;
                worksheet.Cell(linha, 4).Value = pressao.FrequenciaCardiaca;
                worksheet.Cell(linha, 5).Value = pressao.ClassificacaoPressao;
                worksheet.Cell(linha, 6).Value = pressao.ClasseRisco;
                worksheet.Cell(linha, 7).Value = pressao.Observacoes ?? "";

                // Colorir baseado no risco
                var cor = pressao.ClasseRisco switch
                {
                    "Baixo" => XLColor.LightGreen,
                    "Moderado" => XLColor.Yellow,
                    "Alto" => XLColor.Orange,
                    "Muito Alto" => XLColor.Red,
                    _ => XLColor.White
                };

                worksheet.Range(linha, 1, linha, 7).Style.Fill.BackgroundColor = cor;
                linha++;
            }

            // Estatísticas no final
            if (analise != null)
            {
                linha += 2;
                worksheet.Cell(linha, 1).Value = "ESTATÍSTICAS";
                worksheet.Cell(linha, 1).Style.Font.Bold = true;
                linha++;

                worksheet.Cell(linha, 1).Value = "Total de medições:";
                worksheet.Cell(linha, 2).Value = analise.TotalMedicoes;
                linha++;

                worksheet.Cell(linha, 1).Value = "Média sistólica:";
                worksheet.Cell(linha, 2).Value = analise.MediaSistolica;
                linha++;

                worksheet.Cell(linha, 1).Value = "Média diastólica:";
                worksheet.Cell(linha, 2).Value = analise.MediaDiastolica;
                linha++;

                worksheet.Cell(linha, 1).Value = "% Hipertensão:";
                worksheet.Cell(linha, 2).Value = analise.PercentualHipertensao;
            }

            worksheet.Columns().AdjustToContents();
        }

        private void AdicionarAbaGlicose(XLWorkbook workbook, List<Glicose> glicoses, AnalisesGlicoseViewModel? analise)
        {
            var worksheet = workbook.Worksheets.Add("Glicose");
            
            // Cabeçalhos
            var colunas = new[] { "Data/Hora", "Valor (mg/dL)", "Período", "Classificação", "Risco", "Observações" };
            
            for (int i = 0; i < colunas.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = colunas[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
            }

            // Dados
            var linha = 2;
            foreach (var glicose in glicoses.OrderByDescending(g => g.DataHora))
            {
                worksheet.Cell(linha, 1).Value = glicose.DataHora.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(linha, 2).Value = glicose.Valor;
                worksheet.Cell(linha, 3).Value = GetDisplayName(glicose.Periodo);
                worksheet.Cell(linha, 4).Value = glicose.ClassificacaoGlicose;
                worksheet.Cell(linha, 5).Value = glicose.ClasseRisco;
                worksheet.Cell(linha, 6).Value = glicose.Observacoes ?? "";

                // Colorir baseado no risco
                var cor = glicose.ClasseRisco switch
                {
                    "Baixo" => XLColor.LightGreen,
                    "Moderado" => XLColor.Yellow,
                    "Alto" => XLColor.Red,
                    _ => XLColor.White
                };

                worksheet.Range(linha, 1, linha, 6).Style.Fill.BackgroundColor = cor;
                linha++;
            }

            // Estatísticas no final
            if (analise != null)
            {
                linha += 2;
                worksheet.Cell(linha, 1).Value = "ESTATÍSTICAS";
                worksheet.Cell(linha, 1).Style.Font.Bold = true;
                linha++;

                worksheet.Cell(linha, 1).Value = "Total de medições:";
                worksheet.Cell(linha, 2).Value = analise.TotalMedicoes;
                linha++;

                if (analise.MediaJejum > 0)
                {
                    worksheet.Cell(linha, 1).Value = "Média jejum:";
                    worksheet.Cell(linha, 2).Value = analise.MediaJejum;
                    linha++;
                }

                if (analise.MediaPosRefeicao > 0)
                {
                    worksheet.Cell(linha, 1).Value = "Média pós-refeição:";
                    worksheet.Cell(linha, 2).Value = analise.MediaPosRefeicao;
                    linha++;
                }

                worksheet.Cell(linha, 1).Value = "% Valores alterados:";
                worksheet.Cell(linha, 2).Value = analise.PercentualAlteradas;
                linha++;

                worksheet.Cell(linha, 1).Value = "Controle glicêmico:";
                worksheet.Cell(linha, 2).Value = analise.ControleGlicemico;
            }

            worksheet.Columns().AdjustToContents();
        }

        private void AdicionarAbaPeso(XLWorkbook workbook, List<Peso> pesos, AnalisesPesoViewModel? analise)
        {
            var worksheet = workbook.Worksheets.Add("Peso e IMC");
            
            // Cabeçalhos
            var colunas = new[] { "Data/Hora", "Peso (kg)", "Altura (cm)", "IMC", "Peso Ideal (kg)", "Classificação IMC", "Risco", "Observações" };
            
            for (int i = 0; i < colunas.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = colunas[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightYellow;
            }

            // Dados
            var linha = 2;
            foreach (var peso in pesos.OrderByDescending(p => p.DataHora))
            {
                worksheet.Cell(linha, 1).Value = peso.DataHora.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(linha, 2).Value = peso.PesoKg;
                worksheet.Cell(linha, 3).Value = peso.Altura;
                worksheet.Cell(linha, 4).Value = peso.IMC;
                worksheet.Cell(linha, 5).Value = peso.PesoIdeal;
                worksheet.Cell(linha, 6).Value = peso.ClassificacaoIMC;
                worksheet.Cell(linha, 7).Value = peso.ClasseRisco;
                worksheet.Cell(linha, 8).Value = peso.Observacoes ?? "";

                // Colorir baseado no risco do IMC
                var cor = peso.ClasseRisco switch
                {
                    "Baixo" => XLColor.LightGreen,
                    "Moderado" => XLColor.Yellow,
                    "Alto" => XLColor.Orange,
                    "Muito Alto" => XLColor.Red,
                    "Extremamente Alto" => XLColor.DarkRed,
                    _ => XLColor.White
                };

                worksheet.Range(linha, 1, linha, 8).Style.Fill.BackgroundColor = cor;
                linha++;
            }

            // Estatísticas no final
            if (analise != null)
            {
                linha += 2;
                worksheet.Cell(linha, 1).Value = "ESTATÍSTICAS";
                worksheet.Cell(linha, 1).Style.Font.Bold = true;
                linha++;

                worksheet.Cell(linha, 1).Value = "Total de medições:";
                worksheet.Cell(linha, 2).Value = analise.TotalMedicoes;
                linha++;

                worksheet.Cell(linha, 1).Value = "Peso médio:";
                worksheet.Cell(linha, 2).Value = analise.MediaPeso;
                linha++;

                worksheet.Cell(linha, 1).Value = "Altura média:";
                worksheet.Cell(linha, 2).Value = analise.MediaAltura;
                linha++;

                worksheet.Cell(linha, 1).Value = "IMC médio:";
                worksheet.Cell(linha, 2).Value = analise.MediaIMC;
                linha++;

                worksheet.Cell(linha, 1).Value = "Peso ideal médio:";
                worksheet.Cell(linha, 2).Value = analise.PesoIdealMedio;
                linha++;

                worksheet.Cell(linha, 1).Value = "% Peso normal:";
                worksheet.Cell(linha, 2).Value = analise.PercentualPesoNormal;
                linha++;

                worksheet.Cell(linha, 1).Value = "Status geral:";
                worksheet.Cell(linha, 2).Value = analise.StatusPesoGeral;
            }

            worksheet.Columns().AdjustToContents();
        }

        private void AdicionarAbaReferencias(XLWorkbook workbook, TabelasReferenciaViewModel tabelas)
        {
            var worksheet = workbook.Worksheets.Add("Valores de Referência");
            
            var linha = 1;

            // Tabela de Pressão Arterial
            worksheet.Cell(linha, 1).Value = "PRESSÃO ARTERIAL (mmHg)";
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            worksheet.Cell(linha, 1).Style.Font.FontSize = 14;
            linha += 2;

            // Cabeçalhos da pressão
            worksheet.Cell(linha, 1).Value = "Classificação";
            worksheet.Cell(linha, 2).Value = "Sistólica";
            worksheet.Cell(linha, 3).Value = "Diastólica";
            
            for (int i = 1; i <= 3; i++)
            {
                worksheet.Cell(linha, i).Style.Font.Bold = true;
                worksheet.Cell(linha, i).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }
            linha++;

            foreach (var referencia in tabelas.ReferenciaPressao)
            {
                worksheet.Cell(linha, 1).Value = referencia.Classificacao;
                worksheet.Cell(linha, 2).Value = referencia.ValorSistolica;
                worksheet.Cell(linha, 3).Value = referencia.ValorDiastolica;
                linha++;
            }

            linha += 2;

            // Tabela de Glicose
            worksheet.Cell(linha, 1).Value = "GLICOSE (mg/dL)";
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            worksheet.Cell(linha, 1).Style.Font.FontSize = 14;
            linha += 2;

            // Cabeçalhos da glicose
            var cabecalhosGlicose = new[] { "Período", "Normal", "Pré-diabetes", "Diabetes" };
            for (int i = 0; i < cabecalhosGlicose.Length; i++)
            {
                worksheet.Cell(linha, i + 1).Value = cabecalhosGlicose[i];
                worksheet.Cell(linha, i + 1).Style.Font.Bold = true;
                worksheet.Cell(linha, i + 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
            }
            linha++;

            foreach (var referencia in tabelas.ReferenciaGlicose)
            {
                worksheet.Cell(linha, 1).Value = referencia.Periodo;
                worksheet.Cell(linha, 2).Value = referencia.Normal;
                worksheet.Cell(linha, 3).Value = referencia.PreDiabetes;
                worksheet.Cell(linha, 4).Value = referencia.Diabetes;
                linha++;
            }

            linha += 2;

            // Tabela de IMC
            worksheet.Cell(linha, 1).Value = "ÍNDICE DE MASSA CORPORAL (IMC)";
            worksheet.Cell(linha, 1).Style.Font.Bold = true;
            worksheet.Cell(linha, 1).Style.Font.FontSize = 14;
            linha += 2;

            // Cabeçalhos do IMC
            var cabecalhosIMC = new[] { "Classificação", "Faixa IMC", "Risco" };
            for (int i = 0; i < cabecalhosIMC.Length; i++)
            {
                worksheet.Cell(linha, i + 1).Value = cabecalhosIMC[i];
                worksheet.Cell(linha, i + 1).Style.Font.Bold = true;
                worksheet.Cell(linha, i + 1).Style.Fill.BackgroundColor = XLColor.LightYellow;
            }
            linha++;

            foreach (var referencia in tabelas.ReferenciaIMC)
            {
                worksheet.Cell(linha, 1).Value = referencia.Classificacao;
                worksheet.Cell(linha, 2).Value = referencia.FaixaIMC;
                worksheet.Cell(linha, 3).Value = referencia.ClasseRisco;
                linha++;
            }

            worksheet.Columns().AdjustToContents();
        }

        private string GetDisplayName(PeriodoTeste periodo)
        {
            return periodo switch
            {
                PeriodoTeste.Jejum => "Jejum",
                PeriodoTeste.PosRefeicao => "Pós-refeição",
                PeriodoTeste.Casual => "Casual",
                _ => periodo.ToString()
            };
        }
    }
}