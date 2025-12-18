using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;


namespace CompanyWebpages.Pages
{
    public class EmployeeLoginModel : PageModel
    {
        readonly EmployeeService _cs;
        [BindProperty]
        public string UserID { get; set; }

        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string ErrorMessage { get; set; }
        public IActionResult OnGet()
        {
            // når man går til login siden, logges brugeren helt ud
            HttpContext.Session.Clear();
            return Page();
        }
        public EmployeeLoginModel(EmployeeService cs)
        {
            _cs = cs;

        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(UserID))
            {
                ErrorMessage = "Please enter a user ID.";
                return Page();
            }

            int userIdInt;
            if (!int.TryParse(UserID, out userIdInt))
            {
                ErrorMessage = "User ID must be a number.";
                return Page();
            }

            Employee employ = _cs.Get(userIdInt);

            if (employ == null)
            {
                ErrorMessage = "User not found.";
                return Page();
            }

            if (Password == employ.Password)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("username", employ.Name);
                HttpContext.Session.SetString("userid", employ.Id.ToString());

                if (_cs.LoginAdmin(userIdInt, Password))
                {
                    HttpContext.Session.SetString("admin", "true");
                }

                return RedirectToPage("/EmployeeDashboard");
            }

            ErrorMessage = "Incorrect password, please try again.";
            return Page();
        }



    }

}
