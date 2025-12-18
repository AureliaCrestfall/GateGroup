using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GateGroupWebpages.Pages.Shared
{
    public class LoginModel : PageModel
    {
        readonly CustomerService _cs ;
        [BindProperty]
        public string UserID { get; set; }

        [BindProperty ]
        public string Password { get; set; }
        [BindProperty]
        public string ErrorMessage { get; set; }
        public IActionResult OnGet()
        {

    // når man logger ud og ikke mere har adgang til siden (Ordre)
    HttpContext.Session.Remove("IsLoggedIn");
            return Page();

        }
        public LoginModel(CustomerService cs)
        {
            _cs = cs;

        }



        public IActionResult OnPost()
        {
            // 1. Tjek om UserID er tom
            if (string.IsNullOrEmpty(UserID))
            {
                ErrorMessage = "Please enter a User ID.";
                return Page();
            }

            // 2. Konverter UserID til int sikkert
            if (!int.TryParse(UserID, out int userIdInt))
            {
                ErrorMessage = "User ID must be a number.";
                return Page();
            }

            // 3. Hent kunde
            Customer cust = _cs.GetCustomer(userIdInt);

            // 4. Tjek om kunden findes
            if (cust == null)
            {
                ErrorMessage = "Incorrect User ID, please try again.";
                return Page();
            }

            // 5. Tjek password
            if (Password == cust.Password)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("username", cust.Name);
                HttpContext.Session.SetString("userid", cust.ID.ToString());

                return RedirectToPage("/Dashboard");
            }

            // 6. Hvis password er forkert
            ErrorMessage = "Incorrect password, please try again.";
            return Page();
        }



    }
}
