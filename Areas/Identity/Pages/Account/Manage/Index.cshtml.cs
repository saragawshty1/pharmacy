using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess.Repository.IRepository; // Ensure this namespace is included
using Models;
using DataAccess.Repository;

namespace pharmacy.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IPatientRepository _patientRepository; // Change this to the interface type

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IPatientRepository patientRepository) // Fix the parameter name
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _patientRepository = patientRepository; // Set the patient repository
        }

        public Models.Patient PatientModel { get; set; } // Holds the patient model


        // Add your LoadAsync method and other logic here
        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userId = _userManager.GetUserId(User);
           // Username = userName;
            var patient = _patientRepository.Get(e => e.ApplicationUserId == userId).FirstOrDefault();

            //Input = new InputModel
            //{
            //    PhoneNumber = phoneNumber
            //};
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = _userManager.GetUserId(User);
            PatientModel = _patientRepository.Get(e => e.ApplicationUserId == userId).FirstOrDefault();

            if (PatientModel == null)
            {
                return NotFound("Patient not found.");
            }

            await LoadAsync(user);
            return Page();
        }

       
    }
}
