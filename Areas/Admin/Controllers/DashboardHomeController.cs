using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

namespace pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class DashboardHomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IOrderRepository _orderRepository;
        public DashboardHomeController (IProductRepository _productRepository , ICategoryRepository _categoryRepository,
            IPatientRepository _patientRepository , IOrderRepository _orderRepository)
        {
            this._productRepository = _productRepository;
            this._categoryRepository = _categoryRepository;
            this._patientRepository = _patientRepository;
            this._orderRepository = _orderRepository;
        }

      
        public IActionResult Index()
        {
            // Fetch the count of products
            var productCount = _productRepository.Get(null).Count();

            // Fetch the count of categories
            var categoryCount = _categoryRepository.Get(null).Count();

            //Fetch the count of Patient
            var patientCount = _patientRepository.Get(null).Count();

            //Fetch the count of orders
            var orderCount = _patientRepository.Get(null).Count();


            //Count In Stock Products 
            var InStockProduct = _productRepository.Get(e => e.Qty != 0).Count();

            //Count Out Stock Products 
            var OutStockProduct = _productRepository.Get(e => e.Qty == 0).Count();


            ViewBag.ProductCount = productCount;
            ViewBag.CategoryCount = categoryCount;
            ViewBag.patientCount = patientCount;
            ViewBag.orderCount = orderCount;
            ViewBag.InStockProduct = InStockProduct;
            ViewBag.OutStockProduct = OutStockProduct;
           
            var result = _productRepository.Get(null);
            return View(result);
        }

        public IActionResult Search(String name)
        {
            var product = _productRepository.Get(e => e.ProductName.Contains(name), e => e.Category);
                if(product != null)
                {
                    return View(product);
                }
                else
                {
                    return View("NotFound");
               }
        }

        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult ManageInventory()

        {
            var result = _productRepository.Get(null);
            return View(result);
        }

    }
}
