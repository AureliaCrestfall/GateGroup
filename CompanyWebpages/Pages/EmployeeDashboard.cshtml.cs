using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using gategourmetLibrary.Models;
using Microsoft.Data.SqlClient;
using gategourmetLibrary.Secret;
using Microsoft.AspNetCore.Http;

namespace CompanyWebpages.Pages
{
    // Page model for the employee dashboard
    // This page shows all tasks, recipe parts assigned to the logged in employee
    public class EmployeeDashboardModel : PageModel
    {
        // list of all tasks that will be shown on the page
        public List<EmployeeTask> Tasks { get; set; }

        // runs when the page is loaded with a get request
        public IActionResult OnGet()
        {
            // read user id from session employee id here
            string userIdString = HttpContext.Session.GetString("userid");
            string isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");

            // if user is not logged in or id is missing redirect to login
            if (string.IsNullOrEmpty(userIdString) || isLoggedIn != "true")
            {
                return RedirectToPage("/EmployeeLogin");
            }

            // convert user id from a string to an int
            int employeeId = Convert.ToInt32(userIdString);

            // create empty list that we will fill with tasks from database
            Tasks = new List<EmployeeTask>();

            // get connection string from secret class
            string connectionString = new Connect().cstring;

            // open a new sql connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open(); // open database connection

                // sql that gets order id, recipe part id, name, warehouse type and status
                string sql =
                    @"SELECT EmployeeRecipePartOrderTable.O_ID, EmployeeRecipePartOrderTable.R_ID, rp.R_Name, w.W_Type, rp.R_Status
                    FROM EmployeeRecipePartOrderTable 
                    INNER JOIN RecipePart rp 
                    ON EmployeeRecipePartOrderTable.R_ID = rp.R_ID
                    LEFT JOIN werehouseRecipePart wrp 
                    ON EmployeeRecipePartOrderTable.R_ID = wrp.R_ID
                    LEFT JOIN warehouse w 
                    ON wrp.W_ID = w.W_ID
                    WHERE EmployeeRecipePartOrderTable.E_ID = @employeeId";

                // prepare sql command
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // insert employee id in sql parameter
                    command.Parameters.AddWithValue("@employeeId", employeeId);

                    // execute sql and get result set
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // read each row one at a time
                        while (reader.Read())
                        {
                            // create new task object for this row
                            EmployeeTask task = new EmployeeTask();

                            // set order id
                            task.OrderId = reader.GetInt32(reader.GetOrdinal("O_ID"));

                            // set recipe part id
                            task.RecipePartId = reader.GetInt32(reader.GetOrdinal("R_ID"));

                            // set task name (recipe part name)
                            task.TaskName = reader["R_Name"].ToString();

                            // get index for W_Type (warehouse type)
                            int locationIndex = reader.GetOrdinal("W_Type");

                            // checks if there is a warehouse type value
                            if (!reader.IsDBNull(locationIndex))
                            {
                                // here we store the warehouse type, example: Freezer, Fridge, Dry
                                task.Location = reader.GetString(locationIndex);
                            }
                            else
                            {
                                // if there is no warehouse assigned yet
                                task.Location = "Not registered";
                            }

                            // get index for R_Status 
                            int statusIndex = reader.GetOrdinal("R_Status");

                            // check if status has a value
                            if (!reader.IsDBNull(statusIndex))
                            {
                                string statusText = reader.GetString(statusIndex);

                                OrderStatus orderStatus;

                                // try to convert status text to enum
                                if (Enum.TryParse<OrderStatus>(statusText, out orderStatus))
                                {
                                    task.Status = orderStatus;
                                }
                                else
                                {
                                    task.Status = OrderStatus.Created;
                                }
                            }
                            else
                            {
                                task.Status = OrderStatus.Created;
                            }

                            // set IsCompleted to true if status is completed
                            task.IsCompleted = (task.Status == OrderStatus.Completed);

                            // add task to list
                            Tasks.Add(task);
                        }
                    }
                }
            }

            // return page with tasks list filled
            return Page();
        }

        // post handler when employee clicks "mark done"
        public IActionResult OnPostMarkDone(int orderId, int recipePartId)
        {
            // read user id from session
            string userIdString = HttpContext.Session.GetString("userid");
            string isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");

            // if not logged in, send back to login page
            if (string.IsNullOrEmpty(userIdString) || isLoggedIn != "true")
            {
                return RedirectToPage("/EmployeeLogin");
            }

            int employeeId = Convert.ToInt32(userIdString);

            // get connection string
            string connectionString = new Connect().cstring;

            // open sql connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // sql that updates recipe part status to completed
                string sql =
                    @"UPDATE rp
                    SET rp.R_Status = @status
                    FROM RecipePart rp
                    INNER JOIN EmployeeRecipePartOrderTable ert ON rp.R_ID = ert.R_ID
                    WHERE ert.E_ID = @employeeId 
                    AND ert.O_ID = @orderId 
                    AND ert.R_ID = @recipePartId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@status", OrderStatus.Completed.ToString());
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    command.Parameters.AddWithValue("@orderId", orderId);
                    command.Parameters.AddWithValue("@recipePartId", recipePartId);

                    command.ExecuteNonQuery();
                }
            }

            // reload page so updated status is shown
            return RedirectToPage();
        }
    }
}
