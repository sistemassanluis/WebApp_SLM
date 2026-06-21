namespace WebApp_SLM.Models.HorasExtras
{
    public class OverTimeReasonDet
    {
        public long id { get; set; }
        public long overtime_id { get; set; }
        public int motivo_id { get; set; }
        public string motivo_descripcion { get; set; }
        public DateTime date_add { get; set; }
        public long id_user_add { get; set; }
        public DateTime date_modify { get; set; }
        public long id_user_modify { get; set; }
    }
}
