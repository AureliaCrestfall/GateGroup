using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GateGroupWebpages.Pages
{
    public class NewOrderModel : PageModel
    {
        readonly OrderService _os;
        
        public IActionResult OnGet()
        {
            // Tjek om brugeren er logget ind før den giver adgang til siden 
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                // Hvis IKKE logget ind - send til login siden
                return RedirectToPage("/Login");
            }

            // Hvis logget ind - vis siden som normalt
            return Page();

        }

    }
}
