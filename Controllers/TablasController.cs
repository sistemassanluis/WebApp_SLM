using Microsoft.AspNetCore.Mvc;
using WebApp_SLM.Services;

namespace WebApp_SLM.Controllers
{
    public class TablasController : Controller
    {
        private readonly ITablasRepository repo;

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> listaTablasItem(long id)
        {
            var lista = await repo.TablasFindById(id);
            return View(lista);
        }
    }
}
