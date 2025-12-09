using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CompanyWebpages.Pages
{
    public class NewEmployeeModel : PageModel
    {
         readonly EmployeeService _es;
        [BindProperty]
        public Employee newEmployee { get; set; }
        [BindProperty]
        public string ErrorMessage { get; set; }



        public NewEmployeeModel(EmployeeService es)
        {
            _es = es;
            newEmployee = new Employee();
        }

        public IActionResult OnGet()
        {
            // read user id from session (employee id here )
            string userIdString = HttpContext.Session.GetString("userid");
            string isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");

            // if user is not logged in or id is missing redirect to login
            if (string.IsNullOrEmpty(userIdString) || isLoggedIn != "true")
            {
                return RedirectToPage("/EmployeeLogin");
            }
            else
            {
                return Page();
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                _es.Add(newEmployee);

            }
            catch (Exception ex)
            {
                ErrorMessage = $"{ex.Message}";
                return Page();

            }


            return RedirectToPage("/index");

        }
    }
}
