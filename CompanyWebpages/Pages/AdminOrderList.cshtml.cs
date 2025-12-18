using System;
using System.Collections.Generic;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyWebpages.Pages
{
    // Admin page that shows all orders and allows cancelling 
    public class AdminOrderListModel : PageModel
    {
        // list with all orders to show in the table
        public List<Order> Orders { get; set; } = new List<Order>();

        // status message shown after cancel
        public string StatusMessage { get; set; } = string.Empty;
        // error message if something goes wrong
        public string ErrorMessage { get; set; } = string.Empty;

        // list of customers for the filter dropdown
        public List<Customer> Customers { get; set; } = new List<Customer>();

        // bind property to filter from query string (employee dropdown)
        [BindProperty(SupportsGet = true)]
        public string? empFilter { get; set; }

        // bind property to date range
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        // selected customer ID for filtering 
        [BindProperty(SupportsGet = true)]
        public int? SelectedCustomerId { get; set; }

        // dropdown list of employees
        public List<SelectListItem> Filter { get; set; } = new List<SelectListItem>();

        // bind property to get department filter from query string (department dropdown)
        [BindProperty(SupportsGet = true)]
        public string? departmentFilter { get; set; }

        // dropdown list of departments
        public List<SelectListItem> DepartmentFilter { get; set; } = new List<SelectListItem>();

        // bind property to get status filter for user story "filter by status today"
        [BindProperty(SupportsGet = true)]
        public string? statusFilter { get; set; }

        // service that handles order logic
        private readonly OrderService _orderService;

        // service that handles customer logic
        private readonly CustomerService _customerService;

        // service that handles employee logic
        private readonly EmployeeService _employeeService;

        // service that handles department logic
        private readonly DepartmentService _departmentService;

        // constructor uses DI (services are registered in CompanyWebpages/Program.cs)
        public AdminOrderListModel(
            OrderService orderService,
            CustomerService customerService,
            EmployeeService employeeService,
            DepartmentService departmentService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _employeeService = employeeService;
            _departmentService = departmentService;
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
                ErrorMessage = "Error loading customers:" + ex.Message;
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
                ErrorMessage = "Error loading orders:" + ex.Message;
                Orders = new List<Order>();
            }
        }

        // helper method that loads all employees into filter list
        private void LoadEmployeeFilter()
        {
            Filter = new List<SelectListItem>();

            try
            {
                Dictionary<int, string> employees = _employeeService.GetEmployeesForFilter();

                foreach (var employee in employees)
                {
                    SelectListItem item = new SelectListItem(
                        employee.Value,
                        employee.Key.ToString()
                    );

                    Filter.Add(item);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading employees:" + ex.Message;
                Filter = new List<SelectListItem>();
            }
        }

        // helper method that loads all departments into DepartmentFilter list
        private void LoadDepartmentFilter()
        {
            DepartmentFilter = new List<SelectListItem>();

            try
            {
                List<Department> departments = _departmentService.GetAllDepartments() ?? new List<Department>();

                // Keep the same behavior as the old SQL: ORDER BY D_Name
                departments.Sort((a, b) => string.Compare(a?.DepartmentName, b?.DepartmentName, StringComparison.OrdinalIgnoreCase));

                foreach (Department dep in departments)
                {
                    SelectListItem item = new SelectListItem(
                        dep.DepartmentName,
                        dep.DepartmentId.ToString()
                    );

                    DepartmentFilter.Add(item);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading departments:" + ex.Message;
                DepartmentFilter = new List<SelectListItem>();
            }
        }

        // applies employee filter if empFilter has a value
        private void ApplyEmployeeFilter()
        {
            if (!string.IsNullOrEmpty(empFilter))
            {
                try
                {
                    int employeeId = int.Parse(empFilter);
                    List<int> empOrdersIds = _employeeService.GetOrderIdsByEmployeeId(employeeId);
                    Orders = _orderService.FilterOrdersByOrderIds(Orders, empOrdersIds);
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Error filtering by employee:" + ex.Message;
                }
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
            Orders = _orderService.FilterOrdersByCreatedDateRange(Orders, FromDate, ToDate);
        }

        // applies status filter only on todays orders
        private void ApplyStatusTodayFilter()
        {
            if (!string.IsNullOrEmpty(statusFilter))
            {
                Orders = _orderService.FilterOrdersByStatusForOrdersCreatedToday(Orders, statusFilter);
            }
        }


        // runs when the cancel form is posted
        public IActionResult OnPostCancel(int orderId)
        {
            _orderService.CancelOrder(orderId);

            StatusMessage = "Order #" + orderId + "has been cancelled.";

            return RedirectToPage();
        }
        public IActionResult OnPostComplete(int orderId)
        {
            try
            {
                _orderService.MarkorderDone(orderId);
                LoadCustomers();
                LoadAllOrders();
                LoadEmployeeFilter();
                LoadDepartmentFilter();
                StatusMessage = "Order #" + orderId + "has been marked Completed.";
                return Page();
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                LoadCustomers();
                LoadAllOrders();
                LoadEmployeeFilter();
                LoadDepartmentFilter();
                return Page();
            }


            
        }
    }
}
