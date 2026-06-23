using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
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

        private DateTime _fechaInicio = DateTime.Now.AddDays(-30);//.ToString("yyyy-MM-dd");
        private DateTime _fechaActual = DateTime.Now;//.ToString("yyyy-MM-dd");
        private long _idFiltro = -1;
        private string _nombreLabelFiltro = "";
        private string _estadoCrud = "NEW";

        ViewTiempoExtra vte = new ViewTiempoExtra()
        {
            myOverTime = null,
            myListaTiempoExtras = [],
            myFilterControl = new FilterControl
            {
                fechaInicioRango = DateTime.Now,
                fechaFinRango = DateTime.Now,
                idPersonalFind = 0,
                nombreFind = ""

            },
            estado = "NEW"
        };

        public TiempoExtraController(ITiempoExtraRepository rep, ITablasRepository repT)
        {
            this.repo = rep;  
            this.repoT = repT;
        }

        public async Task<ActionResult> TiempoExtra()
        {
            
            OverTime overTime = new OverTime();
            DateTime fI = _fechaInicio;
            DateTime fF = _fechaActual;
            long idFilter = _idFiltro;


            IEnumerable<ListaTiempoExtra> listaTiempoExtras= await repo.ListarHorasExtras(fI, fF, idFilter);

            vte.myOverTime = overTime;
            vte.myListaTiempoExtras = listaTiempoExtras;
            vte.myFilterControl.fechaInicioRango = fI;
            vte.myFilterControl.fechaFinRango = fF;
            vte.myFilterControl.idPersonalFind = idFilter;
            vte.myFilterControl.nombreFind = _nombreLabelFiltro;
            vte.estado = "NEW";

            return View(vte);           
            /*ViewTiempoExtra vte = new ViewTiempoExtra()
            {
                myOverTime = overTime,
                myListaTiempoExtras = listaTiempoExtras,
                myFilterControl = new FilterControl
                {
                    _fechaInicioRango = fI,
                    fechaFinRango = fF,
                    idPersonalFind = idFilter,
                    nombreFind = _nombreLabelFiltro

                },
                estado = "NEW"
            };*/
            
        }

        public async Task<IActionResult> Index(FilterControl filterControl)
        {
            _fechaInicio = filterControl.fechaInicioRango;
            _fechaActual = filterControl.fechaFinRango;
            _idFiltro = filterControl.idPersonalFind;
            _nombreLabelFiltro = filterControl.nombreFind;

            if (_idFiltro == 0)
            {
                _idFiltro = -1;
            } 

            var listaTiempoExtras = await repo.ListarHorasExtras(_fechaInicio, _fechaActual, _idFiltro);

            vte.myOverTime = new OverTime();
            vte.myListaTiempoExtras = listaTiempoExtras;
            vte.myFilterControl.fechaInicioRango = _fechaInicio;
            vte.myFilterControl.fechaFinRango = _fechaActual;
            vte.myFilterControl.idPersonalFind = _idFiltro;
            vte.myFilterControl.nombreFind = _nombreLabelFiltro;
            vte.estado = "NEW";

            return View("TiempoExtra",vte);

            /*ViewTiempoExtra vte = new ViewTiempoExtra()

            {
                myOverTime = new OverTime(),
                myListaTiempoExtras = listaTiempoExtras,
                myFilterControl = new FilterControl
                {
                    _fechaInicioRango = _fechaInicio,
                    fechaFinRango = _fechaActual,
                    idPersonalFind = _idFiltro,
                    nombreFind = _nombreLabelFiltro
                    
                },
                estado = "NEW"
            };*/
            
        }

        [HttpGet]
        public JsonResult searchPersonal(string texto)
        {
            
            List<FindPersonal> lista = new List<FindPersonal>();
            lista = repo.SeachPersonal(texto);
            return Json( lista); 
        }

        public JsonResult searchOverTime(long id)
        {
            List<OverTime> lista = new List<OverTime>();

            return Json( lista);
        }

        public void estadoEdit()
        {
            _estadoCrud = "EDIT";
        }

        public void estadoNew()
        {
            _estadoCrud = "NEW";
        }

        [HttpGet]
        public async Task<ActionResult> CrudHoraExtraDelete(long idOvertime)
        {
            
            _estadoCrud = "DELETE";
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
            _estadoCrud = "NEW";

            IEnumerable<ListaTiempoExtra> listaTiempoExtras = await repo.ListarHorasExtras(_fechaInicio, _fechaActual, _idFiltro);
            //return View("TiempoExtra");
            vte.myOverTime = new OverTime();
            vte.myListaTiempoExtras = listaTiempoExtras;
            vte.myFilterControl.fechaInicioRango = _fechaInicio;
            vte.myFilterControl.fechaFinRango = _fechaActual;
            vte.myFilterControl.idPersonalFind = _idFiltro;
            vte.myFilterControl.nombreFind = _nombreLabelFiltro;
            vte.estado = "NEW";

            return View("TiempoExtra", vte);
            //return RedirectToAction("TiempoExtra", vte);
        }

        public void CrudHoraExtraEdit()
        {
            _estadoCrud = "EDIT";

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
            string tipoProceso = (overtime.myOverTime.id == 0 ? "I": "U");

            IEnumerable<ListaTiempoExtra> listaTiempoExtras = await repo.ListarHorasExtras(_fechaInicio, _fechaActual, _idFiltro);
            await repo.Crud_HoraExtra(overtime.myOverTime, lista, tipoProceso);

            vte.myOverTime = new OverTime();
            vte.myListaTiempoExtras = listaTiempoExtras;
            vte.myFilterControl.fechaInicioRango = _fechaInicio;
            vte.myFilterControl.fechaFinRango = _fechaActual;
            vte.myFilterControl.idPersonalFind = _idFiltro;
            vte.myFilterControl.nombreFind = _nombreLabelFiltro;
            vte.estado = "NEW";

            return View("TiempoExtra", vte);
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

        [HttpGet]
        public async Task<ActionResult> SeachOverTime(long id)
        {
            OverTimeDet overtime = repo.SeachOverTime(id);
            return Ok(overtime);
        }

        public IActionResult ExcelReporte()
        {
            return View();
        }

        [HttpGet]
        public async Task<FileResult> ExportarListaOverTimeFiltro(string fechaIni, string fechaFin, long idPersonalFind)
        {
            const String formatoFecha = "yyyyMMdd";
            DateTime fI = DateTime.Parse(fechaIni);
            DateTime fF = DateTime.Parse(fechaFin);


            var listaTiempoExtras = await repo.ListarHorasExtras(fI, fF, idPersonalFind);            
            var nombreArchivo = $"Listado_horas_extras_{_fechaInicio.ToString(formatoFecha)} - {_fechaActual.ToString(formatoFecha)}.xlsx";
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
 