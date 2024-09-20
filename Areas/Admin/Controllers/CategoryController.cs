using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _CategoryRepository;
        
        public CategoryController(ICategoryRepository _CategoryRepository)
        {
            this._CategoryRepository = _CategoryRepository;
        }


        /*get all category*/
        public IActionResult GetAll()
        {
            var result = _CategoryRepository.Get(null);
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _CategoryRepository.Add(category);
                _CategoryRepository.commit();
                TempData["add"] = "Category Added successfully";
                return RedirectToAction("GetAll");
            }
            return View(category);  
        }

      

        /*Edit Category*/
        public IActionResult Edit(int id)
        {
            var result = _CategoryRepository.GetOne(e => e.CategoryId == id);
            return View(result);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _CategoryRepository.Update(category);
                _CategoryRepository.commit();
                TempData["update"] = "Category Update successfully";
                return RedirectToAction("GetAll");
            }
            return View(category);
            
        }


        /*delete Category*/
        
        public IActionResult Delete(int id)
        {
            var result = _CategoryRepository.GetOne(e => e.CategoryId == id);
            if(result != null)
            {
                _CategoryRepository.Delete(result);
                _CategoryRepository.commit();
                return RedirectToAction("GetAll");
            }
            else
            {
                return RedirectToAction("NotFound", "Home" );
            }
        }


    }
}
