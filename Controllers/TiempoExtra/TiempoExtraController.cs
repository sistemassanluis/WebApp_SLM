using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using System.Collections;
using System.Reflection.Metadata;
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
        public ActionResult TiempoExtra()
        {
            return View();
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
            overtime.observacion = overtime.observacion.Trim().Length < 1 ? overtime.observacion : "Sin Observación";

            List<MiListaType> lista = new List<MiListaType>();
            int ind = 0;
            foreach (var item in overtime.motivos)
            {
                lista.Add(
                
                    Valorid = item,
                    valor2 = "",
                    valor3 = ""
                );
            }


            if (!ModelState.IsValid)
            {
                return View(overtime);
            }
            await repo.Crud_HoraExtra(overtime, null, "I");
            return RedirectToAction("index");
        }

       

    }
}
