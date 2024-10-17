using Azure.Core;
using DataAccess;
using DataAccess.Migrations;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using  Models;
using System.Diagnostics;

namespace pharmacy.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class CategoryController : Controller
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository productRepository;
        private readonly IFavoriteRepository favoriteRepository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IReviewRepository ReviewRepository;
        private readonly ApplicationDBContext context;
        public CategoryController(ApplicationDBContext context,ICategoryRepository _categoryRepository, IProductRepository productRepository, IFavoriteRepository favoriteRepository, UserManager<IdentityUser> userManager, IReviewRepository ReviewRepository)
        {

            this._categoryRepository = _categoryRepository;
            this.productRepository = productRepository;
            this.favoriteRepository = favoriteRepository;
            this.ReviewRepository = ReviewRepository;
            this.userManager = userManager;
            this.context = context;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            var result = _categoryRepository.Get(null);

            return View(result);
        }
        public IActionResult ProductPerCategory(int? id)
        {

            
            var result = productRepository.Get(e => e.CategoryID == id, e => e.Category);
            return result.IsNullOrEmpty() ? RedirectToAction("NotFound", "Home") : View(result);
            //}

            //return View("Index");
        }

         
        public async Task<IActionResult> Details(int id)
        {
            // Fetch product details
            var res = productRepository.Get(e => e.ProductId == id);

            
            var Reviews = ReviewRepository.Get(e => e.ProductId == id);

            // For each review, retrieve the ApplicationUser using the UserManager
            foreach (var review in Reviews)
            {
                if (!string.IsNullOrEmpty(review.ApplicationUserId))
                {
                    // Fetch the ApplicationUser using UserManager
                    var user = await userManager.FindByIdAsync(review.ApplicationUserId);
                    if (user != null)
                    {
                        // Assign the UserName to review.ApplicationUser.UserName
                        if (review.ApplicationUser == null)
                        {
                            review.ApplicationUser = new ApplicationUser(); // Initialize if null
                        }
                        review.ApplicationUser.UserName = user.UserName; // Assign the username
                    }
                    else
                    {
                        // Handle case where user is not found (optional)
                        if (review.ApplicationUser == null)
                        {
                            review.ApplicationUser = new ApplicationUser(); // Initialize if null
                        }
                        review.ApplicationUser.UserName = "Unknown User"; // Default value
                    }
                }
            }

            var viewModel = new ProductDetailsViewModel
            {
                Details = res,
                Feedbacks = Reviews
            };

            return View(viewModel);
        }

        [Authorize]
        public IActionResult AddToFavorite(int ProductId , int CategoryId)
        {
            var userId = userManager.GetUserId(User);
            var res = productRepository.Get(e => e.ProductId == ProductId);
            //var cat = productRepository.Get(e => e.ProductId == ProductId,e=>e.Category);

            if (ProductId != 0)
            {
                var existingFavorite = favoriteRepository.Get(f => f.ProductId == ProductId && f.ApplicationUserId == userId);

                if (existingFavorite.Count() == 0)
                { 
                    
                    Favorite favorite = new Favorite
                    {
                        ProductId = ProductId,
                        ApplicationUserId = userId,

                    };
                    favoriteRepository.Add(favorite);
                    favoriteRepository.commit();
                    TempData["sucess"] = "This Product Added to Your Favorite List!";
                    return RedirectToAction("ProductPerCategory", "Category", new { id = favorite.products.CategoryID});
                }

                else
                {

                    TempData["Erorr"] = "This Product already Exists in Your Favorite List!";
                   return RedirectToAction("ProductPerCategory", "Category", new { id = CategoryId });

                    // return RedirectToAction("ProductPerCategory", "Category", new { id =  });
                }

            }

            var result = favoriteRepository.Get(e => e.ApplicationUserId == userId, e => e.products).OrderByDescending(e=>e.Id).ToList();
            return View(result);

        }
        [Authorize]
        public IActionResult RemoveFromFav(int Id)
        {
            var result = favoriteRepository.Get(e => e.Id == Id, e => e.products).FirstOrDefault();
            favoriteRepository.Delete(result);
            favoriteRepository.commit();
            return RedirectToAction("AddToFavorite");

        }
            
        [HttpPost]
        public IActionResult AddReview(Review review)
        {
            var userId = userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                review.ApplicationUserId = userId;
                    //ProductId = ProductId,
                    //Name= reviewText
                    review.ISApproved = false;
                

                ReviewRepository.Add(review);
                ReviewRepository.commit();
                return RedirectToAction("Details", "Category", new { id = review.ProductId });
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
