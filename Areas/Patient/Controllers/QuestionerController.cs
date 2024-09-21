using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        [HttpPost]
        public IActionResult PatientHistory(Models.Patient patients)
        {
            var userId = userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                patients.ApplicationUserId = userId;
                patients.CreatedDate = DateTime.Now;    
                PatientRepository.Add(patients);
                PatientRepository.commit();
               
            }
            return RedirectToAction("Index" ,"Home");
        }

        public IActionResult PatientProfile()
        {
            var userId = userManager.GetUserId(User);
            var res = PatientRepository.Get(e => e.ApplicationUserId == userId);
            return View(res);

        }

        //public IActionResult PatientRecomend()
        //{
        //    var userId = userManager.GetUserId(User);
        //    var patient = PatientRepository.Get(e => e.ApplicationUserId == userId).FirstOrDefault();

        //    if (patient != null && patient.IsAlzehimer == true)
        //    {
        //        productRepository.Get(e => e.CategoryID ==1, e => e.Category).Take(3);
        //    }
        //    if (patient != null && patient.IsAntiallergic == true)
        //    {
        //        productRepository.Get(e => e.CategoryID == 2, e => e.Category).Take(3);
        //    }
        //    return View();

        //}



    }
}
