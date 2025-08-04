using System;
using System.ComponentModel.DataAnnotations;

namespace ControlePressao.Models
{
    public class Peso
    {
        public int Id { get; set; }

        [Display(Name = "Data e Hora")]
        [Required(ErrorMessage = "A data e hora são obrigatórias")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DataHora { get; set; }

        [Display(Name = "Altura (cm)")]
        [Required(ErrorMessage = "A altura é obrigatória")]
        [Range(100, 250, ErrorMessage = "A altura deve estar entre 100 e 250 cm")]
        public double Altura { get; set; }

        [Display(Name = "Peso (kg)")]
        [Required(ErrorMessage = "O peso é obrigatório")]
        [Range(20, 300, ErrorMessage = "O peso deve estar entre 20 e 300 kg")]
        public double PesoKg { get; set; }

        [Display(Name = "Observações")]
        [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
        public string? Observacoes { get; set; }

        // Relacionamento com usuário
        public int UserId { get; set; }
        public User? User { get; set; }

        // Propriedades calculadas
        public double IMC
        {
            get
            {
                var alturaMetros = Altura / 100.0;
                return Math.Round(PesoKg / (alturaMetros * alturaMetros), 2);
            }
        }

        public string ClassificacaoIMC
        {
            get
            {
                return IMC switch
                {
                    < 18.5 => "Baixo peso",
                    >= 18.5 and < 25 => "Peso normal",
                    >= 25 and < 30 => "Sobrepeso",
                    >= 30 and < 35 => "Obesidade Grau I",
                    >= 35 and < 40 => "Obesidade Grau II",
                    >= 40 => "Obesidade Grau III",
                    _ => "Indefinido"
                };
            }
        }

        public double PesoIdeal
        {
            get
            {
                // Fórmula de Devine: Peso ideal = 50 + 2.3 * (altura_em_polegadas - 60) para homens
                // Para mulheres: 45.5 + 2.3 * (altura_em_polegadas - 60)
                // Usando uma média entre os dois valores como padrão
                var alturaPolegadas = Altura / 2.54;
                var pesoIdealHomem = 50 + 2.3 * (alturaPolegadas - 60);
                var pesoIdealMulher = 45.5 + 2.3 * (alturaPolegadas - 60);
                return Math.Round((pesoIdealHomem + pesoIdealMulher) / 2, 1);
            }
        }

        public string ClasseRisco
        {
            get
            {
                return ClassificacaoIMC switch
                {
                    "Baixo peso" => "Moderado",
                    "Peso normal" => "Baixo",
                    "Sobrepeso" => "Moderado",
                    "Obesidade Grau I" => "Alto",
                    "Obesidade Grau II" => "Muito Alto",
                    "Obesidade Grau III" => "Extremamente Alto",
                    _ => "Indefinido"
                };
            }
        }

        public string StatusPeso
        {
            get
            {
                var diferenca = PesoKg - PesoIdeal;
                if (Math.Abs(diferenca) <= 2)
                    return "No peso ideal";
                else if (diferenca > 0)
                    return $"{diferenca:F1} kg acima do ideal";
                else
                    return $"{Math.Abs(diferenca):F1} kg abaixo do ideal";
            }
        }
    }
}