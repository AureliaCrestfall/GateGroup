using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gategourmetLibrary.Models;
using gategourmetLibrary.Secret;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using gategourmetLibrary.Repo;
using gategourmetLibrary.Service;

namespace GateGroupWebpages.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly OrderService _orderService;

        public DashboardModel(OrderService orderService)
        {
            _orderService = orderService;
        }

        //list to hold orders
        public List<Order> Orders { get; set; }

        //bind property to get status filter from query string
        [BindProperty(SupportsGet = true)]
        // this ? makes it optional and allows null values
        public string? statusFilter { get; set; }

        //list of dropdown choices
        public List<SelectListItem> StatusOptions { get; set; }

        //it runs when the page is loaded
        public void OnGet()
        {


            Orders = _orderService.GetAllOrders();

            //default filter (Created)
            if (String.IsNullOrEmpty(statusFilter))
            {
                statusFilter = "Created";
            }

            //populate dropdown list
            StatusOptions = new List<SelectListItem>
            {
                //dropdown options
                //the first part is appears to the customers,
                //the second part is the value of the choice and here its empty(NOT NULL)
                new SelectListItem("Created", "Created"),
                new SelectListItem( "In Progress", "InProgress"),
                new SelectListItem("Completed", "Completed"),
                new SelectListItem("Cancelled", "Cancelled")
            };

           

            //if user has selected a status filter, filter the orders by using LINQ
                //'out' keyword allows the method to return an additional value through this variable
                if (Enum.TryParse<OrderStatus>(statusFilter, out var selectedStatus))
                {
                    // The expression 'o => o.Status == selectedStatus'
                    // is a condition (lambda) that checks each order. Only orders that match the selected
                    // status are kept. ToList() converts the LINQ result back into a List<Order>.
                    Orders = Orders.Where(o => o.Status == selectedStatus).ToList();
                }



        }

        //delete handler/method
        public IActionResult OnPostDelete(int ID)
        {
            _orderService.DeleteOrder(ID);
            return RedirectToPage();

        }



        //logic to get order status
        private OrderStatus GetStatusFormating(string status)
        {
            // Use a switch expression to match the string value with enum values
            return status switch

            {
                // using LAMBDA expression to map string values to enum, It returns the value on the right when the pattern on the left matches.
                "Created" => OrderStatus.Created,
                "InProgress" => OrderStatus.InProgress,
                "Completed" => OrderStatus.Completed,
                "Cancelled" => OrderStatus.Cancelled,
                // Fallback case: if an unknown value comes from the database, default to Created to avoid breaking the system.
                _ => OrderStatus.Created,

            };

        }
    }
}
