using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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


        public void OnGet()
        {
           Employees =_employeeService.GetAll();


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
