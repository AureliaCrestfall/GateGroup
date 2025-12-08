using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gategourmetLibrary.Models;
using gategourmetLibrary.Secret;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace GateGroupWebpages.Pages
{
    public class OrderDetailsModel : PageModel
    {
        //it holds the order 
        public Order Order { get; set; }

        //bind property to get orderId from query string
        [BindProperty(SupportsGet = true)]
        public int orderId { get; set; }

        //it runs when the page is loaded (Get request)
        public void OnGet()
        {
            //get constring(DB) from connect class
            string connectionString = new Connect().cstring;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //open connection
                conn.Open();
                // SQL query to select order by id
                string sql = @"SELECT O_ID, O_Made, O_Ready, O_PaySatus ,O_status 
                               FROM OrderTable 
                               WHERE O_ID = @id"; 
                //execute command
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    //add parameter value
                    command.Parameters.AddWithValue("@id", orderId);
                    //read data
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //if order found
                        if (reader.Read())
                        {
                            Order = new Order
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
                                OrderMade = reader.GetDateTime(reader.GetOrdinal("O_Made")),
                                OrderDoneBy = reader.GetDateTime(reader.GetOrdinal("O_Ready")),
                                paystatus = reader.GetBoolean(reader.GetOrdinal("O_PaySatus")),
                                // Convert O_status to enum OrderStatus
                                Status = GetStatus(Convert.ToInt32(reader["O_status"]))
                            };
                        }
                    }
                }
            }

        }
        //method to convert int to OrderStatus enum
        private OrderStatus GetStatus(int status)
        {
            return status switch
            {
                // these numbers map to the enum values on the database
                -1 => OrderStatus.Cancelled, // -1 for Cancelled
                0 => OrderStatus.Created, // 0 for Created
                1 => OrderStatus.InProgress, // 1 for InProgress
                2 => OrderStatus.Completed, // 2 for Completed
                _ => OrderStatus.Created,  // default case
            };
        }

        //delete handler/method
        public IActionResult OnPostDelete(int orderId)
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
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }
                string sql3 = @" DELETE FROM OrderTableCustomer WHERE O_ID =@id";
                using (SqlCommand command = new SqlCommand(sql3, conn))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }

                string sql4 = @" DELETE FROM EmployeeRecipePartOrderTable WHERE O_ID =@id";
                using (SqlCommand command = new SqlCommand(sql4, conn))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }


                string sql = @" DELETE FROM OrderTable WHERE O_ID =@id";

                //execute command
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }
            }
            //redirect to dashboard after deletion
            return RedirectToPage("/Dashboard");

        }



    }
}
