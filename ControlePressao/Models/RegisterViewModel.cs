using System.ComponentModel.DataAnnotations;

namespace ControlePressao.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome Completo")]
        public required string Nome { get; set; }
        
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email em formato inválido.")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres.")]
        public required string Email { get; set; }
        
        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        [DataType(DataType.Password)]
        public required string Senha { get; set; }
        
        [Required(ErrorMessage = "O campo Confirmar Senha é obrigatório.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem.")]
        public required string ConfirmarSenha { get; set; }
        
        [Display(Name = "Data de Nascimento")]
        [RegularExpression(@"^\d{2}/\d{2}/\d{4}$", ErrorMessage = "Data deve estar no formato dd/mm/aaaa")]
        public string? DataNascimento { get; set; }
        
        // Propriedade auxiliar para conversão
        public DateTime? DataNascimentoDateTime
        {
            get
            {
                if (string.IsNullOrEmpty(DataNascimento))
                    return null;
                    
                if (DateTime.TryParseExact(DataNascimento, "dd/MM/yyyy", 
                    System.Globalization.CultureInfo.InvariantCulture, 
                    System.Globalization.DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
                return null;
            }
        }
        
        [StringLength(15, ErrorMessage = "O telefone deve ter no máximo 15 caracteres.")]
        public string? Telefone { get; set; }
    }
}