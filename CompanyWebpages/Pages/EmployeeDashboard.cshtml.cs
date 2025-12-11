using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using gategourmetLibrary.Models;
using Microsoft.Data.SqlClient;
using gategourmetLibrary.Secret;
using Microsoft.AspNetCore.Http;
using gategourmetLibrary.Service;
using gategourmetLibrary.Repo;

namespace CompanyWebpages.Pages
{
    // Page model for medarbejderens dashboard
    // Her styrer jeg alt det, der skal vises på "My tasks" siden
    // Henter tasks til den medarbejder der er logget ind
    // Opdaterer status (Mark done)
    // Opdaterer storage location (Freezer, Fridge, Dry) via et dropdown bar
    public class EmployeeDashboardModel : PageModel
    {
        // Liste med alle tasks, som vises i tabellen på siden
        // Hver task svarer til en recipe part på en ordre
        public List<EmployeeTask> Tasks { get; set; }

        // Liste med alle warehouses, som jeg bruger i dropdownen til storage
        // Det er her vi kan vælge om noget står i freezer, fridge eller dry storage
        public List<Warehouse> Warehouses { get; set; }

        // Service lag der håndterer logik omkring orders og warehouses
        private readonly OrderService orderService;

        // I konstruktøren opretter jeg service objektet
        // så jeg kan kalde det senere i mine metoder
        public EmployeeDashboardModel()
        {
            string connectionString = new Connect().cstring;
            IOrderRepo orderRepo = new OrderRepo(connectionString);
            orderService = new OrderService(orderRepo);
        }

        // OnGet kører når siden bliver hentet første gang
        // Her tjekker jeg om medarbejderen er logget ind og hvis ja så hentes alle tasks til deres dashboard
        public IActionResult OnGet()
        {
            // læser employee id og login flag fra session
            string userIdString = HttpContext.Session.GetString("userid");
            string isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");

            // hvis der ikke er nogen bruger i session eller ikke logget ind sendes man tilbage til login siden
            if (string.IsNullOrEmpty(userIdString) || isLoggedIn != "true")
            {
                return RedirectToPage("/EmployeeLogin");
            }

            // konverterer employee id fra string til int
            int employeeId = Convert.ToInt32(userIdString);

            // laver en tom liste, som fylder med tasks fra databasen
            Tasks = new List<EmployeeTask>();

            // henter connection string fra secretk lassen
            string connectionString = new Connect().cstring;

            // åbner en ny sqlconnection til databasen
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL der henter - ordre id,recipe part id, navn på recipe part, warehouse type (Freezer/Fridge/Dry) og status på recipe part
                string sql =
                    @"SELECT EmployeeRecipePartOrderTable.O_ID,  EmployeeRecipePartOrderTable.R_ID, rp.R_Name, w.W_Type, rp.R_Status
                    FROM EmployeeRecipePartOrderTable 
                    INNER JOIN RecipePart rp 
                    ON EmployeeRecipePartOrderTable.R_ID = rp.R_ID
                    LEFT JOIN werehouseRecipePart wrp 
                    ON EmployeeRecipePartOrderTable.R_ID = wrp.R_ID
                    LEFT JOIN warehouse w 
                    ON wrp.W_ID = w.W_ID
                     WHERE EmployeeRecipePartOrderTable.E_ID = @employeeId";

                // gør sql ommandoen klar
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // sætter employee id ind i @employeeId-parameteren
                    command.Parameters.AddWithValue("@employeeId", employeeId);

                    // kører sql og får resultatet tilbage som en reader
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // læser en række ad gangen fra databasen
                        while (reader.Read())
                        {
                            // opretter nyt task objekt til denne række
                            EmployeeTask task = new EmployeeTask();

                            // sætter order id
                            task.OrderId = reader.GetInt32(reader.GetOrdinal("O_ID"));

                            // sætter recipe part id
                            task.RecipePartId = reader.GetInt32(reader.GetOrdinal("R_ID"));

                            // sætter navnet på recipe part (taskens navn)
                            task.TaskName = reader["R_Name"].ToString();

                            // finder kolonnen med warehouse type
                            int locationIndex = reader.GetOrdinal("W_Type");

                            // tjekker om der faktisk er en warehouse-værdi
                            if (!reader.IsDBNull(locationIndex))
                            {
                                // gemmer typen, fx "freezer", "fridge" eller "dry"
                                task.Location = reader.GetString(locationIndex);
                            }
                            else
                            {
                                // hvis recipe part endnu ikke har fået en location
                                task.Location = "Not registered";
                            }

                            // finder kolonnen med status på recipe part
                            int statusIndex = reader.GetOrdinal("R_Status");

                            // tjekker om der er en status
                            if (!reader.IsDBNull(statusIndex))
                            {
                                string statusText = reader.GetString(statusIndex);

                                OrderStatus orderStatus;

                                // prøver at konvertere tekst til enum værdien i OrderStatus
                                if (Enum.TryParse<OrderStatus>(statusText, out orderStatus))
                                {
                                    task.Status = orderStatus;
                                }
                                else
                                {
                                    // hvis noget går galt sætter jeg den til created som standard
                                    task.Status = OrderStatus.Created;
                                }
                            }
                            else
                            {
                                // hvis der slet ikke er nogen status sætter jeg også created
                                task.Status = OrderStatus.Created;
                            }

                            // her laver jeg et lille flag så jeg nemt kan tjekke om tasken er færdig
                            task.IsCompleted = (task.Status == OrderStatus.Completed);

                            // til sidst lægger jeg tasken ned i listen
                            Tasks.Add(task);
                        }
                    }
                }
            }

            // henter alle warehouses til dropdownen 
            Warehouses = orderService.GetAllWarehouses();

            // returnerer siden med udfyldt 
            return Page();
        }

        // metoden kører, når medarbejderen trykker på "Mark done" knappen
        // her opdateres status på den valgte recipe part til "completed" i databasen
        public IActionResult OnPostMarkDone(int orderId, int recipePartId)
        {
            // læser employee id og login flag fra session igen
            string userIdString = HttpContext.Session.GetString("userid");
            string isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");

            // hvis brugeren ikke er logget ind, sendes de tilbage til login
            if (string.IsNullOrEmpty(userIdString) || isLoggedIn != "true")
            {
                return RedirectToPage("/EmployeeLogin");
            }

            int employeeId = Convert.ToInt32(userIdString);

            // henter connection string
            string connectionString = new Connect().cstring;

            // åbner sql connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // sql der sætter R_Status til completed
                // for den recipe part, som matcher employee, ordre og recipe part id
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

            // efter opdateringen loader jeg siden igen,
            // så medarbejderen kan se, at status nu er ændret til completed
            return RedirectToPage();
        }

        // Denne metode kører når medarbejderen vælger et nyt warehouse
        // i dropdownen på dashboardet og trykker "Update"
        // Her bruger jeg mit service lag til at opdatere lagerplaceringen på recipe part
        public IActionResult OnPostUpdateLocation(int recipePartId, int selectedWarehouseId)
        {
            // tjekker om id er giver mening
            // Hvis ikke gør jeg ingenting og loader bare siden igen
            if (recipePartId <= 0 || selectedWarehouseId <= 0)
            {
                return RedirectToPage();
            }

            // kalder OrderService, som opdaterer werehouseRecipePart tabellen
            // så denne recipe part nu peger på det valgte warehouse
            orderService.UpdateRecipePartLocation(recipePartId, selectedWarehouseId);

            // loader dashboardet igen, så medarbejderen kan se den nye location med det samme
            return RedirectToPage();
        }
    }
}
