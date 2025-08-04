namespace ControlePressao.Models
{
    public class RelatorioResultViewModel
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public TipoRelatorio TipoRelatorio { get; set; }
        public User Usuario { get; set; } = null!;
        
        public List<Pressao> Pressoes { get; set; } = new();
        public List<Glicose> Glicoses { get; set; } = new();
        public List<Peso> Pesos { get; set; } = new();
        
        public AnaliseRelatorioViewModel Analise { get; set; } = new();
        public TabelasReferenciaViewModel TabelasReferencia { get; set; } = new();
        
        public string ObservacoesMedicas { get; set; } = string.Empty;
        public string DisclaimerMedico { get; set; } = "IMPORTANTE: Este relatório é apenas informativo e não substitui a consulta médica. Os valores e análises apresentados devem ser interpretados por um profissional de saúde qualificado. Sempre procure orientação médica para diagnóstico e tratamento adequados.";
    }

    public class AnaliseRelatorioViewModel
    {
        public AnalisesPressaoViewModel? AnalisesPressao { get; set; }
        public AnalisesGlicoseViewModel? AnalisesGlicose { get; set; }
        public AnalisesPesoViewModel? AnalisesPeso { get; set; }
        public string RecomendacoesGerais { get; set; } = string.Empty;
        public List<string> AlertasCriticos { get; set; } = new();
    }

    public class AnalisesPressaoViewModel
    {
        public double MediaSistolica { get; set; }
        public double MediaDiastolica { get; set; }
        public double MediaFrequenciaCardiaca { get; set; }
        public string ClassificacaoPredominate { get; set; } = string.Empty;
        public int TotalMedicoes { get; set; }
        public int MedicoesHipertensao { get; set; }
        public double PercentualHipertensao { get; set; }
        public string TendenciaGeral { get; set; } = string.Empty;
        public List<string> Observacoes { get; set; } = new();
    }

    public class AnalisesGlicoseViewModel
    {
        public double MediaJejum { get; set; }
        public double MediaPosRefeicao { get; set; }
        public double MediaCasual { get; set; }
        public string ClassificacaoPredominate { get; set; } = string.Empty;
        public int TotalMedicoes { get; set; }
        public int MedicoesAlteradas { get; set; }
        public double PercentualAlteradas { get; set; }
        public string ControleGlicemico { get; set; } = string.Empty;
        public List<string> Observacoes { get; set; } = new();
    }

    public class AnalisesPesoViewModel
    {
        public double MediaPeso { get; set; }
        public double MediaAltura { get; set; }
        public double MediaIMC { get; set; }
        public double PesoIdealMedio { get; set; }
        public string ClassificacaoIMCPredominante { get; set; } = string.Empty;
        public int TotalMedicoes { get; set; }
        public int MedicoesObesidade { get; set; }
        public int MedicoesSobrepeso { get; set; }
        public int MedicoesBaixoPeso { get; set; }
        public double PercentualPesoNormal { get; set; }
        public string TendenciaPeso { get; set; } = string.Empty;
        public string StatusPesoGeral { get; set; } = string.Empty;
        public List<string> Observacoes { get; set; } = new();
    }

    public class TabelasReferenciaViewModel
    {
        public List<ReferenciaValorViewModel> ReferenciaPressao { get; set; } = new()
        {
            new() { Classificacao = "Ótima", ValorSistolica = "< 120", ValorDiastolica = "< 80", CorRisco = "#28a745" },
            new() { Classificacao = "Normal", ValorSistolica = "< 130", ValorDiastolica = "< 85", CorRisco = "#20c997" },
            new() { Classificacao = "Limítrofe", ValorSistolica = "130-139", ValorDiastolica = "85-89", CorRisco = "#ffc107" },
            new() { Classificacao = "Hipertensão Leve", ValorSistolica = "140-159", ValorDiastolica = "90-99", CorRisco = "#fd7e14" },
            new() { Classificacao = "Hipertensão Moderada", ValorSistolica = "160-179", ValorDiastolica = "100-109", CorRisco = "#dc3545" },
            new() { Classificacao = "Hipertensão Grave", ValorSistolica = "≥ 180", ValorDiastolica = "≥ 110", CorRisco = "#6f42c1" }
        };

        public List<ReferenciaGlicoseViewModel> ReferenciaGlicose { get; set; } = new()
        {
            new() { Periodo = "Jejum", Normal = "70-99 mg/dL", PreDiabetes = "100-125 mg/dL", Diabetes = "≥ 126 mg/dL" },
            new() { Periodo = "Pós-refeição", Normal = "< 140 mg/dL", PreDiabetes = "140-199 mg/dL", Diabetes = "≥ 200 mg/dL" },
            new() { Periodo = "Casual", Normal = "< 200 mg/dL", PreDiabetes = "N/A", Diabetes = "≥ 200 mg/dL" }
        };

        public List<ReferenciaIMCViewModel> ReferenciaIMC { get; set; } = new()
        {
            new() { Classificacao = "Baixo peso", FaixaIMC = "< 18.5", ClasseRisco = "Moderado", CorRisco = "#17a2b8" },
            new() { Classificacao = "Peso normal", FaixaIMC = "18.5 - 24.9", ClasseRisco = "Baixo", CorRisco = "#28a745" },
            new() { Classificacao = "Sobrepeso", FaixaIMC = "25.0 - 29.9", ClasseRisco = "Moderado", CorRisco = "#ffc107" },
            new() { Classificacao = "Obesidade Grau I", FaixaIMC = "30.0 - 34.9", ClasseRisco = "Alto", CorRisco = "#fd7e14" },
            new() { Classificacao = "Obesidade Grau II", FaixaIMC = "35.0 - 39.9", ClasseRisco = "Muito Alto", CorRisco = "#dc3545" },
            new() { Classificacao = "Obesidade Grau III", FaixaIMC = "≥ 40.0", ClasseRisco = "Extremamente Alto", CorRisco = "#6f42c1" }
        };
    }

    public class ReferenciaValorViewModel
    {
        public string Classificacao { get; set; } = string.Empty;
        public string ValorSistolica { get; set; } = string.Empty;
        public string ValorDiastolica { get; set; } = string.Empty;
        public string CorRisco { get; set; } = string.Empty;
    }

    public class ReferenciaGlicoseViewModel
    {
        public string Periodo { get; set; } = string.Empty;
        public string Normal { get; set; } = string.Empty;
        public string PreDiabetes { get; set; } = string.Empty;
        public string Diabetes { get; set; } = string.Empty;
    }

    public class ReferenciaIMCViewModel
    {
        public string Classificacao { get; set; } = string.Empty;
        public string FaixaIMC { get; set; } = string.Empty;
        public string ClasseRisco { get; set; } = string.Empty;
        public string CorRisco { get; set; } = string.Empty;
    }
}