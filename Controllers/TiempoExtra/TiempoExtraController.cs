using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp_SLM.Models.HorasExtras;
using WebApp_SLM.Services;

namespace WebApp_SLM.Controllers.TiempoExtra
{
    public class TiempoExtraController : Controller
    {
         private readonly ITiempoExtraRepository repo;

        public TiempoExtraController(ITiempoExtraRepository rep)
        {
            this.repo = rep;  
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


    }
}
