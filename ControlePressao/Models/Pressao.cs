
using System;
using System.ComponentModel.DataAnnotations;

namespace ControlePressao.Models
{
    public class Pressao
    {
        public int Id { get; set; }

        [Display(Name = "Data e Hora")]
        [Required(ErrorMessage = "A data e hora são obrigatórias")]
        public DateTime DataHora { get; set; }

        [Display(Name = "Pressão Sistólica (mmHg)")]
        [Required(ErrorMessage = "A pressão sistólica é obrigatória")]
        [Range(70, 250, ErrorMessage = "A pressão sistólica deve estar entre 70 e 250 mmHg")]
        public int Sistolica { get; set; }

        [Display(Name = "Pressão Diastólica (mmHg)")]
        [Required(ErrorMessage = "A pressão diastólica é obrigatória")]
        [Range(40, 150, ErrorMessage = "A pressão diastólica deve estar entre 40 e 150 mmHg")]
        public int Diastolica { get; set; }

        [Display(Name = "Frequência Cardíaca (bpm)")]
        [Required(ErrorMessage = "A frequência cardíaca é obrigatória")]
        [Range(40, 200, ErrorMessage = "A frequência cardíaca deve estar entre 40 e 200 bpm")]
        public int FrequenciaCardiaca { get; set; }

        [Display(Name = "Observações")]
        [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
        public string? Observacoes { get; set; }

        // Propriedades calculadas
        public string ClassificacaoPressao
        {
            get
            {
                if (Sistolica < 120 && Diastolica < 80)
                    return "Ótima";
                else if (Sistolica < 130 && Diastolica < 85)
                    return "Normal";
                else if (Sistolica < 140 && Diastolica < 90)
                    return "Limítrofe";
                else if (Sistolica < 160 && Diastolica < 100)
                    return "Hipertensão Leve";
                else if (Sistolica < 180 && Diastolica < 110)
                    return "Hipertensão Moderada";
                else
                    return "Hipertensão Grave";
            }
        }

        public string ClasseRisco
        {
            get
            {
                return ClassificacaoPressao switch
                {
                    "Ótima" or "Normal" => "Baixo",
                    "Limítrofe" => "Moderado",
                    "Hipertensão Leve" => "Moderado",
                    "Hipertensão Moderada" => "Alto",
                    "Hipertensão Grave" => "Muito Alto",
                    _ => "Indefinido"
                };
            }
        }
    }
}
