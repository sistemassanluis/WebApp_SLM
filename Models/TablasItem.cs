namespace WebApp_SLM.Models
{
    public class TablasItem
    {
        public long idTabla {  get; set; }
        public string descripcionTabla { get; set; }
        public long idSubtabla { get; set;}
        public string descripcion {  get; set; }
        public string descripcion2{ get; set; }
        public string abreviado { get; set; }
        public string codigo { get; set; }

        public TablasItem(long idTabla, string descripcionTabla, long idSubtabla, string descripcion, string descripcion2, string abreviado, string codigo)
        {
            this.idTabla = idTabla;
            this.descripcionTabla = descripcionTabla;
            this.idSubtabla = idSubtabla;
            this.descripcion = descripcion;
            this.descripcion2 = descripcion2;
            this.abreviado = abreviado;
            this.codigo = codigo;
        }
    }

}
