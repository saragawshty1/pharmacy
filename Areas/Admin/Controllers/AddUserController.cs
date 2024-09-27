using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class AddUserController : Controller
    {
        private readonly UserManager<IdentityUser> usermanager;
        public AddUserController(UserManager<IdentityUser> usermanager)
        {
            this.usermanager = usermanager;
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var newUser = new ApplicationUser
                {
                    UserName = user.Username,
                    Email = user.Email,
                    Address = user.Adress
                };
                var result = await usermanager.CreateAsync(newUser , user.Password);
                if (result.Succeeded)
                {
                    TempData["add"] = "User Added successfully";
                    return RedirectToAction("Index", "DashboardHome");
                }
                else
                {
                    foreach(var erorr in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, erorr.Description);
                    }
                }
                
            }
            return View(user);
        }
    }
}
