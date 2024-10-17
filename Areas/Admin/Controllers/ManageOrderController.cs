using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageOrderController : Controller
    {

        private readonly IOrderItemRepository _orderItemRepository;
        private readonly EmailService _emailService;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<IdentityUser> userManager;
        // private readonly IPaymentService _paymentService;

        public ManageOrderController(IOrderRepository _orderRepository, UserManager<IdentityUser> userManager, IOrderItemRepository orderItemRepository, EmailService _emailService) //IPaymentService _paymentService )
        {
            this._orderItemRepository = orderItemRepository;
            this._emailService = _emailService;
            this.userManager = userManager;
            this._orderRepository = _orderRepository;
            //this._paymentService = _paymentService;


        }

        public async Task<IActionResult> GetAll()
        {
            var orders = _orderRepository.Getorder(null,
       query => query.Include(o => o.OrderItems).ThenInclude(oi => oi.Product),
       query => query.Include(o => o.OrderItems).ThenInclude(oi => oi.Device),
       query => query.Include(o => o.ApplicationUser)
   ).OrderByDescending(o=>o.orderDate);

            // Assign the UserName to review.ApplicationUser.UserName
            foreach (var orderItem in orders)
            {
                if (!string.IsNullOrEmpty(orderItem.ApplicationUserId))
                {
                    // Fetch the ApplicationUser using UserManager
                    var user = await userManager.FindByIdAsync(orderItem.ApplicationUserId);
                    if (user != null)
                    {
                        // Assign the UserName to review.ApplicationUser.UserName
                        if (orderItem.ApplicationUser == null)
                        {
                            orderItem.ApplicationUser = new ApplicationUser(); // Initialize if null
                        }
                        orderItem.ApplicationUser.Email = user.Email; // Assign the username
                    }
                    else
                    {
                        // Handle case where user is not found (optional)
                        if (orderItem.ApplicationUser == null)
                        {
                            orderItem.ApplicationUser = new ApplicationUser(); // Initialize if null
                        }
                        orderItem.ApplicationUser.Email = "Unknown User"; // Default value
                    }
                }
            }
            return View(orders);
        }







        //Confirm Order
        public async Task<IActionResult> Confirm(int id)
        {

            var order = _orderRepository.Getorder(
            e => e.OrderId == id,
            query => query.Include(o => o.OrderItems).ThenInclude(oi => oi.Product),
            query => query.Include(o => o.OrderItems).ThenInclude(oi => oi.Device),
            query => query.Include(o => o.ApplicationUser)
           ).FirstOrDefault();







            if (order != null)
            {
                foreach (var OrderItem in order.OrderItems)
                {

                    if (!string.IsNullOrEmpty(OrderItem.Order.ApplicationUserId))
                    {
                        // Fetch the ApplicationUser using UserManager
                        var user =await userManager.FindByIdAsync(OrderItem.Order.ApplicationUserId);
                        if (user != null)
                        {
                            // Assign the UserName to review.ApplicationUser.UserName
                            if (OrderItem.Order.ApplicationUser == null)
                            {
                                OrderItem.Order.ApplicationUser= new ApplicationUser(); // Initialize if null
                            }
                            OrderItem.Order.ApplicationUser.Email = user.Email; // Assign the username
                            OrderItem.Order.ApplicationUser.UserName = user.UserName;

                        }
                    }
                }





                string toEmail = order.ApplicationUser.Email;
                string subject = $"Order Confirmation - Order #{order.OrderId}";


                var totalCost = order.OrderItems.Sum(item => item.cost * item.count);
                var itemList = string.Join("<br/>", order.OrderItems.Select(item =>
                    $"{item.Product?.ProductName} - Quantity: {item.count} - Price: {item.cost * item.count} EGP"));

                string body = $"Dear {order.ApplicationUser?.UserName},<br/><br/>" +
                              $"Your order has been confirmed.<br/>" +
                              $"Order ID: {order.OrderId}<br/>" +
                              $"Total Price: {totalCost} EGP<br/>" +
                              $"Items:<br/>{itemList}<br/>" +
                              $"Thank you for shopping with us!";

                _emailService.SendOrderConfirmationEmail(toEmail, subject, body);
                TempData["confirm"] = "Order confirmed and email sent successfully.";
                return RedirectToAction("GetAll");
            }
            return NotFound();

        }
        public IActionResult CancelOrder(int id)
        {
            var order = _orderRepository.GetOne(e => e.OrderId == id, e => e.ApplicationUser, e => e.OrderItems);

            if (order != null)
            {
                _orderRepository.Delete(order);
                _orderRepository.commit();
                var customerEmail = order.ApplicationUser?.Email; // جلب بريد العميل
                string subject = $"Order #{order.OrderId} Cancellation";
                string body = $"Dear {order.ApplicationUser?.UserName},<br/><br/>" +
                                  $"Your order has been canceled<br/>" +
                                  $"Order ID: {order.OrderId}<br/><br/>" +
                                  "Thank you for using our services.";

                _emailService.SendOrderConfirmationEmail(customerEmail, subject, body);

                TempData["confirm"] = "Order Canceled and email sent successfully.";

                return RedirectToAction("GetAll");
            }

            return NotFound();
        }


    }
}



    

