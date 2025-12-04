using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using gategourmetLibrary.Models;
using Microsoft.Data.SqlClient;
using gategourmetLibrary.Secret;


namespace CompanyWebpages.Pages
{
    public class EmployeeDashboardModel : PageModel
    {
        // list that holds all tasks to show in the table
        public List<EmployeeTask> Tasks { get; set; }

        // runs when the page is loaded with On get methods
        public void OnGet()
        {
            // create empty list 
            Tasks = new List<EmployeeTask>();

            // for now we use one fixed employee(id = 1)
            int employeeId = 1;

            string connectionString = new Connect().cstring;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                   
                string sql =
                    @" SELECT EmployeeRecipePartOrderTable.O_ID, EmployeeRecipePartOrderTable.R_ID, rp.R_Name, w.W_Location,rp.R_Status
                    FROM EmployeeRecipePartOrderTable 
                    INNER JOIN RecipePart rp ON EmployeeRecipePartOrderTable.R_ID = rp.R_ID
                    LEFT JOIN werehouseRecipePart wrp ON EmployeeRecipePartOrderTable.R_ID = wrp.R_ID
                    LEFT JOIN warehouse w ON wrp.W_ID = w.W_ID
                    WHERE EmployeeRecipePartOrderTable.E_ID = @employeeId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@employeeId", employeeId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())

                        {
                            EmployeeTask task = new EmployeeTask();

                            task.OrderId = reader.GetInt32(reader.GetOrdinal("O_ID"));
                            task.RecipePartId = reader.GetInt32(reader.GetOrdinal("R_ID"));
                            task.TaskName = reader["R_Name"].ToString();

                            int locationIndex = reader.GetOrdinal("W_Location");
                            if (!reader.IsDBNull(locationIndex))
                            {
                                task.Location = reader.GetString(locationIndex);
                            }
                            else
                            {
                                task.Location = "Not registered";
                            }

                            int completedIndex = reader.GetOrdinal("R_Status");
                            //task.IsCompleted =
                            //    !reader.IsDBNull(completedIndex) &&
                            //    reader.GetBoolean(completedIndex);

                            Tasks.Add(task);
                        }
                    }


                }
            }
        }
        public IActionResult OnPostMarkDone(int orderId, int recipePartId)
        {
            int employeeId = 1;

            string connectionString = new Connect().cstring;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql =
                     @"UPDATE EmployeeRecipePartOrderTable
                     SET IsCompleted = 1
                     WHERE E_ID = @employeeId AND O_ID = @orderId AND R_ID = @recipePartId";

                using (SqlCommand command = new SqlCommand(sql, connection)) 
                {
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    command.Parameters.AddWithValue("@orderId", orderId);
                    command.Parameters.AddWithValue("@recipePartId", recipePartId);

                    command.ExecuteNonQuery();
                }
            }
            return RedirectToPage();
        }
    }
}
