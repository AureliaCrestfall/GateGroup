using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gategourmetLibrary.Models;
using gategourmetLibrary.Secret;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace GateGroupWebpages.Pages
{
    public class DashboardModel : PageModel
    {
        //list to hold orders
        public List<Order> Orders { get; set; }

        //it runs when the page is loaded
        public void OnGet()
        {
            Orders = new List<Order>() ;
            
            //get constring from connect class
            string connectionString = new Connect().cstring;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //open connection
                conn.Open();

                // SQL query to select orders
                string sql = @"SELECT O_ID, O_Made, O_Ready, O_PaySatus FROM OrderTable";

                //execute command
                using (SqlCommand command = new SqlCommand(sql, conn))
                //read data
                using (SqlDataReader reader = command.ExecuteReader())

                {
                    //loop through the data
                    while (reader.Read())
                    {
                        //create order object
                        Order order = new Order
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
                                OrderMade = reader.GetDateTime(reader.GetOrdinal("O_Made")),
                                OrderDoneBy = reader.GetDateTime(reader.GetOrdinal("O_Ready")),
                                paystatus = reader.GetBoolean(reader.GetOrdinal("O_PaySatus")),
                            };

                        // set order status
                        order.Status= GetStatus(order);
                        //add order to list
                        Orders.Add(order);
                    }
                }
            }

        }

        //delete handler/method
        public IActionResult OnPostDelete(int ID)
        {
            // get constring from connect class
            string connectionString = new Connect().cstring;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // open connection
                conn.Open();
                string sql2 = @" DELETE FROM orderTableRecipePart WHERE O_ID =@id";
                using (SqlCommand command = new SqlCommand(sql2, conn))
                {
                    command.Parameters.AddWithValue("@id", ID);
                    command.ExecuteNonQuery();
                }
                string sql3 = @" DELETE FROM OrderTableCustomer WHERE O_ID =@id";
                using (SqlCommand command = new SqlCommand(sql3, conn))
                {
                    command.Parameters.AddWithValue("@id", ID);
                    command.ExecuteNonQuery();
                }

                string sql4 = @" DELETE FROM EmployeeRecipePartOrderTable WHERE O_ID =@id";
                using (SqlCommand command = new SqlCommand(sql4, conn))
                {
                    command.Parameters.AddWithValue("@id", ID);
                    command.ExecuteNonQuery();
                }


                string sql = @" DELETE FROM OrderTable WHERE O_ID =@id";

                //execute command
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@id", ID);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToPage();

        }



        //logic to get order status
        private OrderStatus GetStatus (Order order)
        {
            DateTime now = DateTime.Now;

            // cancelled logic if it has not been ready in 24 hours
            if ( now < order.OrderMade && (now - order.OrderMade).TotalHours > 24)
            {
                return OrderStatus.Cancelled;
            }

            // created logic
            if (now < order.OrderMade)
            {
                return OrderStatus.Created;
            }

            // in progress logic
            if (now >= order.OrderMade && now <= order.OrderDoneBy)
            {
                return OrderStatus.InProgress;
            }

            // completed logic
            if (now > order.OrderDoneBy)
            {
                return OrderStatus.Completed;
            }
            return OrderStatus.Created;

        }
    }
}
