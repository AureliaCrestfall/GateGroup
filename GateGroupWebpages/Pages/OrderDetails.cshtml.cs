using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gategourmetLibrary.Models;
using gategourmetLibrary.Secret;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using gategourmetLibary.Models;

namespace GateGroupWebpages.Pages
{
    public class OrderDetailsModel : PageModel
    {
        //it holds the order 
        public Order Order { get; set; }
        //it holds the recipe parts details for the order
        public List<RecipePartDetails> RecipeParts { get; set; } = new List<RecipePartDetails>(); //Initialize to avoid null reference and it means (= new List<string>(); )

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
                            String strStatus = reader["O_status"].ToString();
                            Order = new Order
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
                                OrderMade = reader.GetDateTime(reader.GetOrdinal("O_Made")),
                                OrderDoneBy = reader.GetDateTime(reader.GetOrdinal("O_Ready")),
                                paystatus = reader.GetBoolean(reader.GetOrdinal("O_PaySatus")),
                                // Convert O_status to enum OrderStatus
                                Status = GetStatus(strStatus)
                            };
                        }
                    }
                }

                // SQL query to get recipe parts for the order
                string sqlParts = @"SELECT RP.R_ID, RP.R_Name, RP.R_HowToPrep
                                FROM orderTableRecipePart OTP
                                JOIN RecipePart RP ON OTP.R_ID = RP.R_ID
                                WHERE OTP.O_ID = @id";
                using (SqlCommand command = new SqlCommand(sqlParts, conn))
                    {
                    command.Parameters.AddWithValue("@id", orderId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Add each recipe part to the list
                            RecipeParts.Add(new RecipePartDetails
                            {
                                R_ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                HowToPrep = reader.GetString(2),
                            });
                        }
                    }
                }
                foreach (var part in RecipeParts)
                {
                    // SQL query to get ingredients for each recipe part
                    string sqlIngredients = @"SELECT I.I_Name
                                            FROM IngrefientrecipePart IRP
                                            JOIN ingredient I ON IRP.I_ID = I.I_ID
                                            WHERE IRP.R_ID = @rid";
                    using (SqlCommand command = new SqlCommand(sqlIngredients, conn))
                    {
                        command.Parameters.AddWithValue("@rid", part.R_ID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                part.Ingredients.Add(reader.GetString(0));
                            }
                        }
                    }
                }
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
