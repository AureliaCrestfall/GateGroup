using System.Collections.Generic;
using gategourmetLibary.Models;
using gategourmetLibrary.Models;
using gategourmetLibrary.Secret;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace GateGroupWebpages.Pages
{
    public class OrderDetailsModel : PageModel
    {
        // Service used to handle order logic

        private readonly OrderService _orderService;

        // Constructor that injects OrderService
        public OrderDetailsModel(OrderService orderService)
        {
            _orderService = orderService;
            Order = new Order();
        }
        [BindProperty]
        public string ErrorMessage { get; set; }

        //it holds the order 
        public Order Order { get; set; }
        //it holds the recipe parts details for the order
        public List<RecipePartDetails> RecipeParts { get; set; } = new List<RecipePartDetails>(); //Initialize to avoid null reference and it means (= new List<string>(); )

        //bind property to get orderId from query string
        [BindProperty(SupportsGet = true)]
        public int orderId { get; set; }

        //it runs when the page is loaded (Get request)
        public IActionResult OnGet(int orderid)
        {
            // Tjek om brugeren er logget ind før den giver adgang til siden 
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                // Hvis IKKE logget ind - send til login siden
                return RedirectToPage("/Login");
            }
            else
            {
                try
                {
                    Order = _orderService.GetOrder(orderid);
                    if (Order == null)
                    {
                        ErrorMessage = $"Order #{orderId} was not found.";
                        Order = new Order();
                        Order.ID = orderid;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    Order.ID = orderid;
                }
                // Hvis logget ind - vis siden som normalt
                return Page();
            }
            
            
            
        }
        //method to convert int to OrderStatus enum
        private OrderStatus GetStatus(string status)
        {
            // Use a switch expression to match the string value with enum values
            return status switch
            {
                // using LAMBDA expression to map string values to enum, It returns the value on the right when the pattern on the left matches.
                "Cancelled" => OrderStatus.Cancelled, 
                "Created" => OrderStatus.Created, 
                "InProgress" => OrderStatus.InProgress, 
                "Completed" => OrderStatus.Completed,
                // Fallback case: if an unknown value comes from the database, default to Created to avoid breaking the system.
                _ => OrderStatus.Created,
            };
        }

        //delete handler/method
        public IActionResult OnPostDelete(int orderId)
        {
            // call the service to delete the order
            _orderService.DeleteOrder(orderId);

            return RedirectToPage("/Dashboard");

        }



    }
}
