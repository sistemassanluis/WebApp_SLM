namespace WebApp_SLM.Models.HorasExtras
{
    public class Personal
    {
        public long id { get; set; }
        public long personal_id { get; set; }
        public int area { get; set; }
        public int puesto { get; set; }
        public DateTime date_add { get; set; }
        public long id_user_add { get; set; }
        public DateTime date_modify { get; set; }
        public long id_user_modify { get; set; }
    }
}
