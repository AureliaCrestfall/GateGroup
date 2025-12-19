using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using gategourmetLibrary.Repo;
using gategourmetLibrary.Secret;

namespace CompanyWebpages.Pages
{
    // Razor Page model for admin view that shows all customers and deletes customer from system
    public class CustomerAdminListModel : PageModel
    {
        // list with all customers to show in the table
        public List<Customer> Customers { get; set; }

        // small message to show when an action has been done
        public string StatusMessage { get; set; }

        // service that handles customer logic
        private readonly CustomerService _customerService;

        // constructor - creates repository and service
        public CustomerAdminListModel()
        {
            string connectionString = new Connect().cstring;
            ICustomerRepo customerRepo = new CustomerRepo(connectionString);
            _customerService = new CustomerService(customerRepo);
        }

        // runs when the page is loaded with a GET request
        public IActionResult OnGet()
        {
            // Tjek om brugeren er logget ind før den giver adgang til siden 
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                // Hvis IKKE logget ind - send til login siden
                return RedirectToPage("/Login");
            }
            else
            {
                Customers = _customerService.GetAllCustomers();
                // Hvis logget ind - vis siden som normalt
                return Page();
            }
            
        }

        // runs when the Delete form is posted
        public void OnPostDelete(int customerId)
        {
            // delete from database
            _customerService.DeleteCustomer(customerId);

            // set status message
            StatusMessage = "Deleted customer with ID: " + customerId;

            // reload list so the table updates
            Customers = _customerService.GetAllCustomers();
        }
    }
}
