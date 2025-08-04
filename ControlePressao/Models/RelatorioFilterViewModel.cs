using System.ComponentModel.DataAnnotations;

namespace ControlePressao.Models
{
    public class RelatorioFilterViewModel
    {
        [Display(Name = "Data Inicial")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DataInicial { get; set; }

        [Display(Name = "Data Final")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DataFinal { get; set; }

        [Display(Name = "Tipo de Relatório")]
        public TipoRelatorio TipoRelatorio { get; set; }

        [Display(Name = "Formato de Exportação")]
        public FormatoExportacao FormatoExportacao { get; set; }

        public RelatorioFilterViewModel()
        {
            DataFinal = DateTime.Today;
            DataInicial = DateTime.Today.AddDays(-30);
            TipoRelatorio = TipoRelatorio.Consolidado;
            FormatoExportacao = FormatoExportacao.PDF;
        }
    }

    public enum TipoRelatorio
    {
        [Display(Name = "Pressão Arterial")]
        Pressao = 1,
        [Display(Name = "Glicose")]
        Glicose = 2,
        [Display(Name = "Peso e IMC")]
        Peso = 3,
        [Display(Name = "Relatório Consolidado")]
        Consolidado = 4
    }

    public enum FormatoExportacao
    {
        [Display(Name = "PDF")]
        PDF = 1,
        [Display(Name = "Excel")]
        Excel = 2
    }
}