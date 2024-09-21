using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using  Models;
using System.Diagnostics;

namespace pharmacy.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IPatientRepository PatientRepository;
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(ICategoryRepository _categoryRepository,IPatientRepository PatientRepository, IProductRepository productRepository, UserManager<IdentityUser> userManager)
        {

            this.PatientRepository = PatientRepository;
            this.productRepository = productRepository;
            this.userManager = userManager;
            this._categoryRepository = _categoryRepository;
        }
        [HttpGet]

        public IActionResult Index()
        {
            var result = _categoryRepository.Get(null);

            var userId = userManager.GetUserId(User);
            var patient = PatientRepository.Get(e => e.ApplicationUserId == userId).FirstOrDefault();

            // Initialize the recommendedProducts collection
            List<Product> recommendedProducts = new List<Product>();

            if (patient != null)
            {
                // Add Alzheimer's products if applicable
                if (patient.IsCough == true)
                {
                    var alzheimerProducts = productRepository.Get(e => e.CategoryID == 5, e => e.Category).Take(2);
                    recommendedProducts.AddRange(alzheimerProducts);
                }

                // Add Antiallergic products if applicable
                if (patient.IsAntiallergic == true)
                {
                    var antiallergicProducts = productRepository.Get(e => e.CategoryID == 2, e => e.Category).Take(2);
                    recommendedProducts.AddRange(antiallergicProducts);
                }
                if (patient.IsAntidepressants == true)
                {
                    var Antidepressants = productRepository.Get(e => e.CategoryID == 3, e => e.Category).Take(2);
                    recommendedProducts.AddRange(Antidepressants);
                }
                if (patient.IsInfluenza == true)
                {
                    var Influenza = productRepository.Get(e => e.CategoryID == 7, e => e.Category).Take(2);
                    recommendedProducts.AddRange(Influenza);
                }
            }

            // Ensure distinct products in case of overlap between categories (optional)
            recommendedProducts = recommendedProducts.Distinct().ToList();

            // Create the ViewModel
            var viewModel = new HomeViewModel
            {
                Categories = result,
                RecommendedProducts = recommendedProducts
            };

            return View(viewModel);
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

        //public IActionResult PatientRecomend()
        //{
        //    var userId = userManager.GetUserId(User);
        //    var patient = PatientRepository.Get(e => e.ApplicationUserId == userId).FirstOrDefault();

        //    if (patient != null && patient.IsAlzehimer == true)
        //    {
        //        productRepository.Get(e => e.CategoryID == 1, e => e.Category).Take(3);
        //    }
        //    if (patient != null && patient.IsAntiallergic == true)
        //    {
        //        productRepository.Get(e => e.CategoryID == 2, e => e.Category).Take(3);
        //    }
        //    return View();

        //}






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
