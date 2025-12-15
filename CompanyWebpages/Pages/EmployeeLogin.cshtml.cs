using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;


namespace CompanyWebpages.Pages
{
    public class EmployeeLoginModel : PageModel
    {
        readonly EmployeeService _es;
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
        public EmployeeLoginModel(EmployeeService es)
        {
            _es = es;

        }

        public IActionResult OnPost()
        {
            Employee employ = _es.Get(Convert.ToInt32(UserID));
            if (employ != null)
            {
                if (Password == employ.Password && Convert.ToInt32(UserID) == employ.Id)
                {
                    HttpContext.Session.SetString("IsLoggedIn", "true"); // Gem i session
                    HttpContext.Session.SetString("username", $"{employ.Name}"); // Gem i session
                    HttpContext.Session.SetString("userid", $"{employ.Id}");
                    if (_es.LoginAdmin(Convert.ToInt32(UserID), Password) == true)
                    {
                        HttpContext.Session.SetString("admin", "true"); // Gem i session
                    }


                    return RedirectToPage("/EmployeeDashboard");
                    //return RedirectToPage("/NewOrder");

                }
            }
            else
            {
                // Hvis id IKKE er korrekt
                ErrorMessage = "Incorrect id, please try again.";
                return Page();
            }

                // Hvis password IKKE er korrekt
                ErrorMessage = "Incorrect password, please try again.";
            return Page();

        }


    }
    
}
