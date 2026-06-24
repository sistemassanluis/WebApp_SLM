using WebApp_SLM.Models.HorasExtras;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace WebApp_SLM.Services
{
    
    public class GeneraPdfService : IGeneraPdfService
    {
        public readonly string _fechaInicio;
        public readonly string _fechaFin;
        public readonly string _nombrePersonal;
        IEnumerable<ListaTiempoExtra> _listaTiempoExtra;

        public GeneraPdfService(string fechaInicio, string fechaFin, string nombrePersonal, IEnumerable<ListaTiempoExtra> listaTiempoExtra)
        {
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
            _nombrePersonal = nombrePersonal;
            _listaTiempoExtra = listaTiempoExtra;
        }

        public byte[] GenerarPDFValidacion()
        {
            

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(14));

                    page.Header().Element(buildHeader);
                   
                });
            });

            using var stream = new MemoryStream();
            documento.GeneratePdf(stream);
            return stream.ToArray();

        }

        private void buildHeader(IContainer contenedor)
        {
            DateTime fechaHoy = DateTime.Now;
            contenedor.Row(fila =>
            {
                fila.RelativeItem().Column(columna =>
                {
                    columna.Item().Height(5, Unit.Millimetre).AlignRight().Text($"Fecha hora de Impresión:{fechaHoy.ToString("dd-MM-yyyy HH:mm:ss")}").FontSize(8);
                    columna.Item().Text("");
                    columna.Item().Height(1, Unit.Centimetre).AlignCenter().Text("LISTA DE EVENTOS HORA EXTRA").FontSize(14);
                    columna.Item().Height(5, Unit.Millimetre).AlignCenter().Text($"Rango de Fechas: {_fechaInicio} - {_fechaFin}   Personal: {_nombrePersonal} .").FontSize(10);
                });
            });

        }
    }
}
