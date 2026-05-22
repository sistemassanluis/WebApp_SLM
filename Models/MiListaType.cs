namespace WebApp_SLM.Models
{
    public class MiListaType
    {
        public long Valorid {  get; set; }
        public string valor2 { get; set;}
        public string valor3 { get; set; }

        public MiListaType(long valorid, string valor2, string valor3)
        {
            Valorid = valorid;
            this.valor2 = valor2;
            this.valor3 = valor3;
        }
    }
}
