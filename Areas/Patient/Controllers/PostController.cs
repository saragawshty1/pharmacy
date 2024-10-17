using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace pharmacy.Areas.Patient.Controllers
{
     
    public class PostController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IPatientRepository PatientRepository;
        private readonly IPostRepository PostRepository;
        private readonly ICommentRepository CommentRepository;


       public PostController(ICommentRepository CommentRepository, IPatientRepository PatientRepository,UserManager<IdentityUser> userManager , IPostRepository PostRepository)
        {

            this.PatientRepository = PatientRepository;
            this.userManager = userManager;
            this.PostRepository = PostRepository;
            this.CommentRepository=CommentRepository;
        }
        [Area("Patient")]

        [HttpGet]
    public IActionResult Index()
        {
            return View();
        }
        [Area("Patient")]

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
                post.IsCommented = false;
                PostRepository.Add(post);
                PostRepository.commit();
                TempData["add"] = "Post Added successfully";
                return RedirectToAction("Index", "Community");
            }

            return RedirectToAction("Index","Home");


        }
        


    }

}
