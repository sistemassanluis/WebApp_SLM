using System.ComponentModel.DataAnnotations;

namespace WebApp_SLM.Models.HorasExtras
{
    public class FilterControl
    {
        [DataType(DataType.Date)]
        public DateTime fechaInicioRango { get; set; } = DateTime.Parse(DateTime.Now.AddDays(-30).ToString("dd-MM-yyyy"));

        [DataType(DataType.Date)]
        public DateTime fechaFinRango { get; set; } = DateTime.Parse(DateTime.Now.ToString("dd-MM-yyyy"));
        public long idPersonalFind { get; set; }
        public string nombreFind { get; set; }
    }
}
