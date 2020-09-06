using Microsoft.AspNetCore.Mvc;

namespace Xu.WebMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}