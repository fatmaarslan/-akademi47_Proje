using Microsoft.AspNetCore.Mvc;

namespace İakademi47_Proje.Controllers
{
    public class WebServiseController : Controller
    {

        public static string tckimlikno = "";
        public static string vergino = "";

  
        public IActionResult Index()
        {
            return View();
        }
    }
}
