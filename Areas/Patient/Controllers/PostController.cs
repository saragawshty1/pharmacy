using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace pharmacy.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class PostController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IPatientRepository PatientRepository;
        private readonly IPostRepository PostRepository;


       public PostController(IPatientRepository PatientRepository,UserManager<IdentityUser> userManager , IPostRepository PostRepository)
        {

            this.PatientRepository = PatientRepository;
            this.userManager = userManager;
            this.PostRepository = PostRepository;
        }
        
        [HttpGet]
    public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
    public IActionResult Index(Post post)
        {
            var userId = userManager.GetUserId(User);
            var patid = PatientRepository.GetOne(e=>e.ApplicationUserId == userId).PatientID;
            if (ModelState.IsValid)
            {
                post.PatientID = patid;
                post.ApplicationUserId = userId;
                post.createdAt = DateTime.Now;  
                PostRepository.Add(post);
                PostRepository.commit();
                TempData["add"] = "Post Added successfully";
                return RedirectToAction("GetPost");
            }

            return RedirectToAction("Index","Home");


        }

        public IActionResult GetPost()
        {
            var userId = userManager.GetUserId(User);
            var res = PostRepository.Get(e => e.ApplicationUserId == userId,e=>e.Patients).OrderByDescending(e => e.createdAt); ;
            return View(res);
        }
    }

}
