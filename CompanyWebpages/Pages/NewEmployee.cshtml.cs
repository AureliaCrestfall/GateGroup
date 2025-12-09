using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CompanyWebpages.Pages
{
    public class NewEmployeeModel : PageModel
    {
         readonly EmployeeService _es;



        public NewEmployeeModel(EmployeeService es)
        {
            _es = es;

        }

        public void OnGet()
        {
        }
    }
}
