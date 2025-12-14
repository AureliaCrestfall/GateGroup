using System;
using System.Collections.Generic;
using gategourmetLibrary.Models;
using gategourmetLibrary.Repo;
using gategourmetLibrary.Secret;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GateGroupWebpages.Pages
{
    // Customer dashboard page that shows the logged-in customer's orders.
    // Filtering is done without LINQ or lambda expressions.
    public class DashboardModel : PageModel
    {
        // Orders shown in the table
        public List<Order> Orders { get; set; }

        // Dropdown options for status filter
        public List<SelectListItem> StatusOptions { get; set; }

        // Selected status filter from query string
        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        // Service used to access orders
        private readonly OrderService _orderService;

        public DashboardModel()
        {
            // Manual service setup (no dependency injection)
            string connectionString = new Connect().cstring;
            IOrderRepo orderRepo = new OrderRepo(connectionString);
            _orderService = new OrderService(orderRepo);

            Orders = new List<Order>();
            StatusOptions = new List<SelectListItem>();
        }

        public IActionResult OnGet()
        {
            // User must be logged in
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToPage("/Login");
            }

            // Default filter
            if (string.IsNullOrEmpty(StatusFilter))
            {
                StatusFilter = "Created";
            }

            // Build dropdown (values match enum names)
            StatusOptions.Add(new SelectListItem("Created", "Created"));
            StatusOptions.Add(new SelectListItem("In progress", "InProgress"));
            StatusOptions.Add(new SelectListItem("Completed", "Completed"));
            StatusOptions.Add(new SelectListItem("Cancelled", "Cancelled"));

            // Get customer id from session
            string userIdFromSession = HttpContext.Session.GetString("userid");
            if (string.IsNullOrEmpty(userIdFromSession))
            {
                return RedirectToPage("/Login");
            }

            int customerId = Convert.ToInt32(userIdFromSession);

            // Get only this customer's orders
            List<Order> customerOrders = _orderService.GetOrdersByCustomerId(customerId);

            // Convert filter text to enum
            OrderStatus selectedStatus = ParseStatus(StatusFilter);

            // Filter orders without LINQ
            Orders = new List<Order>();
            for (int i = 0; i < customerOrders.Count; i++)
            {
                if (customerOrders[i].Status == selectedStatus)
                {
                    Orders.Add(customerOrders[i]);
                }
            }

            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            // User must be logged in
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToPage("/Login");
            }

            // Delete order
            _orderService.DeleteOrder(id);

            // Keep current filter
            return RedirectToPage(new { StatusFilter = StatusFilter });
        }

        // Converts dropdown value to OrderStatus enum
        private OrderStatus ParseStatus(string statusText)
        {
            if (statusText == "Created")
            {
                return OrderStatus.Created;
            }
            if (statusText == "InProgress")
            {
                return OrderStatus.InProgress;
            }
            if (statusText == "Completed")
            {
                return OrderStatus.Completed;
            }
            if (statusText == "Cancelled")
            {
                return OrderStatus.Cancelled;
            }

            // Default fallback
            return OrderStatus.Created;
        }
    }
}
