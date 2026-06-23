using WebApp_SLM.Models.HorasExtras;

namespace WebApp_SLM.Services
{
    public interface IGeneraPdfService
    {
        byte[] GenerarPDFValidacion(ListaTiempoExtra lista, DateTime fechaIni, DateTime fechaFin, long idPersonal, string nombrePersonal);
    }
}
