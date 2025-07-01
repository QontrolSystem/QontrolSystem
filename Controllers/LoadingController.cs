using Microsoft.AspNetCore.Mvc;

namespace QontrolSystem.Controllers
{
    public class LoadingController : Controller
    {
        // Show loading screen, then redirect after duration
        public IActionResult Index(string? returnUrl = null, int duration = 3000, string? message = null)
        {
            var redirectUrl = !string.IsNullOrEmpty(returnUrl)
                ? returnUrl
                : Url.Action("Index", "Dashboard");

            ViewBag.LoadingConfig = new
            {
                RedirectUrl = redirectUrl,
                Duration = duration,
                Title = "Loading - QontrolSystem",
                Message = message ?? "Loading..."
            };

            return View();
        }
    }
}
