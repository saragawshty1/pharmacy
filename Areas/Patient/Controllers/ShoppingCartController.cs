using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using Stripe.Checkout;

namespace pharmacy.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IOrderRepository orderRepository;

        public ShoppingCartController(IShoppingCartRepository _shoppingCartRepository,
            UserManager<IdentityUser> userManager, IOrderRepository orderRepository)
        {

            this._shoppingCartRepository = _shoppingCartRepository;
            this.userManager = userManager;
            this.orderRepository = orderRepository;
        }
        [Authorize]
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

                // var cat = cart.Product.Category.CategoryID;
                //return RedirectToAction("ProductPerCategory", "Category");
                //return View(cart);
            }
            else
            {
                // Handle case when no product is selected
                ViewBag.ErrorMessage = "No product selected. Please select a product.";
            }

            // Fetch the shopping cart items for the user
            var result = _shoppingCartRepository.Get(e => e.ApplicationUserId == userId, e => e.Product);
            TempData["Total"] = result.Sum(e => e.Count * e.Product.Price);
            TempData["shoppingCart"] = JsonConvert.SerializeObject(result);

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
        public IActionResult Pay()
        {
            // Deserialize the shopping cart from TempData
            var cartItems = JsonConvert.DeserializeObject<IEnumerable<ShoppingCart>>((string)TempData["shoppingCart"]);

            // Stripe session options for payment
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Patient/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Patient/checkout/cancel",
            };

            // Add shopping cart items to Stripe line items
            foreach (var cartItem in cartItems)
            {
                var lineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cartItem.Product.ProductName,
                        },
                        UnitAmount = (long)cartItem.Product.Price * 100, // Price in cents
                    },
                    Quantity = cartItem.Count,
                };
                options.LineItems.Add(lineItem);
            }

            // Create a Stripe session
            var service = new SessionService();
            var session = service.Create(options);

            if (session != null)
            {
                var userId = userManager.GetUserId(User);

                // Retrieve items from the shopping cart stored in the database
                var cartItemsDb = _shoppingCartRepository.Get(e => e.ApplicationUserId == userId, e => e.Product);

                // Create a new order for the user
                var order = new Order
                {

                    ApplicationUserId = userId,
                    orderDate = DateTime.Now,
                    OrderStatusID = 1,
                   // totalprice = (decimal)TempData["Total"],
                    OrderItems = new List<OrderItem>() // Initialize list for order items
                };

                // Add items from the shopping cart to the order
                foreach (var cartItem in cartItemsDb)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        count = cartItem.Count,
                         cost= cartItem.Product.Price,
                        OrderId = order.OrderId // Link OrderItem to the newly created order
                    };

                    order.OrderItems.Add(orderItem); // Add to in-memory list
                }

                // Add the new order to the database and save changes
                 orderRepository.Add(order);
                orderRepository.commit(); // Save to generate OrderId

                // Clear the shopping cart after the order is created
                _shoppingCartRepository.Deleterange(cartItemsDb);
                _shoppingCartRepository.commit();
            }

            return Redirect(session.Url); // Redirect to Stripe payment page
        }





    }
}
