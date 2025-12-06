using System; 
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
        // liste af alle opgaver der skal vises på siden
        public List<EmployeeTask> Tasks { get; set; }

        // kører når siden indlæses 
        public void OnGet()
        {
            // opretter en tom liste som vi fylder med opgaver fra databasen
            Tasks = new List<EmployeeTask>();

            // midlertidigt medarbejder id, indtil login virker
            int employeeId = 1;

            // henter connection string fra vores Secret klasse
            string connectionString = new Connect().cstring;

            // åbner en ny sql forbindelse
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open(); // åbner forbindelsen til databasen

                // sql der henter ordre id, recipe del, navn, lokation og status
                string sql =
                    @" SELECT EmployeeRecipePartOrderTable.O_ID, 
                    EmployeeRecipePartOrderTable.R_ID, rp.R_Name, w.W_Location, rp.R_Status
                    FROM EmployeeRecipePartOrderTable 
                    INNER JOIN RecipePart rp 
                    ON EmployeeRecipePartOrderTable.R_ID = rp.R_ID
                    LEFT JOIN werehouseRecipePart wrp 
                    ON EmployeeRecipePartOrderTable.R_ID = wrp.R_ID
                    LEFT JOIN warehouse w 
                    ON wrp.W_ID = w.W_ID
                    WHERE EmployeeRecipePartOrderTable.E_ID = @employeeId";

                // gør sql klar til at blive kørt
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // indsætter medarbejderid i sql parameter
                    command.Parameters.AddWithValue("@employeeId", employeeId);

                    // kører sql og får et resultatsæt tilbage
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // læser hver række en ad gangen
                        while (reader.Read())
                        {
                            // opretter en ny task klasse til denne række
                            EmployeeTask task = new EmployeeTask();

                            // sætter ordre id
                            task.OrderId = reader.GetInt32(reader.GetOrdinal("O_ID"));
                            // sætter recipe part id
                            task.RecipePartId = reader.GetInt32(reader.GetOrdinal("R_ID"));

                            // henter navnet på opgaven (recipe part name)
                            task.TaskName = reader["R_Name"].ToString();

                            // finder index for W_Location kolonne
                            int locationIndex = reader.GetOrdinal("W_Location");

                            // tjekker om der findes en lokation i databasen
                            if (!reader.IsDBNull(locationIndex))
                            {
                                // sætter location hvis den findes
                                task.Location = reader.GetString(locationIndex);
                            }
                            else
                            {
                                // standardtekst hvis der ikke er en lokation
                                task.Location = "Not registered";
                            }

                            // finder index for R_Status
                            int statusIndex = reader.GetOrdinal("R_Status");

                            // tjekker om status har en værdi i databasen
                            if (!reader.IsDBNull(statusIndex))
                            {
                                // læser status som tekst created eller completed
                                string statusText = reader.GetString(statusIndex);

                                OrderStatus orderStatus; // enum variabel til resultatet

                                // prøver at konvertere tekst til enum
                                if (Enum.TryParse<OrderStatus>(statusText, out orderStatus))
                                {
                                    task.Status = orderStatus; // sætter enum i task
                                }
                                else
                                {
                                    // hvis teksten ikke matcher enum navne
                                    task.Status = OrderStatus.Created;
                                }
                            }
                            else
                            {
                                // hvis status er tom i databasen
                                task.Status = OrderStatus.Created;
                            }

                            // sætter IsCompleted  true hvis enum er completed
                            task.IsCompleted = (task.Status == OrderStatus.Completed);

                            // tilføjer task til listen
                            Tasks.Add(task);
                        }
                    }
                }
            }
        }

        // POST handler når medarbejder trykker mark done
        public IActionResult OnPostMarkDone(int orderId, int recipePartId)
        {
            // midlertidigt medarbejder id
            int employeeId = 1;

            // henter connection string
            string connectionString = new Connect().cstring;

            // åbner sql forbindelsen
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open(); // åbner databaseforbindelsen

                // sql der opdaterer status på recipe part til completed
                string sql =
                    @"UPDATE rp
                    SET rp.R_Status = @status
                    FROM RecipePart rp
                    INNER JOIN EmployeeRecipePartOrderTable ert
                    ON rp.R_ID = ert.R_ID
                    WHERE ert.E_ID = @employeeId 
                    AND ert.O_ID = @orderId 
                    AND ert.R_ID = @recipePartId";

                // gør sql klar til at blive kørt
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // indsætter enum som tekst i databasen completed
                    command.Parameters.AddWithValue("@status", OrderStatus.Completed.ToString());
                    // indsætter id i sql
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    command.Parameters.AddWithValue("@orderId", orderId);
                    command.Parameters.AddWithValue("@recipePartId", recipePartId);

                    command.ExecuteNonQuery(); // kører opdateringen
                }
            }

            // loader siden igen
            return RedirectToPage();
        }
    }
}
