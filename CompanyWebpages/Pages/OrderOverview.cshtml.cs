using Microsoft.AspNetCore.Mvc.RazorPages;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using gategourmetLibrary.Secret;

namespace CompanyWebpages.Pages
{
    public class OrderOverviewModel : PageModel
    {
        public List<Order> Orders { get; set; }

        public void OnGet()
        {
            Orders = new List<Order>();

            string connectionString = new Connect().cstring;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"SELECT O_ID, O_Made, O_Ready, O_PaySatus, O_Status FROM OrderTable ORDER BY O_Made DESC";

                using (SqlCommand command = new SqlCommand(sql, conn))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order order = new Order
                        {
                            ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
                            OrderMade = reader.GetDateTime(reader.GetOrdinal("O_Made")),
                            OrderDoneBy = reader.GetDateTime(reader.GetOrdinal("O_Ready")),
                            paystatus = reader.GetBoolean(reader.GetOrdinal("O_PaySatus")),
                        };

                        // Try to get status, default to Created if null or invalid
                        if (!reader.IsDBNull(reader.GetOrdinal("O_Status")))
                        {
                            string statusStr = reader.GetString(reader.GetOrdinal("O_Status"));
                            if (System.Enum.TryParse<OrderStatus>(statusStr, out OrderStatus status))
                            {
                                order.Status = status;
                            }
                            else
                            {
                                order.Status = OrderStatus.Created;
                            }
                        }
                        else
                        {
                            order.Status = OrderStatus.Created;
                        }

                        Orders.Add(order);
                    }
                }
            }
        }
    }
}

