using WebApp_SLM.Models.HorasExtras;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace WebApp_SLM.Services
{
    public class GeneraPdfService : IGeneraPdfService
    {
        public byte[] GenerarPDFValidacion(ListaTiempoExtra lista, DateTime fechaIni, DateTime fechaFin, long idPersonal, string nombrePersonal )
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header()
                        .Text("Lista de Horas Extras")
                        .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text(Placeholders.LoremIpsum());
                            x.Item().Image(Placeholders.Image(200, 100));
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            using var stream = new MemoryStream();
            documento.GeneratePdf(stream);
            return stream.ToArray();

        }
    }
}
