using System.ComponentModel.DataAnnotations;

namespace ControlePressao.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }
        
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email em formato inválido.")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres.")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [StringLength(255, ErrorMessage = "A senha deve ter no máximo 255 caracteres.")]
        public string Senha { get; set; }
        
        [DataType(DataType.DateTime)]
        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; }
        
        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }
        
        [StringLength(15, ErrorMessage = "O telefone deve ter no máximo 15 caracteres.")]
        public string? Telefone { get; set; }
        
        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }
        
        public User()
        {
            DataCadastro = DateTime.Now;
            Ativo = true;
        }
    }
} 