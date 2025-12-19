using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace CompanyWebpages.Pages
{
    public class EmployeesModel : PageModel
    {

        private readonly EmployeeService _employeeService;

        public Dictionary<int, Employee> Employees { get; set; }
   

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }



        public EmployeesModel (EmployeeService employeeService)
        {

           _employeeService = employeeService;

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
                Employees = _employeeService.GetAll();
                // Hvis logget ind - vis siden som normalt
                return Page();
            }

        }

        public IActionResult OnPostDelete(int id)
        {
            try
            { 
                _employeeService.delete(id);
                SuccessMessage = $"Employee with ID {id} was deleted successfully.";

            }
            catch
            {
                ErrorMessage = $"Error deleting employee";
            }
            return RedirectToPage("/Employees");
        }


    }
}
