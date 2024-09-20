using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IProductRepository _productRepository , ICategoryRepository _categoryRepository)
        {
            this._productRepository = _productRepository;
            this._categoryRepository = _categoryRepository;
        }

        /*Create product*/
        public IActionResult Create()
        {

            var result = _categoryRepository.Get(null).Select(e => new SelectListItem
            {
                Value = e.CategoryId.ToString(),
                Text = e.CategoryName
            }).ToList();

            ViewData["CategoryName"] = result;
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Add(product);
                _productRepository.commit();
                TempData["Add"] = "Product Added successfully";
                return RedirectToAction("Index", "DashboardHome");
            }
            return View(product);
        }

        /*Edit product */
        public IActionResult Edit(int id)
        {
            var listOfCategories = _categoryRepository.Get(null).Select(e => new SelectListItem
            {
                Value = e.CategoryId.ToString(),
                Text = e.CategoryName
            }).ToList();
            ViewData["CategoryName"] = listOfCategories;

            
            var result = _productRepository.GetOne(e => e.ProductId == id);
            return View(result);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Update(product);
                _productRepository.commit();
                TempData["Update"] = "Product update successfully";
                return RedirectToAction("Index", "DashboardHome");
            }
            return View(product);
        }

        /*delete product*/

        public IActionResult Delete(int id)
        {
            var result = _productRepository.GetOne(e => e.ProductId == id);
            if(result != null)
            {
                _productRepository.Delete(result);
                _productRepository.commit();
                return RedirectToAction("Index", "DashboardHome");
            }
            else
            {
                return RedirectToAction("NotFound", "Home");
            }
        }

       
    }
}
