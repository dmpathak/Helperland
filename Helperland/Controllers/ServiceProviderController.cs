using Microsoft.AspNetCore.Mvc;

namespace Helperland.Controllers
{
    public class ServiceProviderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
