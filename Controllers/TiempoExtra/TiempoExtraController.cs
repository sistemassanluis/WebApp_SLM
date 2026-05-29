using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp_SLM.Models;
using WebApp_SLM.Models.HorasExtras;
using WebApp_SLM.Services;

namespace WebApp_SLM.Controllers.TiempoExtra
{
    public class TiempoExtraController : Controller
    {
        private readonly ITiempoExtraRepository repo;
        private readonly ITablasRepository repoT;

        public TiempoExtraController(ITiempoExtraRepository rep, ITablasRepository repT)
        {
            this.repo = rep;  
            this.repoT = repT;
        }
        public async Task<ActionResult> TiempoExtra()
        {
            OverTime overTime = new OverTime();

            DateTime fI = DateTime.Parse("2026-01-01 00:00:00");
            DateTime fF = DateTime.Parse("2026-12-01 00:00:00");
            IEnumerable<ListaTiempoExtra> listaTiempoExtras= await repo.ListarHorasExtras(fI, fF, -1);

            ViewTiempoExtra vte = new ViewTiempoExtra()
            {
                myOverTime = overTime,
                myListaTiempoExtras = listaTiempoExtras
            };
            return View(vte);
        }

        [HttpGet]
        public JsonResult searchPersonal(string texto)
        {
            
            List<FindPersonal> lista = new List<FindPersonal>();
            lista = repo.SeachPersonal(texto);
            return Json( lista); 
        }
        [HttpPost]
        public async Task<ActionResult> CrearHoraExtra(OverTime overtime)
        {
            overtime.id_user_add = 1;
            overtime.observacion = overtime.observacion.Trim().Length > 0 ? overtime.observacion.Trim() : "Sin Observación";

            List<MiListaType> lista = new List<MiListaType>();
            foreach (var item in overtime.motivos)
            {
                MiListaType newItem = new MiListaType(item, "", "");
                lista.Add(newItem);
            }

            if (!ModelState.IsValid)
            {
                return View(overtime);
            }
            await repo.Crud_HoraExtra(overtime, lista, "I");
            return RedirectToAction("TiempoExtra");
        }

        [HttpGet]
        public async Task<ActionResult> ListarHorasExtras(string fechaini, string fechafin, long personal)
        {
            DateTime fI = DateTime.Parse(fechaini);
            DateTime fF = DateTime.Parse(fechafin);
            var lista = await repo.ListarHorasExtras(fI, fF, personal);
            return Ok(lista);
        }

        [HttpGet]
        public async Task<ActionResult> ViewHorarioPersonal(long id_personal)
        {
            var lista = await repo.ViewHorarioPersonal(id_personal);
            return Ok(lista);
        }




    }
}
