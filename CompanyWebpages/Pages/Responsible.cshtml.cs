using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace CompanyWebpages.Pages
{
    public class ResponsibleModel : PageModel
    {
        readonly OrderService _os;
        readonly EmployeeService _es;
        readonly CustomerService _cs;
        [BindProperty]
        public Order Order { get; set; }
        [BindProperty]
        public Dictionary<int,Employee> Employees { get; set; }
        [BindProperty]
        public Customer CustomersOrder { get; set; }

        public ResponsibleModel(OrderService os, EmployeeService es, CustomerService cs)
        {
            _os = os;
            _es = es;
            _cs = cs;
        }

        public IActionResult OnGet(int orderid)
        {
            // Tjek om brugeren er logget ind før den giver adgang til siden 
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                // Hvis IKKE logget ind - send til login siden
                return RedirectToPage("/EmployeeLogin");
            }
            else
            {
                Load(orderid);
                // Hvis logget ind - vis siden som normalt
                return Page();
            }
            
        }

        public IActionResult OnPost()
        {
            Load(Order.ID);
            return Page();
        }

        public void Load(int orderid)
        {
            try
            {
                Order = _os.GetOrder(orderid);
                Employees = _es.GetEmployeeFromOrderID(orderid);
                CustomersOrder = _cs.GetCustomerByOrder(orderid);
                foreach (KeyValuePair<int, RecipePart> kp in Order.Recipe)
                {

                    Warehouse StoredLocation = _os.GetRecipePartLocation(kp.Key);

                    if (StoredLocation != null)
                    {
                        kp.Value.StoredLocation = StoredLocation.Name;
                    }
                    else
                    {
                        kp.Value.StoredLocation = "this part has no Stored Location";
                    }
                }

            }
            catch (Exception)
            {

            }
        }
    }
}
