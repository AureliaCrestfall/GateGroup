using System;
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
    // Admin page that shows all orders and allows cancelling them.
    // Her har admin mulighed for at filtrere på employee, department
    // og nu også status på dagens ordrer.
    public class AdminOrderListModel : PageModel
    {
        // list with all orders to show in the table
        public List<Order> Orders { get; set; }

        // status message shown after cancel
        public string StatusMessage { get; set; }

        // bind property to get emp filter from query string (employee dropdown)
        [BindProperty(SupportsGet = true)]
        public string empFilter { get; set; }

        // dropdown list of employees
        public List<SelectListItem> Filter { get; set; }

        // bind property to get department filter from query string (department dropdown)
        [BindProperty(SupportsGet = true)]
        public string departmentFilter { get; set; }

        // dropdown list of departments
        public List<SelectListItem> DepartmentFilter { get; set; }

        // NEW: bind property to get status filter for user story "filter by status today"
        [BindProperty(SupportsGet = true)]
        public string statusFilter { get; set; }

        // service that handles order logic
        private readonly OrderService _orderService;
        
        // service that handles customer logic
        private readonly CustomerService _customerService;

        // constructor creates repository and service
        public AdminOrderListModel()
        {
            string connectionString = new Connect().cstring;
            IOrderRepo orderRepo = new OrderRepo(connectionString);
            ICustomerRepo customerRepo = new CustomerRepo(connectionString);
            _orderService = new OrderService(orderRepo);
            _customerService = new CustomerService(customerRepo);
        }

        // runs when the page is loaded with a get method 
        public void OnGet()
        {
            // first we load all orders from the service
            Orders = _orderService.GetAllOrders();

            // then we prepare both dropdowns: employees and departments
            LoadEmployeeFilter();
            LoadDepartmentFilter();

            // afterwards we apply filters if admin has selected something
            ApplyEmployeeFilter();
            ApplyDepartmentFilter();
            ApplyStatusTodayFilter();
        }

        // helper method that loads all employees into filter list
        private void LoadEmployeeFilter()
        {
            Filter = new List<SelectListItem>();

            string connectionString = new Connect().cstring;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"SELECT E_ID, E_Name FROM Employee";

                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // value = E_ID, text = E_Name
                            SelectListItem item = new SelectListItem(
                                reader["E_Name"].ToString(),
                                reader["E_ID"].ToString()
                            );

                            Filter.Add(item);
                        }
                    }
                }
            }
        }

        // helper method that loads all departments into DepartmentFilter list
        private void LoadDepartmentFilter()
        {
            DepartmentFilter = new List<SelectListItem>();

            string connectionString = new Connect().cstring;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // vi bruger D_ID og D_Name fra Department
                string sql = @"SELECT D_ID, D_Name FROM Department ORDER BY D_Name";

                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SelectListItem item = new SelectListItem(
                                reader["D_Name"].ToString(),
                                reader["D_ID"].ToString()
                            );

                            DepartmentFilter.Add(item);
                        }
                    }
                }
            }
        }

        // applies employee filter if empFilter has a value
        private void ApplyEmployeeFilter()
        {
            if (!string.IsNullOrEmpty(empFilter))
            {
                string connectionString = new Connect().cstring;

                // list to hold employee order IDs
                List<int> empOrdersIds = new List<int>();

                // get all O_IDs for this employee from EmployeeRecipePartOrderTable
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"SELECT O_ID FROM EmployeeRecipePartOrderTable WHERE E_ID = @id";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", int.Parse(empFilter));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int orderId = reader.GetInt32(0);
                                empOrdersIds.Add(orderId);
                            }
                        }
                    }
                }

                // build a new list with only the orders belonging to this employee
                List<Order> filteredOrders = new List<Order>();

                foreach (Order order in Orders)
                {
                    if (empOrdersIds.Contains(order.ID))
                    {
                        filteredOrders.Add(order);
                    }
                }

                Orders = filteredOrders;
            }
        }

        // applies department filter if departmentFilter has a value
        // så admin kan se ordrestatus for en specifik afdeling
        private void ApplyDepartmentFilter()
        {
            if (!string.IsNullOrEmpty(departmentFilter))
            {
                string connectionString = new Connect().cstring;

                // list with order IDs that belong to the selected department
                List<int> depOrderIds = new List<int>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Vi finder alle ordrer, som har recipe parts i et warehouse,
                    // der er knyttet til den valgte department via WarehouseDepartment
                    string sql =
                        @"SELECT DISTINCT ot.O_ID
                          FROM OrderTable ot
                          JOIN orderTableRecipePart otr ON ot.O_ID = otr.O_ID
                          JOIN RecipePart rp ON rp.R_ID = otr.R_ID
                          JOIN werehouseRecipePart wrp ON wrp.R_ID = rp.R_ID
                          JOIN warehouse w ON w.W_ID = wrp.W_ID
                          JOIN WarehouseDepartment wd ON wd.W_ID = w.W_ID
                          WHERE wd.D_ID = @depId";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@depId", int.Parse(departmentFilter));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int orderId = reader.GetInt32(0);
                                depOrderIds.Add(orderId);
                            }
                        }
                    }
                }

                // build a new list that only contains orders for this department
                List<Order> filteredOrders = new List<Order>();

                foreach (Order order in Orders)
                {
                    if (depOrderIds.Contains(order.ID))
                    {
                        filteredOrders.Add(order);
                    }
                }

                Orders = filteredOrders;
            }
        }

        // NEW: applies status filter only on today's orders
        // Dette opfylder user story "filter by status på dagens ordre"
        private void ApplyStatusTodayFilter()
        {
            if (!string.IsNullOrEmpty(statusFilter))
            {
                List<Order> filteredOrders = new List<Order>();

                DateTime today = DateTime.Today;

                foreach (Order order in Orders)
                {
                    string currentStatus = order.Status.ToString();

                    // vi viser kun ordrer, hvor status matcher og datoen for OrderDoneBy er i dag
                    if (currentStatus == statusFilter && order.OrderDoneBy.Date == today)
                    {
                        filteredOrders.Add(order);
                    }
                }

                Orders = filteredOrders;
            }
        }

        // runs when the cancel form is posted
        public IActionResult OnPostCancel(int orderId)
        {
            // call service to cancel the order
            _orderService.CancelOrder(orderId);

            // set status message so admin can see what happened
            StatusMessage = "Ordre #" + orderId + " er blevet annulleret.";

            // after a post we redirect to get so the page reloads clean
            return RedirectToPage();
        }
    }
}
