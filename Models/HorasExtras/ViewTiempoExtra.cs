namespace WebApp_SLM.Models.HorasExtras
{
    public class ViewTiempoExtra
    {
        public OverTime myOverTime { get; set; }
        public IEnumerable<ListaTiempoExtra> myListaTiempoExtras { get; set; }
        public FilterControl myFilterControl { get; set; }

        public string estado { get; set; }

    }
}
