using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace pharmacy.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class QuestionerController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IPatientRepository PatientRepository;
        private readonly IProductRepository productRepository;

        public QuestionerController(IPatientRepository PatientRepository, IProductRepository productRepository , UserManager<IdentityUser> userManager)
        {

            this.PatientRepository = PatientRepository;
            this.productRepository = productRepository;
            this.userManager = userManager;
        }
        [HttpGet]
        public IActionResult PatientHistory()
        {

            return View();
        }
    }
}
