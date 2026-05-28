using System.ComponentModel.DataAnnotations;

namespace WebApp_SLM.Models.HorasExtras
{
    public class ListaTiempoExtra
    {
        public long id {  get; set; }
        public long personal_id { get; set; }
        public string nombre_completo { get; set; }
        public string horario_dia { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime dia_hora_inicio { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime dia_hora_fin {  get; set; }
        public int horas_extra { get; set; }
        public int minutos_extra {  get; set; }
        public string motivos {  get; set; }
        public string observacion {  get; set; }
        public DateTime date_add { get; set; }
        public long id_user_add { get; set; }
        public DateTime date_modify { get; set; }
        public long id_user_modify { get; set; }
    }
}
