using ControlePressao.Models;

namespace ControlePressao.Services
{
    public class AnaliseService
    {
        public AnaliseRelatorioViewModel AnalisarDados(List<Pressao> pressoes, List<Glicose> glicoses, List<Peso> pesos, DateTime dataInicial, DateTime dataFinal)
        {
            var analise = new AnaliseRelatorioViewModel();

            if (pressoes.Any())
            {
                analise.AnalisesPressao = AnalisarPressao(pressoes);
            }

            if (glicoses.Any())
            {
                analise.AnalisesGlicose = AnalisarGlicose(glicoses);
            }

            if (pesos.Any())
            {
                analise.AnalisesPeso = AnalisarPeso(pesos);
            }

            analise.RecomendacoesGerais = GerarRecomendacoesGerais(analise.AnalisesPressao, analise.AnalisesGlicose, analise.AnalisesPeso);
            analise.AlertasCriticos = IdentificarAlertasCriticos(pressoes, glicoses, pesos);

            return analise;
        }

        private AnalisesPressaoViewModel AnalisarPressao(List<Pressao> pressoes)
        {
            var analise = new AnalisesPressaoViewModel
            {
                TotalMedicoes = pressoes.Count,
                MediaSistolica = Math.Round(pressoes.Average(p => p.Sistolica), 1),
                MediaDiastolica = Math.Round(pressoes.Average(p => p.Diastolica), 1),
                MediaFrequenciaCardiaca = Math.Round(pressoes.Average(p => p.FrequenciaCardiaca), 1)
            };

            var classificacoes = pressoes.GroupBy(p => p.ClassificacaoPressao)
                                       .OrderByDescending(g => g.Count())
                                       .First();
            analise.ClassificacaoPredominate = classificacoes.Key;

            analise.MedicoesHipertensao = pressoes.Count(p => 
                p.ClassificacaoPressao.Contains("Hipertensão") || 
                p.ClassificacaoPressao == "Limítrofe");
            
            analise.PercentualHipertensao = Math.Round((double)analise.MedicoesHipertensao / analise.TotalMedicoes * 100, 1);

            analise.TendenciaGeral = CalcularTendenciaPressao(pressoes);
            analise.Observacoes = GerarObservacoesPressao(analise);

            return analise;
        }

        private AnalisesGlicoseViewModel AnalisarGlicose(List<Glicose> glicoses)
        {
            var analise = new AnalisesGlicoseViewModel
            {
                TotalMedicoes = glicoses.Count
            };

            var jejum = glicoses.Where(g => g.Periodo == PeriodoTeste.Jejum);
            var posRefeicao = glicoses.Where(g => g.Periodo == PeriodoTeste.PosRefeicao);
            var casual = glicoses.Where(g => g.Periodo == PeriodoTeste.Casual);

            analise.MediaJejum = jejum.Any() ? Math.Round(jejum.Average(g => g.Valor), 1) : 0;
            analise.MediaPosRefeicao = posRefeicao.Any() ? Math.Round(posRefeicao.Average(g => g.Valor), 1) : 0;
            analise.MediaCasual = casual.Any() ? Math.Round(casual.Average(g => g.Valor), 1) : 0;

            var classificacoes = glicoses.GroupBy(g => g.ClassificacaoGlicose)
                                       .OrderByDescending(g => g.Count())
                                       .First();
            analise.ClassificacaoPredominate = classificacoes.Key;

            analise.MedicoesAlteradas = glicoses.Count(g => 
                g.ClassificacaoGlicose != "Normal");
            
            analise.PercentualAlteradas = Math.Round((double)analise.MedicoesAlteradas / analise.TotalMedicoes * 100, 1);

            analise.ControleGlicemico = AvaliarControleGlicemico(analise);
            analise.Observacoes = GerarObservacoesGlicose(analise);

            return analise;
        }

        private string CalcularTendenciaPressao(List<Pressao> pressoes)
        {
            if (pressoes.Count < 3) return "Dados insuficientes para análise de tendência";

            var ordenadas = pressoes.OrderBy(p => p.DataHora).ToList();
            var metadeInicial = ordenadas.Take(ordenadas.Count / 2);
            var metadeFinal = ordenadas.Skip(ordenadas.Count / 2);

            var mediaSistolicaInicial = metadeInicial.Average(p => p.Sistolica);
            var mediaSistolicaFinal = metadeFinal.Average(p => p.Sistolica);

            var diferenca = mediaSistolicaFinal - mediaSistolicaInicial;

            if (Math.Abs(diferenca) < 5)
                return "Pressão estável no período";
            else if (diferenca > 0)
                return $"Tendência de elevação (+{diferenca:F1} mmHg)";
            else
                return $"Tendência de redução ({diferenca:F1} mmHg)";
        }

        private string AvaliarControleGlicemico(AnalisesGlicoseViewModel analise)
        {
            if (analise.PercentualAlteradas <= 10)
                return "Excelente controle glicêmico";
            else if (analise.PercentualAlteradas <= 30)
                return "Bom controle glicêmico";
            else if (analise.PercentualAlteradas <= 50)
                return "Controle glicêmico moderado";
            else
                return "Controle glicêmico inadequado";
        }

        private List<string> GerarObservacoesPressao(AnalisesPressaoViewModel analise)
        {
            var observacoes = new List<string>();

            if (analise.MediaSistolica >= 140 || analise.MediaDiastolica >= 90)
            {
                observacoes.Add("Valores médios indicam hipertensão arterial. Acompanhamento médico urgente recomendado.");
            }
            else if (analise.MediaSistolica >= 130 || analise.MediaDiastolica >= 85)
            {
                observacoes.Add("Valores médios no limite superior da normalidade. Recomenda-se monitoramento frequente.");
            }

            if (analise.PercentualHipertensao > 50)
            {
                observacoes.Add($"{analise.PercentualHipertensao:F1}% das medições apresentaram valores elevados.");
            }

            if (analise.MediaFrequenciaCardiaca > 100)
            {
                observacoes.Add("Frequência cardíaca média elevada (taquicardia). Avaliação cardiológica recomendada.");
            }
            else if (analise.MediaFrequenciaCardiaca < 60)
            {
                observacoes.Add("Frequência cardíaca média baixa (bradicardia). Avaliação médica recomendada.");
            }

            return observacoes;
        }

        private List<string> GerarObservacoesGlicose(AnalisesGlicoseViewModel analise)
        {
            var observacoes = new List<string>();

            if (analise.MediaJejum >= 126)
            {
                observacoes.Add("Glicemia de jejum média indica diabetes. Acompanhamento endocrinológico urgente.");
            }
            else if (analise.MediaJejum >= 100)
            {
                observacoes.Add("Glicemia de jejum média indica pré-diabetes. Modificações no estilo de vida recomendadas.");
            }

            if (analise.MediaPosRefeicao >= 200)
            {
                observacoes.Add("Glicemia pós-refeição média muito elevada. Investigação para diabetes necessária.");
            }
            else if (analise.MediaPosRefeicao >= 140)
            {
                observacoes.Add("Glicemia pós-refeição média elevada. Atenção à dieta e atividade física.");
            }

            if (analise.PercentualAlteradas > 50)
            {
                observacoes.Add($"{analise.PercentualAlteradas:F1}% das medições apresentaram valores alterados.");
            }

            return observacoes;
        }

        private AnalisesPesoViewModel AnalisarPeso(List<Peso> pesos)
        {
            var analise = new AnalisesPesoViewModel
            {
                TotalMedicoes = pesos.Count,
                MediaPeso = Math.Round(pesos.Average(p => p.PesoKg), 1),
                MediaAltura = Math.Round(pesos.Average(p => p.Altura), 1),
                MediaIMC = Math.Round(pesos.Average(p => p.IMC), 1)
            };

            // Calcular peso ideal médio usando fórmula de Lorentz
            analise.PesoIdealMedio = Math.Round(pesos.Average(p => p.PesoIdeal), 1);

            // Classificação IMC predominante
            var classificacoes = pesos.GroupBy(p => p.ClassificacaoIMC)
                                   .OrderByDescending(g => g.Count())
                                   .First();
            analise.ClassificacaoIMCPredominante = classificacoes.Key;

            // Contadores por categoria
            analise.MedicoesBaixoPeso = pesos.Count(p => p.ClassificacaoIMC == "Baixo peso");
            analise.MedicoesSobrepeso = pesos.Count(p => p.ClassificacaoIMC == "Sobrepeso");
            analise.MedicoesObesidade = pesos.Count(p => 
                p.ClassificacaoIMC.Contains("Obesidade"));

            var medicoesNormal = pesos.Count(p => p.ClassificacaoIMC == "Peso normal");
            analise.PercentualPesoNormal = Math.Round((double)medicoesNormal / analise.TotalMedicoes * 100, 1);

            // Tendência de peso
            analise.TendenciaPeso = CalcularTendenciaPeso(pesos);
            
            // Status geral
            analise.StatusPesoGeral = AvaliarStatusPesoGeral(analise);
            
            // Observações
            analise.Observacoes = GerarObservacoesPeso(analise);

            return analise;
        }

        private string CalcularTendenciaPeso(List<Peso> pesos)
        {
            if (pesos.Count < 3) return "Dados insuficientes para análise de tendência";

            var ordenados = pesos.OrderBy(p => p.DataHora).ToList();
            var metadeInicial = ordenados.Take(ordenados.Count / 2);
            var metadeFinal = ordenados.Skip(ordenados.Count / 2);

            var pesoMedioInicial = metadeInicial.Average(p => p.PesoKg);
            var pesoMedioFinal = metadeFinal.Average(p => p.PesoKg);

            var diferenca = pesoMedioFinal - pesoMedioInicial;

            if (Math.Abs(diferenca) < 0.5)
                return "Peso estável no período";
            else if (diferenca > 0)
                return $"Tendência de ganho (+{diferenca:F1} kg)";
            else
                return $"Tendência de perda ({diferenca:F1} kg)";
        }

        private string AvaliarStatusPesoGeral(AnalisesPesoViewModel analise)
        {
            if (analise.PercentualPesoNormal >= 80)
                return "Peso adequado na maioria das medições";
            else if (analise.MediaIMC >= 30)
                return "Obesidade - acompanhamento médico recomendado";
            else if (analise.MediaIMC >= 25)
                return "Sobrepeso - atenção à dieta e exercícios";
            else if (analise.MediaIMC < 18.5)
                return "Baixo peso - avaliação nutricional recomendada";
            else
                return "Peso dentro da faixa normal";
        }

        private List<string> GerarObservacoesPeso(AnalisesPesoViewModel analise)
        {
            var observacoes = new List<string>();

            if (analise.MediaIMC >= 35)
            {
                observacoes.Add("IMC médio indica obesidade grave. Acompanhamento médico urgente recomendado.");
            }
            else if (analise.MediaIMC >= 30)
            {
                observacoes.Add("IMC médio indica obesidade. Consulta médica e nutricional recomendada.");
            }
            else if (analise.MediaIMC >= 25)
            {
                observacoes.Add("IMC médio indica sobrepeso. Atenção à alimentação e atividade física.");
            }
            else if (analise.MediaIMC < 18.5)
            {
                observacoes.Add("IMC médio indica baixo peso. Avaliação nutricional recomendada.");
            }

            var pesoIdealDiferenca = analise.MediaPeso - analise.PesoIdealMedio;
            if (Math.Abs(pesoIdealDiferenca) > 10)
            {
                if (pesoIdealDiferenca > 0)
                    observacoes.Add($"Peso atual está {pesoIdealDiferenca:F1} kg acima do peso ideal.");
                else
                    observacoes.Add($"Peso atual está {Math.Abs(pesoIdealDiferenca):F1} kg abaixo do peso ideal.");
            }

            if (analise.MedicoesObesidade > 0)
            {
                var percentualObesidade = Math.Round((double)analise.MedicoesObesidade / analise.TotalMedicoes * 100, 1);
                observacoes.Add($"{percentualObesidade}% das medições indicam obesidade.");
            }

            return observacoes;
        }

        private string GerarRecomendacoesGerais(AnalisesPressaoViewModel? pressao, AnalisesGlicoseViewModel? glicose, AnalisesPesoViewModel? peso)
        {
            var recomendacoes = new List<string>();

            if (pressao != null)
            {
                if (pressao.PercentualHipertensao > 30)
                {
                    recomendacoes.Add("• Consulta cardiológica para avaliação da hipertensão");
                    recomendacoes.Add("• Redução do consumo de sal e alimentos processados");
                    recomendacoes.Add("• Prática regular de atividade física (conforme orientação médica)");
                }
            }

            if (glicose != null)
            {
                if (glicose.PercentualAlteradas > 30)
                {
                    recomendacoes.Add("• Consulta endocrinológica para avaliação do controle glicêmico");
                    recomendacoes.Add("• Orientação nutricional especializada");
                    recomendacoes.Add("• Monitoramento mais frequente da glicemia");
                }
            }

            if (peso != null)
            {
                if (peso.MediaIMC >= 30)
                {
                    recomendacoes.Add("• Consulta médica para avaliação da obesidade");
                    recomendacoes.Add("• Acompanhamento nutricional especializado");
                    recomendacoes.Add("• Programa de exercícios supervisionado");
                }
                else if (peso.MediaIMC >= 25)
                {
                    recomendacoes.Add("• Atenção ao controle de peso corporal");
                    recomendacoes.Add("• Orientação nutricional preventiva");
                }
                else if (peso.MediaIMC < 18.5)
                {
                    recomendacoes.Add("• Avaliação nutricional para ganho de peso saudável");
                }
            }

            recomendacoes.Add("• Manutenção de hábitos alimentares saudáveis");
            recomendacoes.Add("• Controle do peso corporal");
            recomendacoes.Add("• Prática regular de exercícios físicos");
            recomendacoes.Add("• Evitar tabagismo e consumo excessivo de álcool");
            recomendacoes.Add("• Gestão adequada do estresse");

            return string.Join("\n", recomendacoes);
        }

        private List<string> IdentificarAlertasCriticos(List<Pressao> pressoes, List<Glicose> glicoses, List<Peso> pesos)
        {
            var alertas = new List<string>();

            foreach (var pressao in pressoes)
            {
                if (pressao.Sistolica >= 180 || pressao.Diastolica >= 110)
                {
                    alertas.Add($"⚠️ CRÍTICO: Pressão arterial muito elevada em {pressao.DataHora:dd/MM/yyyy HH:mm} ({pressao.Sistolica}/{pressao.Diastolica} mmHg)");
                }
                
                if (pressao.FrequenciaCardiaca >= 120 || pressao.FrequenciaCardiaca <= 50)
                {
                    alertas.Add($"⚠️ ATENÇÃO: Frequência cardíaca anormal em {pressao.DataHora:dd/MM/yyyy HH:mm} ({pressao.FrequenciaCardiaca} bpm)");
                }
            }

            foreach (var glicose in glicoses)
            {
                if (glicose.Valor >= 300)
                {
                    alertas.Add($"⚠️ CRÍTICO: Glicemia muito elevada em {glicose.DataHora:dd/MM/yyyy HH:mm} ({glicose.Valor} mg/dL)");
                }
                else if (glicose.Valor <= 60)
                {
                    alertas.Add($"⚠️ CRÍTICO: Hipoglicemia severa em {glicose.DataHora:dd/MM/yyyy HH:mm} ({glicose.Valor} mg/dL)");
                }
                else if (glicose.ClassificacaoGlicose == "Diabetes")
                {
                    alertas.Add($"⚠️ ATENÇÃO: Valor compatível com diabetes em {glicose.DataHora:dd/MM/yyyy HH:mm} ({glicose.Valor} mg/dL - {glicose.Periodo})");
                }
            }

            foreach (var peso in pesos)
            {
                if (peso.IMC >= 40)
                {
                    alertas.Add($"⚠️ CRÍTICO: Obesidade grau III em {peso.DataHora:dd/MM/yyyy HH:mm} (IMC: {peso.IMC})");
                }
                else if (peso.IMC >= 35)
                {
                    alertas.Add($"⚠️ ATENÇÃO: Obesidade grau II em {peso.DataHora:dd/MM/yyyy HH:mm} (IMC: {peso.IMC})");
                }
                else if (peso.IMC < 16)
                {
                    alertas.Add($"⚠️ CRÍTICO: Baixo peso severo em {peso.DataHora:dd/MM/yyyy HH:mm} (IMC: {peso.IMC})");
                }
            }

            return alertas.Distinct().Take(10).ToList(); // Limita a 10 alertas mais relevantes
        }

        public string GerarObservacoesMedicasPersonalizadas(AnaliseRelatorioViewModel analise, DateTime dataInicial, DateTime dataFinal)
        {
            var observacoes = new List<string>();
            var periodo = (dataFinal - dataInicial).Days;

            observacoes.Add($"ANÁLISE DO PERÍODO DE {periodo} DIAS ({dataInicial:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}):");
            observacoes.Add("");

            if (analise.AnalisesPressao != null)
            {
                observacoes.Add("PRESSÃO ARTERIAL:");
                observacoes.Add($"• Total de medições: {analise.AnalisesPressao.TotalMedicoes}");
                observacoes.Add($"• Média sistólica: {analise.AnalisesPressao.MediaSistolica} mmHg");
                observacoes.Add($"• Média diastólica: {analise.AnalisesPressao.MediaDiastolica} mmHg");
                observacoes.Add($"• Classificação predominante: {analise.AnalisesPressao.ClassificacaoPredominate}");
                observacoes.Add($"• {analise.AnalisesPressao.TendenciaGeral}");
                
                if (analise.AnalisesPressao.Observacoes.Any())
                {
                    observacoes.AddRange(analise.AnalisesPressao.Observacoes.Select(o => $"• {o}"));
                }
                observacoes.Add("");
            }

            if (analise.AnalisesGlicose != null)
            {
                observacoes.Add("CONTROLE GLICÊMICO:");
                observacoes.Add($"• Total de medições: {analise.AnalisesGlicose.TotalMedicoes}");
                if (analise.AnalisesGlicose.MediaJejum > 0)
                    observacoes.Add($"• Média em jejum: {analise.AnalisesGlicose.MediaJejum} mg/dL");
                if (analise.AnalisesGlicose.MediaPosRefeicao > 0)
                    observacoes.Add($"• Média pós-refeição: {analise.AnalisesGlicose.MediaPosRefeicao} mg/dL");
                observacoes.Add($"• Classificação predominante: {analise.AnalisesGlicose.ClassificacaoPredominate}");
                observacoes.Add($"• {analise.AnalisesGlicose.ControleGlicemico}");
                
                if (analise.AnalisesGlicose.Observacoes.Any())
                {
                    observacoes.AddRange(analise.AnalisesGlicose.Observacoes.Select(o => $"• {o}"));
                }
                observacoes.Add("");
            }

            if (analise.AnalisesPeso != null)
            {
                observacoes.Add("CONTROLE DE PESO E IMC:");
                observacoes.Add($"• Total de medições: {analise.AnalisesPeso.TotalMedicoes}");
                observacoes.Add($"• Peso médio: {analise.AnalisesPeso.MediaPeso} kg");
                observacoes.Add($"• Altura média: {analise.AnalisesPeso.MediaAltura} cm");
                observacoes.Add($"• IMC médio: {analise.AnalisesPeso.MediaIMC}");
                observacoes.Add($"• Peso ideal médio: {analise.AnalisesPeso.PesoIdealMedio} kg");
                observacoes.Add($"• Classificação predominante: {analise.AnalisesPeso.ClassificacaoIMCPredominante}");
                observacoes.Add($"• {analise.AnalisesPeso.StatusPesoGeral}");
                observacoes.Add($"• {analise.AnalisesPeso.TendenciaPeso}");
                
                if (analise.AnalisesPeso.Observacoes.Any())
                {
                    observacoes.AddRange(analise.AnalisesPeso.Observacoes.Select(o => $"• {o}"));
                }
                observacoes.Add("");
            }

            if (analise.AlertasCriticos.Any())
            {
                observacoes.Add("ALERTAS CRÍTICOS:");
                observacoes.AddRange(analise.AlertasCriticos);
                observacoes.Add("");
            }

            // Recomendações removidas daqui pois já são exibidas em seção separada

            return string.Join("\n", observacoes);
        }
    }
}