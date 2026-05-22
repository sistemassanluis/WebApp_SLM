using Microsoft.AspNetCore.Mvc;
using WebApp_SLM.Models;
using WebApp_SLM.Services;

namespace WebApp_SLM.Controllers
{
    public class TablasController : Controller
    {
        private readonly ITablasRepository repo;

        public TablasController(ITablasRepository repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IEnumerable<TablasItem>> listaTablasItem(long id)
        {
            var lista = await repo.TablasFindById(id);
            return lista.Select(x => new TablasItem(x.idTabla, x.descripcionTabla, x.idSubtabla, x.descripcion, x.descripcion2, x.abreviado, x.codigo));
        }

        [HttpPost]
        public async Task<ActionResult> listarItemTablas(long id)
        {
            var lista = await listaTablasItem(id);
            return Ok(lista);
        }


    }
}
