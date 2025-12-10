using System.Collections.Generic;
using gategourmetLibrary.Models;
using gategourmetLibrary.Repo;
using gategourmetLibrary.Secret;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CompanyWebpages.Pages
{
    // admin page that shows all orders and allows cancelling them
    public class AdminOrderListModel : PageModel
    {
        // list with all not cancelled orders to show in the table
        public List<Order> Orders { get; set; }

        // status message shown after cancel
        public string StatusMessage { get; set; }

        // service that handles order logic
        private readonly OrderService _orderService;

        // constructor creates repository and service
        public AdminOrderListModel()
        {
            string connectionString = new Connect().cstring;
            IOrderRepo orderRepo = new OrderRepo(connectionString);
            _orderService = new OrderService(orderRepo);
        }

        // hjælper method that loads only non cancelled orders
        private void LoadActiveOrders()
        {
            List<Order> allOrders = _orderService.GetAllOrders();
            Orders = new List<Order>();

            foreach (Order order in allOrders)
            {
                if (order.Status != OrderStatus.Cancelled)
                {
                    Orders.Add(order);
                }
            }
        }

        // runs when the page is loaded with a get metode 
        public void OnGet()
        {
            LoadActiveOrders();
        }

        // runs when the cancel form is posted
        public IActionResult OnPostCancel(int orderId)
        {
            // call service to cancel the order
            _orderService.CancelOrder(orderId);

            // set status message so admin can see what happened
            StatusMessage = "Cancelled order with ID: " + orderId;

            // after a POST we redirect to GET so the page reloads clean
            return RedirectToPage();
        }
    }
}
