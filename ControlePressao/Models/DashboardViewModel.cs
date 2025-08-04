using System.ComponentModel.DataAnnotations;

namespace ControlePressao.Models
{
    public class DashboardViewModel
    {
        public List<Pressao> UltimasPressoes { get; set; } = new();
        public List<Glicose> UltimasGlicoses { get; set; } = new();
        public List<Peso> UltimosPesos { get; set; } = new();
        public EstatisticasSaude Estatisticas { get; set; } = new();
        public List<AlertaSaude> Alertas { get; set; } = new();
    }

    public class EstatisticasSaude
    {
        [Display(Name = "Pressão Sistólica Média")]
        public double PressaoSistolicaMedia { get; set; }

        [Display(Name = "Pressão Diastólica Média")]
        public double PressaoDiastolicaMedia { get; set; }

        [Display(Name = "Frequência Cardíaca Média")]
        public double FrequenciaCardiacaMedia { get; set; }

        [Display(Name = "Glicose Média")]
        public double GlicoseMedia { get; set; }

        [Display(Name = "Total de Medições de Pressão")]
        public int TotalMedicoesPressao { get; set; }

        [Display(Name = "Total de Medições de Glicose")]
        public int TotalMedicoesGlicose { get; set; }

        [Display(Name = "Total de Medições de Peso")]
        public int TotalMedicoesPeso { get; set; }

        [Display(Name = "IMC Médio")]
        public double IMCMedio { get; set; }

        [Display(Name = "Peso Médio")]
        public double PesoMedio { get; set; }

        [Display(Name = "Altura Média")]
        public double AlturaMedia { get; set; }

        [Display(Name = "Última Medição")]
        public DateTime? UltimaMedicao { get; set; }
    }

    public class AlertaSaude
    {
        public string Tipo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public string Classe { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
    }
}
