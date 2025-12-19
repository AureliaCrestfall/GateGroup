using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CompanyWebpages.Pages
{
    public class SignUpCustommerModel : PageModel
    {
        private readonly CustomerService _customerService;

        [BindProperty(SupportsGet = true)]
        public int CustomerID { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public SignUpCustommerModel(CustomerService customerService)
        {
            _customerService = customerService;
        }


        public IActionResult OnGet()
        {
            // Tjek om brugeren er logget ind før den giver adgang til siden 
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                // Hvis IKKE logget ind - send til login siden
                return RedirectToPage("/EmployeeLogin");
            }
            else
            {
                
                // Hvis logget ind - vis siden som normalt
                return Page();
            }
        }
        public IActionResult OnPost(int customerId, string password, string confirmPassword)
        {
            CustomerID = customerId;
            if (password != confirmPassword)
            {
                ErrorMessage = "The passwords do not match";
                return Page();
            }

            // Hent kunden fra databasen
            var customer = _customerService.GetCustomer(customerId);
            if (customer == null)
            {
                ErrorMessage = $"Customer with ID {customerId} does not exist";
                return Page();
            }
          
            // Opdater kundens password
            customer.Password = password;
            _customerService.UpdateCustomer(customerId, customer);

            SuccessMessage = $"Password created for customer ID {customerId}";
            return Page();


        }
    }
}
