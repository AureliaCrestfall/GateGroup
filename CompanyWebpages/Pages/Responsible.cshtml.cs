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

        public void OnGet(int orderid)
        {
            Load(orderid);
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
