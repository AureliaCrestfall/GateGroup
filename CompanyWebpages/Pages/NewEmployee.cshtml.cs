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

        public void OnGet()
        {
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
