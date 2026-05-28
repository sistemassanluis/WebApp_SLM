namespace WebApp_SLM.Models.HorasExtras
{
    public class HorarioPersonal
    {
        public long id {  get; set; }
        public long personal_id { get; set; }
        public int dia { get; set; }
        public string dia_label { get; set; }
        public Boolean turno_cruzado { get; set; }
        public TimeSpan hora_ingreso { get; set; }
        public TimeSpan hora_salida { get; set; }
        public TimeSpan hora_refr_inicio { get; set; }
        public TimeSpan hora_refr_fin { get; set; }
        public DateTime date_add { get; set; }
        public long id_user_add {  get; set; }
        public DateTime date_modify { get; set; }
        public long id_user_modify { get; set; }


    }
}
