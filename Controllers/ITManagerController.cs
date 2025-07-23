using Microsoft.AspNetCore.Mvc;

namespace QontrolSystem.Controllers
{
    public class ITManagerController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
