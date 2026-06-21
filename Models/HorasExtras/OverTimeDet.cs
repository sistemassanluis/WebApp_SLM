using System.ComponentModel.DataAnnotations;

namespace WebApp_SLM.Models.HorasExtras
{
    public class OverTimeDet
    {
        public long id { get; set; }
        public long personal_id { get; set; }
        public string nombre_completo { get; set; }
        public string horario_dia { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime dia_hora_inicio { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime dia_hora_fin { get; set; }
        public int horas_extra { get; set; }
        public int minutos_extra { get; set; }
        public List<OverTimeReasonDet> motivos { get; set; } = [];
        public string observacion { get; set; }
        public DateTime date_add { get; set; }
        public long id_user_add { get; set; }
        public DateTime date_modify { get; set; }
        public long id_user_modify { get; set; }

        
        public OverTimeDet(long id, long personal_id, string nombre_completo, DateTime dia_hora_inicio, DateTime dia_hora_fin, int horas_extra, int minutos_extra, string observacion, DateTime date_add, long id_user_add, DateTime date_modify, long id_user_modify, string horario_dia)
        {
            this.id = id;
            this.personal_id = personal_id;
            this.nombre_completo = nombre_completo;
            this.horario_dia = horario_dia;
            this.dia_hora_inicio = dia_hora_inicio;
            this.dia_hora_fin = dia_hora_fin;
            this.horas_extra = horas_extra;
            this.minutos_extra = minutos_extra;
            this.observacion = observacion;
            this.date_add = date_add;
            this.id_user_add = id_user_add;
            this.date_modify = date_modify;
            this.id_user_modify = id_user_modify;
        }
    }
}
