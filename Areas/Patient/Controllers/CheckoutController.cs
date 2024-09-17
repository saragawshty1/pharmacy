using Microsoft.AspNetCore.Mvc;

namespace pharmacy.Controllers
{
    [Area("Patient")]
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
         
            public IActionResult success()
            {
                return View();
            }
            public IActionResult cancel()
            {
                return View();
            }
        }
}
