
using System;
using System.ComponentModel.DataAnnotations;

namespace ControlePressao.Models
{
    public class Glicose
    {
        public int Id { get; set; }

        [Display(Name = "Data e Hora")]
        [Required(ErrorMessage = "A data e hora são obrigatórias")]
        public DateTime DataHora { get; set; }

        [Display(Name = "Valor da Glicose (mg/dL)")]
        [Required(ErrorMessage = "O valor da glicose é obrigatório")]
        [Range(40, 600, ErrorMessage = "O valor da glicose deve estar entre 40 e 600 mg/dL")]
        public int Valor { get; set; }

        [Display(Name = "Período do Teste")]
        [Required(ErrorMessage = "O período do teste é obrigatório")]
        public PeriodoTeste Periodo { get; set; }

        [Display(Name = "Observações")]
        [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
        public string? Observacoes { get; set; }

        // Propriedades calculadas
        public string ClassificacaoGlicose
        {
            get
            {
                return Periodo switch
                {
                    PeriodoTeste.Jejum => Valor switch
                    {
                        < 70 => "Hipoglicemia",
                        >= 70 and < 100 => "Normal",
                        >= 100 and < 126 => "Pré-diabetes",
                        >= 126 => "Diabetes"
                    },
                    PeriodoTeste.PosRefeicao => Valor switch
                    {
                        < 70 => "Hipoglicemia",
                        >= 70 and < 140 => "Normal",
                        >= 140 and < 200 => "Pré-diabetes",
                        >= 200 => "Diabetes"
                    },
                    PeriodoTeste.Casual => Valor switch
                    {
                        < 70 => "Hipoglicemia",
                        >= 70 and < 200 => "Normal",
                        >= 200 => "Suspeita de Diabetes"
                    },
                    _ => "Indefinido"
                };
            }
        }

        public string ClasseRisco
        {
            get
            {
                return ClassificacaoGlicose switch
                {
                    "Hipoglicemia" => "Alto",
                    "Normal" => "Baixo",
                    "Pré-diabetes" => "Moderado",
                    "Diabetes" or "Suspeita de Diabetes" => "Alto",
                    _ => "Indefinido"
                };
            }
        }
    }

    public enum PeriodoTeste
    {
        [Display(Name = "Jejum")]
        Jejum,
        [Display(Name = "Pós-refeição")]
        PosRefeicao,
        [Display(Name = "Casual")]
        Casual
    }
}
