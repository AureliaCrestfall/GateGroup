using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GateGroupWebpages.Pages
{
    public class Create_accountModel : PageModel
    {
        private readonly CustomerService _customerService;

        public Create_accountModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty]
        public InputModel Customer { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("is not vaild");
                return Page();
            }

            var c = new Customer
            {
                Name = Customer.Name,
                Password = Customer.Password,
                Email = Customer.Email,
                CompanyName = Customer.CompanyName,
                CVR = Customer.CVR
            };

            _customerService.AddCustomer(c);

            return RedirectToPage("/Index");
        }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            public string CompanyName { get; set; }

            public string CVR { get; set; }
        }
    }
}

