using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using  Models;
using System.Diagnostics;

namespace pharmacy.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository productRepository;
        public HomeController(ILogger<HomeController> logger, ICategoryRepository _categoryRepository, IProductRepository productRepository)
        {
            _logger = logger;
            this._categoryRepository = _categoryRepository;
            this.productRepository = productRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var result = _categoryRepository.Get(null);

            return View(result);
        }
        [HttpGet]
        public IActionResult Search(string Name)
        {
            var res = productRepository.Get(e => e.ProductName.Contains(Name), e => e.Category);
            //var res = context.Movies.Include(e => e.Categories).Include(e => e.Cinemas).Where(e => e.Name.Contains(Name)).ToList();
            if (!res.Any())
            {
                return View("NotFound");
            }
            else
            {
                return View("Search", res);
            }
        }
        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
