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
    // Admin page that shows all orders and allows cancelling 
    public class AdminOrderListModel : PageModel
    {
        // list with all orders to show in the table
        public List<Order> Orders { get; set; }

        // status message shown after cancel
        public string StatusMessage { get; set; }
        // error message if something goes wrong
        public string ErrorMessage { get; set; }

        // list of customers for the filter dropdown
        public List<Customer> Customers { get; set; }

        // bind property to filter from query string (employee dropdown)
        [BindProperty(SupportsGet = true)]
        public string empFilter { get; set; }

        // bind property to date range
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        // selected customer ID for filtering 
        [BindProperty(SupportsGet = true)]
        public int? SelectedCustomerId { get; set; }

        // dropdown list of employees
        public List<SelectListItem> Filter { get; set; }

        // bind property to get department filter from query string (department dropdown)
        [BindProperty(SupportsGet = true)]
        public string departmentFilter { get; set; }

        // dropdown list of departments
        public List<SelectListItem> DepartmentFilter { get; set; }

        // bind property to get status filter for user story "filter by status today"
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
            LoadCustomers();
            LoadAllOrders();
            LoadEmployeeFilter();
            LoadDepartmentFilter();

            ApplyEmployeeFilter();
            ApplyDepartmentFilter();
            ApplyStatusTodayFilter();
        }

        // loads all customers for the dropdown
        private void LoadCustomers()
        {
            try
            {
                Customers = _customerService.GetAllCustomers();
                if (Customers == null)
                {
                    Customers = new List<Customer>();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Fejl ved indlæsning af kunder: " + ex.Message;
                Customers = new List<Customer>();
            }
        }

        // helper method that loads orders 
        private void LoadAllOrders()
        {
            try
            {
                if (SelectedCustomerId.HasValue && SelectedCustomerId.Value > 0)
                {
                    Customer selectedCustomer = _customerService.GetCustomer(SelectedCustomerId.Value);

                    if (selectedCustomer != null)
                    {
                        Orders = _orderService.FilterOrdersByCompany(selectedCustomer);
                    }
                    else
                    {
                        Orders = new List<Order>();
                    }
                }
                else
                {
                    Orders = _orderService.GetAllOrders();
                }

                if (Orders == null)
                {
                    Orders = new List<Order>();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Fejl ved indlæsning af ordrer: " + ex.Message;
                Orders = new List<Order>();
            }
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

                List<int> empOrdersIds = new List<int>();

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

        // applies department filter and optional date range
        private void ApplyDepartmentFilter()
        {
            // 1) Department filter moved to repo/service
            if (!string.IsNullOrEmpty(departmentFilter))
            {
                int departmentId = int.Parse(departmentFilter);

                Orders = _orderService.FilterOrdersByDepartment(departmentId);

                if (Orders == null)
                {
                    Orders = new List<Order>();
                }
            }

            // 2) FromDate filter
            if (FromDate.HasValue)
            {
                List<Order> fromFiltered = new List<Order>();

                foreach (Order order in Orders)
                {
                    if (order.OrderMade.Date >= FromDate.Value.Date)
                    {
                        fromFiltered.Add(order);
                    }
                }

                Orders = fromFiltered;
            }

            // 3) ToDate filter 
            if (ToDate.HasValue)
            {
                List<Order> toFiltered = new List<Order>();

                foreach (Order order in Orders)
                {
                    if (order.OrderMade.Date <= ToDate.Value.Date)
                    {
                        toFiltered.Add(order);
                    }
                }

                Orders = toFiltered;
            }
        }

        // applies status filter only on todays orders
        private void ApplyStatusTodayFilter()
        {
            if (!string.IsNullOrEmpty(statusFilter))
            {
                List<Order> filteredOrders = new List<Order>();

                foreach (Order order in Orders)
                {
                    bool isToday = order.OrderMade.Date == DateTime.Today;

                    if (isToday)
                    {
                        string currentStatus = order.Status.ToString();

                        if (currentStatus == statusFilter)
                        {
                            filteredOrders.Add(order);
                        }
                    }
                }

                Orders = filteredOrders;
            }
        }


        // runs when the cancel form is posted
        public IActionResult OnPostCancel(int orderId)
        {
            _orderService.CancelOrder(orderId);

            StatusMessage = "Ordre #" + orderId + " er blevet annulleret.";

            return RedirectToPage();
        }
    }
}
