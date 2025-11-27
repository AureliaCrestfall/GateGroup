using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GateGroupWebpages.Pages.Shared
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string UserID { get; set; }

        [BindProperty ]
        public string Password { get; set; }
        [BindProperty]
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            // når man logger ud og ikke mere har adgang til siden (Ordre)
            HttpContext.Session.Remove("IsLoggedIn");
            return Page();

        }

       

        public IActionResult OnPost()
        {
            if (Password == "sas123" && UserID == "SAS")
            {
                HttpContext.Session.SetString("IsLoggedIn", "true"); // Gem i session

                return RedirectToPage("/NewOrder");
            }

            // Hvis password IKKE er korrekt
            ErrorMessage = "Incorrect password, please try again.";
            return Page();

        }
       

    }
}
