using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Collections.Immutable;
using System.Data;
using WebApp_SLM.Models;
using WebApp_SLM.Models.HorasExtras;
using WebApp_SLM.Services;

namespace WebApp_SLM.Controllers.TiempoExtra
{
    public class TiempoExtraController : Controller
    {
        private readonly ITiempoExtraRepository repo;
        private readonly ITablasRepository repoT;

        DateTime fechaInicio = DateTime.Now.AddDays(-30);//.ToString("yyyy-MM-dd");
        DateTime fechaActual = DateTime.Now;//.ToString("yyyy-MM-dd");
        long idFiltro = -1;
        string nombreLabelFiltro = "";
        string estadoCrud = "NEW";

        public TiempoExtraController(ITiempoExtraRepository rep, ITablasRepository repT)
        {
            this.repo = rep;  
            this.repoT = repT;
        }

        public async Task<ActionResult> TiempoExtra()
        {
            
            OverTime overTime = new OverTime();
            DateTime fI = fechaInicio;
            DateTime fF = fechaActual;
            long idFilter = idFiltro;


            IEnumerable<ListaTiempoExtra> listaTiempoExtras= await repo.ListarHorasExtras(fI, fF, idFilter);

            ViewTiempoExtra vte = new ViewTiempoExtra()
            {
                myOverTime = overTime,
                myListaTiempoExtras = listaTiempoExtras,
                myFilterControl = new FilterControl
                {
                    fechaInicioRango = fI,
                    fechaFinRango = fF,
                    idPersonalFind = idFilter,
                    nombreFind = nombreLabelFiltro

                },
                estado = "NEW"
            };
            return View(vte);
        }

        public async Task<IActionResult> Index(DateTime fechaini, DateTime fechafin, long personalFind, string nombreFind)
        {
            fechaInicio = fechaini;
            fechaActual = fechafin;
            idFiltro = personalFind;
            nombreLabelFiltro = nombreFind;

            if (personalFind == 0)
            {
                idFiltro = -1;
            } 

            var listaTiempoExtras = await repo.ListarHorasExtras(fechaini, fechafin, idFiltro);

            ViewTiempoExtra vte = new ViewTiempoExtra()


            {
                myOverTime = new OverTime(),
                myListaTiempoExtras = listaTiempoExtras,
                myFilterControl = new FilterControl
                {
                    fechaInicioRango = fechaInicio,
                    fechaFinRango = fechaActual,
                    idPersonalFind = idFiltro,
                    nombreFind=nombreLabelFiltro
                    
                },
                estado = "NEW"
            };
            return View("TiempoExtra",vte);
        }

        [HttpGet]
        public JsonResult searchPersonal(string texto)
        {
            
            List<FindPersonal> lista = new List<FindPersonal>();
            lista = repo.SeachPersonal(texto);
            return Json( lista); 
        }

        public void estadoEdit()
        {
            estadoCrud = "EDIT";
        }

        public void estadoNew()
        {
            estadoCrud = "NEW";
        }

        [HttpGet]
        public async Task<ActionResult> CrudHoraExtraDelete(long idOvertime)
        {
            
            estadoCrud = "DELETE";
            List<MiListaType> lista = new List<MiListaType>();
            OverTime itemOvertime = new OverTime();
            itemOvertime.id = idOvertime;
            itemOvertime.date_add = DateTime.Now;
            itemOvertime.date_modify = DateTime.Now;
            itemOvertime.dia_hora_fin = DateTime.Now;
            itemOvertime.dia_hora_inicio = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return View(itemOvertime);
            }
            await repo.Crud_HoraExtra(itemOvertime, lista, "D");
            estadoCrud = "NEW";
            return RedirectToAction("TiempoExtra");
        }

        [HttpPost]
        public async Task<ActionResult> CrudHoraExtra(ViewTiempoExtra overtime)
        {
            overtime.myOverTime.id_user_add = 1;
            overtime.myOverTime.observacion = overtime.myOverTime.observacion.Trim().Length > 0 ? overtime.myOverTime.observacion.Trim() : "Sin Observación";

            List<MiListaType> lista = new List<MiListaType>();
            foreach (var item in overtime.myOverTime.motivos)
            {
                MiListaType newItem = new MiListaType(item, "", "");
                lista.Add(newItem);
            }

            if (!ModelState.IsValid)
            {
                return View(overtime);
            }
            await repo.Crud_HoraExtra(overtime.myOverTime, lista, "I");
            return RedirectToAction("TiempoExtra");
        }

        


        [HttpGet]
        public async Task<ActionResult> ListarHorasExtras(string fechaini, string fechafin, long personal)
        {

            DateTime fI = DateTime.Parse(fechaini);
            DateTime fF = DateTime.Parse(fechafin);

            IEnumerable<ListaTiempoExtra> listaTiempoExtras = await repo.ListarHorasExtras(fI, fF, personal);
            return Ok(listaTiempoExtras);
        }

        [HttpGet]
        public async Task<ActionResult> ViewHorarioPersonal(long id_personal)
        {
            var lista = await repo.ViewHorarioPersonal(id_personal);
            return Ok(lista);
        }

        public IActionResult ExcelReporte()
        {
            return View();
        }

        [HttpGet]
        public async Task<FileResult> ExportarListaOverTimeFiltro(DateTime fechaini, DateTime fechafin, long personalFind)
        {
            const String formatoFecha = "yyyyMMdd";

            var listaTiempoExtras = await repo.ListarHorasExtras(fechaInicio, fechaActual, personalFind);            
            var nombreArchivo = $"Listado_horas_extras_{fechaInicio.ToString(formatoFecha)} - {fechaActual.ToString(formatoFecha)}.xlsx";
            return GeneraExcel(nombreArchivo, listaTiempoExtras);
        }

        public FileResult GeneraExcel(string nombreArchivo, IEnumerable<ListaTiempoExtra> listaTiempoExtras)
        {
            DataTable dataTable = new DataTable("ListaTiempoExtra");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Nombre Completo"),
                new DataColumn("Horario"),
                new DataColumn("Inicio H.Extra"),
                new DataColumn("Fin H.Extra"),
                new DataColumn("Horas extras"),
                new DataColumn("Minutos Extras"),
                new DataColumn("Motivo"),
                new DataColumn("Observacion"),
                new DataColumn("Fecha Ingreso")
            });

            foreach (var item in listaTiempoExtras)
            {
                dataTable.Rows.Add(
                    item.nombre_completo,
                    item.horario_dia,
                    item.dia_hora_inicio,
                    item.dia_hora_fin,
                    item.horas_extra,
                    item.minutos_extra,
                    item.motivos,
                    item.observacion,
                    item.date_add
                );
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.scheet", nombreArchivo);
                }
            }
        }

    }
}
 