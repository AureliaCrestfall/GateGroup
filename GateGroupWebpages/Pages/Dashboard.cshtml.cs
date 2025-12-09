using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gategourmetLibrary.Models;
using gategourmetLibrary.Secret;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GateGroupWebpages.Pages
{
    public class DashboardModel : PageModel
    {
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
            Orders = new List<Order>() ;

            //populate dropdown list
            StatusOptions = new List<SelectListItem>
            {
                //the first part is appears to the customers,
                //the second part is the value of the choice and here its empty(NOT NULL)
                new SelectListItem("Choose Status ...", ""),
                new SelectListItem("Created", "Created"),
                new SelectListItem( "In Progress", "InProgress"),
                new SelectListItem("Completed", "Completed"),
                new SelectListItem("Cancelled", "Cancelled")
            };
            
            //get constring(DB) from connect class
            string connectionString = new Connect().cstring;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //open connection
                conn.Open();

                // SQL query to select orders
                string sql = @"SELECT O_ID, O_Made, O_Ready, O_PaySatus ,O_status FROM OrderTable";

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
                                Status = GetStatusFormating (reader["O_status"].ToString())
                        };

                        //add order to list
                        Orders.Add(order);
                    }
                }
            }

            //if user has selected a status filter, filter the orders by using LINQ
            if (!string.IsNullOrEmpty(statusFilter))
            {
                //'out' keyword allows the method to return an additional value through this variable
                if (Enum.TryParse<OrderStatus>(statusFilter, out var selectedStatus))
                {
                    // The expression 'o => o.Status == selectedStatus'
                    // is a condition (lambda) that checks each order. Only orders that match the selected
                    // status are kept. ToList() converts the LINQ result back into a List<Order>.
                    Orders = Orders.Where(o => o.Status == selectedStatus).ToList();
                }
            }
            else
            {
                // if no filter is selected, hide orderTable by clearing the list
                Orders = new List<Order>();
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
