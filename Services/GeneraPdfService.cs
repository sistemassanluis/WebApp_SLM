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
        public IEnumerable<ListaTiempoExtra> _listaTiempoExtra;
        public IEnumerable<ListarTiempoExtraConsolidado> _listaTiempoExtraConsol;

        public GeneraPdfService(string fechaInicio, string fechaFin, string nombrePersonal, IEnumerable<ListaTiempoExtra> listaTiempoExtra)
        {
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
            _nombrePersonal = nombrePersonal;
            _listaTiempoExtra = listaTiempoExtra;
        }

        public GeneraPdfService(string fechaInicio, string fechaFin, string nombrePersonal, IEnumerable<ListarTiempoExtraConsolidado> listaTiempoExtraConsol)
        {
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
            _nombrePersonal = nombrePersonal;
            _listaTiempoExtraConsol = listaTiempoExtraConsol;
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
                    page.Content().Element(builderContent);
                    page.Footer().Element(builderFooter);
                   
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
                    columna.Item().Height(5, Unit.Millimetre).AlignRight().Text($"Fecha hora de Impresión:{fechaHoy.ToString("dd-MM-yyyy HH:mm:ss")}").FontSize(8).Italic().FontColor(Colors.Grey.Medium);
                    
                    columna.Item().Height(1, Unit.Centimetre).AlignCenter().Text("LISTA DE EVENTOS HORA EXTRA").FontSize(14).Bold();
                    columna.Item().Height(5, Unit.Millimetre).AlignCenter().Text($"Rango de Fechas: {_fechaInicio} - {_fechaFin}   Personal: {_nombrePersonal} .").FontSize(8).SemiBold();
                });
            });

        }

        private void builderContent(IContainer contenedor)
        {
            contenedor.PaddingVertical(12).Column(columna =>
            {
                columna.Item().Element(construirTabla);
            });
        }

        private void construirTabla(IContainer contenedor)
        {
            contenedor.Table(tabla =>
            {
                tabla.ColumnsDefinition(columna =>
                {
                    columna.ConstantColumn(7,Unit.Millimetre);
                    columna.RelativeColumn(2);
                    columna.RelativeColumn();
                    columna.ConstantColumn(2, Unit.Centimetre);
                    columna.RelativeColumn();
                    columna.RelativeColumn();
                    columna.RelativeColumn();
                    columna.ConstantColumn(3,Unit.Centimetre);

                });
                tabla.Header(cabecera =>
                {
                    cabecera.Cell().Element(EstiloCelda).Text("#");
                    cabecera.Cell().Element(EstiloCelda).Text("PERSONAL");
                    cabecera.Cell().Element(EstiloCelda).Text("HORARIO");
                    cabecera.Cell().Element(EstiloCelda).Text("INICIO - FIN H. EXTRA");
                    cabecera.Cell().Element(EstiloCelda).Text("TIEMPO EXTRA");
                    cabecera.Cell().Element(EstiloCelda).Text("MOTIVO");
                    cabecera.Cell().Element(EstiloCelda).Text("OBSERVACION");                    
                    cabecera.Cell().Element(EstiloCelda).AlignRight().Text("FIRMA");

                    static IContainer EstiloCelda(IContainer contenedor)
                    {
                        return contenedor.DefaultTextStyle(x => x.SemiBold().FontSize(7)).PaddingVertical(5).BorderBottom(1, Unit.Point).BorderTop(1, Unit.Point).BorderColor(Colors.Black);
                    }
                });

                int index = 0;
                float sumHora = 0;
                float sumMinutos = 0;
                float totalHoras = 0;
                float totalMinutos = 0;
                foreach (var item in _listaTiempoExtra)
                {
                    index++;
                    sumHora += item.horas_extra;
                    sumMinutos += item.minutos_extra;

                    tabla.Cell().Element(EstiloCeldaLista).AlignLeft().Text(index.ToString());
                    tabla.Cell().Element(EstiloCeldaLista).Text(item.nombre_completo);
                    tabla.Cell().Element(EstiloCeldaLista).Text(item.horario_dia);
                    tabla.Cell().Element(EstiloCeldaLista).Text($"{item.dia_hora_inicio.ToString("dd-MM-yy HH:mm")} / {item.dia_hora_fin.ToString("dd-MM-yy HH:mm")}");
                    tabla.Cell().Element(EstiloCeldaLista).AlignCenter().Text($"<{ item.horas_extra}h-{item.minutos_extra}m>");
                    tabla.Cell().Element(EstiloCeldaLista).Text(item.motivos);
                    tabla.Cell().Element(EstiloCeldaLista).Text(item.observacion);
                    tabla.Cell().Element(EstiloCeldaLista).Text("|                          |").AlignRight();

                    static IContainer EstiloCeldaLista(IContainer contenedor)
                    {
                        return contenedor.DefaultTextStyle(x => x.FontSize(7)).BorderBottom(0.25f,Unit.Point).BorderColor(Colors.Grey.Medium).PaddingVertical(1);
                    }

                }
                //totales
                int parteEntera = (int)Math.Truncate(sumMinutos / 60);
                totalHoras = (sumHora) + (parteEntera);
                totalMinutos = sumMinutos - (60 * parteEntera);

                tabla.Cell().Element(EstiloCeldaTotales).Text("");
                tabla.Cell().Element(EstiloCeldaTotales).Text("");
                tabla.Cell().Element(EstiloCeldaTotales).Text("");
                tabla.Cell().Element(EstiloCeldaTotales).Text("TOTAL:");
                tabla.Cell().Element(EstiloCeldaTotales).Text($"<{totalHoras}h-{totalMinutos}m>");
                tabla.Cell().Element(EstiloCeldaTotales).Text("");
                tabla.Cell().Element(EstiloCeldaTotales).Text("");
                tabla.Cell().Element(EstiloCeldaTotales).Text("");

                static IContainer EstiloCeldaTotales(IContainer contenedor)
                {
                    return contenedor.DefaultTextStyle(x => x.FontSize(8).Bold()).BorderTop(1).BorderColor(Colors.Black).AlignCenter().PaddingVertical(1);
                }

            });
        }
        private void builderFooter(IContainer contenedor)
        {
            contenedor.Element(EstiloCeldaFooter).AlignRight().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });

            static IContainer EstiloCeldaFooter(IContainer contenedor)            {
                return contenedor.DefaultTextStyle(x => x.FontSize(7).FontColor(Colors.Grey.Medium)).PaddingVertical(1).AlignRight().BorderTop(0.5f,Unit.Point).BorderColor(Colors.Grey.Medium).Width(10,Unit.Centimetre);
            }
        }

        public byte[] GenerarPDFValidacionConsolidado()
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(14));

                    page.Header().Element(buildHeaderConsol);
                    page.Content().Element(builderContentConsol);
                    page.Footer().Element(builderFooterConsol);

                });
            });

            using var stream = new MemoryStream();
            documento.GeneratePdf(stream);
            return stream.ToArray();
        }

        private void buildHeaderConsol(IContainer contenedor)
        {
            DateTime fechaHoy = DateTime.Now;
            contenedor.Row(fila =>
            {
                fila.RelativeItem().Column(columna =>
                {
                    columna.Item().Height(5, Unit.Millimetre).AlignRight().Text($"Fecha hora de Impresión:{fechaHoy.ToString("dd-MM-yyyy HH:mm:ss")}").FontSize(8).Italic().FontColor(Colors.Grey.Medium);

                    columna.Item().Height(1, Unit.Centimetre).AlignCenter().Text("LISTA DE EVENTOS HORA EXTRA CONSOLIDADO").FontSize(14).Bold();
                    columna.Item().Height(5, Unit.Millimetre).AlignCenter().Text($"Rango de Fechas: {_fechaInicio} - {_fechaFin}").FontSize(8).SemiBold();
                });
            });

        }

        private void builderContentConsol(IContainer contenedor)
        {
            contenedor.PaddingVertical(12).Column(columna =>
            {
                columna.Item().Element(construirTablaConsol);
            });
        }

        private void construirTablaConsol(IContainer contenedor)
        {
            contenedor.Table(tabla =>
            {
                tabla.ColumnsDefinition(columna =>
                {
                    columna.ConstantColumn(7, Unit.Millimetre);
                    columna.RelativeColumn(2);
                    columna.RelativeColumn();
                    columna.RelativeColumn();
                    

                });
                tabla.Header(cabecera =>
                {
                    cabecera.Cell().Element(EstiloCelda).Text("#");
                    cabecera.Cell().Element(EstiloCelda).Text("PERSONAL");
                    cabecera.Cell().Element(EstiloCelda).Text("TOTAL HORAS EXTRAS");
                    cabecera.Cell().Element(EstiloCelda).Text("TOTAL MINUTOS EXTRAS");
                    
                    static IContainer EstiloCelda(IContainer contenedor)
                    {
                        return contenedor.DefaultTextStyle(x => x.SemiBold().FontSize(9)).PaddingVertical(5).BorderBottom(1, Unit.Point).BorderTop(1, Unit.Point).BorderColor(Colors.Black);
                    }
                });

                int index = 0;
                float sumHora = 0;
                float sumMinutos = 0;
                float totalHoras = 0;
                float totalMinutos = 0;
                foreach (var item in _listaTiempoExtraConsol)
                {
                    index++;
                    sumHora += item.total_horas;
                    sumMinutos += item.total_minutos;

                    tabla.Cell().Element(EstiloCeldaLista).AlignLeft().Text(index.ToString());
                    tabla.Cell().Element(EstiloCeldaLista).AlignLeft().Text(item.nombre_completo);
                    tabla.Cell().Element(EstiloCeldaLista).AlignCenter().Text(item.total_horas.ToString());
                    tabla.Cell().Element(EstiloCeldaLista).AlignCenter().Text(item.total_minutos.ToString());

                    static IContainer EstiloCeldaLista(IContainer contenedor)
                    {
                        return contenedor.DefaultTextStyle(x => x.FontSize(9)).BorderBottom(0.25f, Unit.Point).BorderColor(Colors.Grey.Medium).PaddingVertical(5);
                    }

                }
                //totales
                int parteEntera = (int)Math.Truncate(sumMinutos / 60);
                totalHoras = (sumHora) + (parteEntera);
                totalMinutos = sumMinutos - (60 * parteEntera);

                tabla.Cell().Element(EstiloCeldaTotales).Text("");
                tabla.Cell().Element(EstiloCeldaTotales).Text("TOTAL:").AlignRight();
                tabla.Cell().Element(EstiloCeldaTotales).Text($"{totalHoras} h").AlignCenter();
                tabla.Cell().Element(EstiloCeldaTotales).Text($"{totalMinutos} m").AlignCenter();

                static IContainer EstiloCeldaTotales(IContainer contenedor)
                {
                    return contenedor.DefaultTextStyle(x => x.FontSize(8).Bold()).BorderTop(1).BorderColor(Colors.Black).PaddingVertical(5);
                }

            });
        }

        private void builderFooterConsol(IContainer contenedor)
        {
            contenedor.Element(EstiloCeldaFooter).AlignRight().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });

            static IContainer EstiloCeldaFooter(IContainer contenedor)
            {
                return contenedor.DefaultTextStyle(x => x.FontSize(7).FontColor(Colors.Grey.Medium)).PaddingVertical(1).AlignRight().BorderTop(0.5f, Unit.Point).BorderColor(Colors.Grey.Medium).Width(10, Unit.Centimetre);
            }
        }

    }
}
