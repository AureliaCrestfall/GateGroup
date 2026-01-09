using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GateGroupWebpages.Pages
{
    public class DashboardModel : PageModel
    {
        // Service used to access order logic
        private readonly OrderService _orderService;

        [BindProperty]
        public string SuccessMessage { get; set; }
        // Constructor injection of service
        public DashboardModel(OrderService orderService)
        {
            _orderService = orderService;
        }

        // List of orders shown on the page
        public List<Order> Orders { get; set; }

        // Selected status from query string (optional)
        [BindProperty(SupportsGet = true)]
        public string statusFilter { get; set; }

        // Dropdown options for status filter
        public List<SelectListItem> StatusOptions { get; set; }

        // Runs when the page is loaded
        public IActionResult OnGet()
        {
            // Tjek om brugeren er logget ind før den giver adgang til siden 
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                // Hvis IKKE logget ind - send til login siden
                return RedirectToPage("/Login");
            }
            else
            {
                Load();
                // Hvis logget ind - vis siden som normalt
                return Page();
            }
           
        }

        // Handles delete button click
        public IActionResult OnPostDelete(int id)
        {
            _orderService.DeleteOrder(id);
            SuccessMessage = $"order with ID {id} was deleted successfully.";
            Load();
            return Page();
        }

        // Converts status string from database into OrderStatus enum
        private OrderStatus GetStatusFormatting(string status)
        {
            switch (status)
            {
                case "Created":
                    return OrderStatus.Created;

                case "InProgress":
                    return OrderStatus.InProgress;

                case "Completed":
                    return OrderStatus.Completed;

                case "Cancelled":
                    return OrderStatus.Cancelled;

                default:
                    // Fallback if database contains unexpected value
                    return OrderStatus.Created;
            }
        }
        public void Load()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            // Get all orders from service
            Orders = _orderService.GetAllOrdersFromid(id);

            // Default filter if none is selected
            if (string.IsNullOrEmpty(statusFilter))
            {
                statusFilter = "Created";
            }

            // Populate dropdown list
            StatusOptions = new List<SelectListItem>
            {
                new SelectListItem("Created", "Created"),
                new SelectListItem("In Progress", "InProgress"),
                new SelectListItem("Completed", "Completed"),
                new SelectListItem("Cancelled", "Cancelled")
            };






            //TO EXAM
            // Variable that will hold the converted enum value from the UI filter
            //OrderStatus selectedStatus;

            //// First we make sure that the Orders list is not null
            //// Then we safely try to convert the selected status (string from dropdown)
            //// into the OrderStatus enum using TryParse to avoid runtime exceptions
            //if (Orders != null && Enum.TryParse(statusFilter, out selectedStatus))
            //{
            //    // Filter the orders using LINQ
            //    // Where() iterates through the collection and keeps only
            //    // the orders whose Status matches the selected enum value
            //    Orders = Orders
            //        .Where(o => o.Status == selectedStatus)
            //        .ToList(); // Convert the result back to a List for further use in the UI
            //}







            // Try to convert selected string to OrderStatus enum
            OrderStatus selectedStatus;

            if (Enum.TryParse(statusFilter, out selectedStatus))
            {
                // Create new list for filtered orders
                List<Order> filteredOrders = new List<Order>();

                // Loop through all orders and keep matching ones
                foreach (Order order in Orders)
                {
                    if (order.Status == selectedStatus)
                    {
                        filteredOrders.Add(order);
                    }
                }


                Orders = filteredOrders;
            }
        }
    }
}
