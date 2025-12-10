using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GateGroupWebpages.Pages.Shared
{
    public class LoginModel : PageModel
    {
        readonly CustomerService _cs ;
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
        public LoginModel(CustomerService cs)
        {
            _cs = cs;

        }



        public IActionResult OnPost()
        {
            Customer cust = _cs.GetCustomer( Convert.ToInt32(UserID));

            try
            {
               if(cust != null)
                {
                    if (Password == cust.Password && UserID == cust.ID.ToString())
                    {
                        HttpContext.Session.SetString("IsLoggedIn", "true"); // Gem i session
                        HttpContext.Session.SetString("username", $"{cust.Name}"); // Gem i session
                        HttpContext.Session.SetString("userid", $"{cust.ID}");

                        return RedirectToPage("/Dashboard");
                        //return RedirectToPage("/NewOrder");

                    }
                    else
                    {
                        ErrorMessage = "Incorrect password, please try again.";
                        return Page();
                    }
               }
                else
                {

                    ErrorMessage = "Incorrect Userid, please try again.";
                    return Page();
                }
                
            }
            catch(NullReferenceException ex)
            {
                ErrorMessage = "Incorrect password, please try again.";
                return Page();
            }
           
            
            


        }
       

    }
}
