using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;

namespace pharmacy.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly UserManager<IdentityUser> userManager;

        public ShoppingCartController(IShoppingCartRepository _shoppingCartRepository, UserManager<IdentityUser> userManager)
        {

            this._shoppingCartRepository = _shoppingCartRepository;
            this.userManager = userManager;
        }
        public IActionResult Index(int ProductId)
        {
            var userId = userManager.GetUserId(User);

            if (ProductId != 0)
            {
                ShoppingCart cart = new()
                {
                    ProductId = ProductId,
                    ApplicationUserId = userId,
                    Count = 1
                };
                _shoppingCartRepository.Add(cart);
                _shoppingCartRepository.commit();
            }
            else
            {
                // Handle case when no product is selected
                ViewBag.ErrorMessage = "No product selected. Please select a product.";
            }

            // Fetch the shopping cart items for the user
            var result = _shoppingCartRepository.Get(e => e.ApplicationUserId == userId, e => e.Product);
            TempData["shoppingCart"] = JsonConvert.SerializeObject(result);
            ViewBag.Total = result.Sum(e => e.Count * e.Product.Price);

            return View(result);
        }
        public IActionResult Increment(int Id)
        {
            // Fetch the shopping cart item by its Id
            var result = _shoppingCartRepository.Get(e => e.Id == Id, e => e.Product).FirstOrDefault();
 
            result.Count++; 
            _shoppingCartRepository.commit();  

            return RedirectToAction("Index");
        }

        public IActionResult Decreamnt(int Id)
        {

            //var result = context.ShoppingCart.Include(e => e.Movies).Where(e => e.Id == Id).FirstOrDefault();
            var result = _shoppingCartRepository.Get(e => e.Id == Id, e => e.Product).FirstOrDefault();
            if (result.Count == 1)
            {
                _shoppingCartRepository.Delete(result);
            }
            else
                result.Count--;

            _shoppingCartRepository.commit();
            return RedirectToAction("Index");

        }

        public IActionResult Delete(int Id)
        {
            var result = _shoppingCartRepository.Get(e => e.Id == Id, e => e.Product).FirstOrDefault();
            _shoppingCartRepository.Delete(result);
            _shoppingCartRepository.commit();
            return RedirectToAction("Index");

        }

    }
}
