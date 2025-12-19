using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace CompanyWebpages.Pages
{
    public class AsignTaskModel : PageModel
    {


        readonly OrderService _os;
        readonly EmployeeService _es;
        readonly CustomerService _cs;
        [BindProperty]
        public Order Order { get; set; }

        [BindProperty]
        public Customer CustomersOrder { get; set; }
        [BindProperty]

        public List<SelectListItem> EmployeesOptions { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string SuccessMessage { get; set; }





        public AsignTaskModel(OrderService os, EmployeeService es, CustomerService cs)
        {
            _os = os;
            _es = es;
            _cs = cs;
            EmployeesOptions = new List<SelectListItem>();
        }

        public IActionResult OnGet(int orderid )
        {

            // Tjek om brugeren er logget ind før den giver adgang til siden 
            if (HttpContext.Session.GetString("admin") != "true")
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
       


        public IActionResult OnPost(int employeeid, int recipeid, int orderid)
        {
            try
            {
                _es.AsignTask(employeeid, orderid, recipeid );
                SuccessMessage = "task asigned";
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            try
            {
                Order = _os.GetOrder(orderid);
                CustomersOrder = _cs.GetCustomerByOrder(orderid);
                Dictionary<int, Employee> AllEmployeesDict = _es.GetAll();

                foreach (KeyValuePair<int, Employee> keyValuePair in AllEmployeesDict)
                {
                    EmployeesOptions.Add(new SelectListItem(keyValuePair.Value.Name, keyValuePair.Key.ToString()));

                }
            }
            catch (Exception)
            {

            }

            return Page();
        }

        public void Load(int orderid)
        {
            try
            {
                Order = _os.GetOrder(orderid);
                CustomersOrder = _cs.GetCustomerByOrder(orderid);
                Dictionary<int, Employee> AllEmployeesDict = _es.GetAll();

                foreach (KeyValuePair<int, Employee> keyValuePair in AllEmployeesDict)
                {
                    EmployeesOptions.Add(new SelectListItem(keyValuePair.Value.Name, keyValuePair.Key.ToString()));

                }
            }
            catch (Exception)
            {

            }
        }
    }
}
