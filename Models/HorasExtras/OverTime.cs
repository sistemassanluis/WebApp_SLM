using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApp_SLM.Models.HorasExtras
{
    public class OverTime
    {
        public long id { get; set; }

        public long personal_id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        
        public DateTime dia_hora_inicio { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]

        public DateTime dia_hora_fin { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]

        public string? observacion { get; set; }
        public DateTime date_add { get; set; }
        public long id_user_add { get; set; }
        public DateTime date_modify { get; set; }
        public long id_user_modify { get; set; }
        public long[] motivos { get; set; } =[]; 
    }
}
