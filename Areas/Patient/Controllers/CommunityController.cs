using DataAccess;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace pharmacy.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class CommunityController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IPatientRepository PatientRepository;
        private readonly IPostRepository PostRepository;
        private readonly ICommentRepository CommentRepository;

        ApplicationDBContext _context = new ApplicationDBContext();
        public CommunityController(ICommentRepository CommentRepository, IPatientRepository PatientRepository, UserManager<IdentityUser> userManager, IPostRepository PostRepository)
        {

            this.PatientRepository = PatientRepository;
            this.userManager = userManager;
            this.PostRepository = PostRepository;
            this.CommentRepository = CommentRepository;
        }

        public IActionResult Index()
        {
            //var res = PostRepository.Get(null, e => e.Patients,e=>e.Comments).OrderByDescending(e => e.createdAt);
            var res =_context.Posts
        .Include(p => p.Patients)  // Include related Patients
        .Include(p => p.Comments)   // Include related Comments
        .ThenInclude(c => c.Doctors) // Include related Doctors for each Comment
        .OrderByDescending(p => p.createdAt) // Order by createdAt
        .ToList();

            return View(res);
        }
    }
}
