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
        public void OnGet(int orderid)
        {
            try
            {
                Order = _orderService.GetOrder(orderid);
                if (Order == null)
                {
                    ErrorMessage = $"Order #{orderid} blev ikke fundet.";
                    Order = new Order();
                    Order.ID = orderid;
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                Order.ID = orderid;
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

            //// get constring from connect class
            //string connectionString = new Connect().cstring;
            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    // open connection
            //    conn.Open();
            //    string sql2 = @" DELETE FROM orderTableRecipePart WHERE O_ID =@id";
            //    using (SqlCommand command = new SqlCommand(sql2, conn))
            //    {
            //        command.Parameters.AddWithValue("@id", orderId);
            //        command.ExecuteNonQuery();
            //    }
            //    string sql3 = @" DELETE FROM OrderTableCustomer WHERE O_ID =@id";
            //    using (SqlCommand command = new SqlCommand(sql3, conn))
            //    {
            //        command.Parameters.AddWithValue("@id", orderId);
            //        command.ExecuteNonQuery();
            //    }

            //    string sql4 = @" DELETE FROM EmployeeRecipePartOrderTable WHERE O_ID =@id";
            //    using (SqlCommand command = new SqlCommand(sql4, conn))
            //    {
            //        command.Parameters.AddWithValue("@id", orderId);
            //        command.ExecuteNonQuery();
            //    }


            //    string sql = @" DELETE FROM OrderTable WHERE O_ID =@id";

            //    //execute command
            //    using (SqlCommand command = new SqlCommand(sql, conn))
            //    {
            //        command.Parameters.AddWithValue("@id", orderId);
            //        command.ExecuteNonQuery();
            //    }
            //}
            //redirect to dashboard after deletion
            return RedirectToPage("/Dashboard");

        }



    }
}
