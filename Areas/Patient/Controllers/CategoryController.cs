using Azure.Core;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
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
        public CategoryController(ICategoryRepository _categoryRepository,IProductRepository productRepository)
        {
            
            this._categoryRepository = _categoryRepository;
            this.productRepository = productRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var result = _categoryRepository.Get(null);

            return View(result);
        }
        public IActionResult ProductPerCategory(int? id)
        {
            
            //TempData["id"] = id;
            //if (id == null)
            //{
            //    var result2 = productRepository.Get(e => e.CategoryID == (int)TempData["id"], e => e.Category);
            //    return View(result2);
            //}
            //else { 
            var  result = productRepository.Get(e => e.CategoryID == id, e => e.Category);
            return result.IsNullOrEmpty() ? RedirectToAction("NotFound", "Home") : View(result);
            //}

            //return View("Index");
        }

        public IActionResult Details (int id)
        {
            var res = productRepository.Get(e => e.ProductId == id);
            return res.IsNullOrEmpty() ? RedirectToAction("NotFound", "Home") : View(res);
        }

    }
}
