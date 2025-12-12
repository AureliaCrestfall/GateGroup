using System.Collections.Generic;
using gategourmetLibrary.Models;
using gategourmetLibrary.Repo;
using gategourmetLibrary.Secret;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace CompanyWebpages.Pages
{
    // admin page that shows all orders and allows cancelling them
    public class AdminOrderListModel : PageModel
    {

        // list with all orders to show in the table
        public List<Order> Orders { get; set; }

        // status message shown after cancel
        public string StatusMessage { get; set; }

        //bind property to get emp filter from query string
        [BindProperty(SupportsGet = true)]
        // this ? makes it optional and allows null values
        public string? empFilter { get; set; }

        //bind property to 
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        //bind property to 
        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }




        //dropdown list of employees
        public List<SelectListItem> Filter { get; set; }


        // service that handles order logic
        private readonly OrderService _orderService;

        // constructor creates repository and service
        public AdminOrderListModel()
        {
            string connectionString = new Connect().cstring;
            IOrderRepo orderRepo = new OrderRepo(connectionString);
            _orderService = new OrderService(orderRepo);
        }

        //// hj�lper method that loads only non cancelled orders
        //private void LoadAllOrders()
        //{
        //    Orders = _orderService.GetAllOrders();
        //}

        // runs when the page is loaded with a get method 
        public void OnGet()
        {
            // load all orders
            Orders = _orderService.GetAllOrders();

            //initialize the filter list
            Filter = new List<SelectListItem>();

            //get constring(DB) from connect class
            string connectionString = new Connect().cstring;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //open connection
                conn.Open();

                // SQL query to select orders
                string sql = @"SELECT E_ID, E_Name FROM Employee";

                //execute command
                using (SqlCommand command = new SqlCommand(sql, conn))
                //read data
                using (SqlDataReader reader = command.ExecuteReader())

                {
                    //loop through the data
                    while (reader.Read())
                    {
                        //add data to the filter list
                        Filter.Add(new SelectListItem(
                            // ID as value, Name as text
                            reader["E_Name"].ToString(),
                            reader["E_ID"].ToString()

                       ));
                    }
                }
            }

            //if admin has selected an employee, filter employee´s orders
            if (!string.IsNullOrEmpty(empFilter))
            {
                //convert empFilter to int from string
                int empId = int.Parse(empFilter);

                //list to hold employee order IDs
                List<int> empOrdersIds = new List<int>();

                //get constring(DB) again from connect class 
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //open connection
                    conn.Open();

                    //sql: get all order IDs for the selected employee
                    string sql = @"SELECT O_ID FROM EmployeeRecipePartOrderTable WHERE E_ID = @id";

                    //create sql command
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        //add parameter for employee id to avoid sql injection
                        cmd.Parameters.AddWithValue("@id", empId);

                        //execute command and read results
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            //loop through results and add order IDs to the list
                            while (reader.Read())
                            {
                                //add order ID to the list
                                empOrdersIds.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
                //filter orders to keep only those that match the employee order IDs
                Orders = Orders
                    .Where(o => empOrdersIds.Contains(o.ID)) // keep only matching orders
                    .ToList(); // convert back to list

            }

            //filter by date FROM
            if (FromDate.HasValue)
            {
                //keep only orders made on or after FromDate
                Orders = Orders
                    .Where(o => o.OrderMade.Date >= FromDate.Value.Date)
                    .ToList();
            }

            //filter by date TO
            if (ToDate.HasValue)
            {
                //keep only orders made on or before ToDate
                Orders = Orders
                    .Where(o => o.OrderMade.Date <= ToDate.Value.Date)
                    .ToList();
            }



        }

        

        // runs when the cancel form is posted
        public IActionResult OnPostCancel(int orderId)
        {
            // call service to cancel the order
            _orderService.CancelOrder(orderId);

            // set status message so admin can see what happened
            StatusMessage = "Ordre #" + orderId + " er blevet annulleret.";

            // after a POST we redirect to GET so the page reloads clean
            return RedirectToPage();
        }
    }
}
