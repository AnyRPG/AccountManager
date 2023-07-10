using Microsoft.AspNetCore.Mvc;

namespace AccountManager.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
