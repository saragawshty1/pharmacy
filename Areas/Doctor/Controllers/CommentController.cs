using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Models;

namespace pharmacy.Areas.Patient.Controllers
{
    [Area("Doctor")]
    public class CommentController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IPatientRepository PatientRepository;
        private readonly IPostRepository PostRepository;
        private readonly ICommentRepository CommentRepository;


       public CommentController(ICommentRepository CommentRepository, IPatientRepository PatientRepository,UserManager<IdentityUser> userManager , IPostRepository PostRepository)
        {

            this.PatientRepository = PatientRepository;
            this.userManager = userManager;
            this.PostRepository = PostRepository;
            this.CommentRepository=CommentRepository;
        }




        //public IActionResult GetPost()
        //{
        //    var res = PostRepository.Get(null,e=>e.Patients).OrderByDescending(e => e.createdAt); 
        //    return View(res);
        //}


        public IActionResult GetPost(string filter)
        {
            // Initialize the variable for the posts result
            IEnumerable<Post> res;

            // If filter is "all" or not provided (null), get all posts
            if (filter == "all" || string.IsNullOrEmpty(filter))
            {
                // Get all posts including Patients
                var posts = PostRepository.Get(null, e => e.Patients);
                res = posts.OrderByDescending(e => e.createdAt);
            }
            // If filter is "replied", get only posts with comments
            else if (filter == "replied")
            {
                var posts = PostRepository.Get(e => e.IsCommented == true, e => e.Patients);
                res = posts.OrderByDescending(e => e.createdAt);
            }
            // If filter is "not_replied", get only posts without comments
            else if (filter == "not_replied")
            {
                var posts = PostRepository.Get(e => e.IsCommented == false, e => e.Patients);
                res = posts.OrderByDescending(e => e.createdAt);
            }
            else
            {
                // Default to getting all posts if the filter is invalid or unknown
                var posts = PostRepository.Get(null, e => e.Patients);
                res = posts.OrderByDescending(e => e.createdAt);
            }

            // Return the result to the view
            return View(res);
        }




        [HttpPost]
        public IActionResult AddComment( Comment comment)
        {
            
            if (ModelState.IsValid)
            {
                comment.CommentDate = DateTime.Now;
                comment.DoctorID = 1;
                CommentRepository.Add(comment);
                CommentRepository.commit();
                
                var result=PostRepository.GetOne(e=>e.Id== comment.PostID);
                if (result != null)
                {
                    result.IsCommented  = true;
                    // Update other properties if necessary...
                    PostRepository.Update(result);
                    PostRepository.commit();
                }

                return RedirectToAction("GetPost");
            }

            return RedirectToAction("Index", "Home");
        }

    }

}
