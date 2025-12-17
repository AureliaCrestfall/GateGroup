using System;
using System.Collections.Generic;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CompanyWebpages.Pages
{
    // Page model for medarbejderens dashboard
    // Her styrer jeg alt det, der skal vises p� "My tasks" siden
    // Henter tasks til den medarbejder der er logget ind
    // Opdaterer status (Mark done)
    // Opdaterer storage location (Freezer, Fridge, Dry) via et dropdown bar
    public class EmployeeDashboardModel : PageModel
    {
        // Liste med alle tasks, som vises i tabellen p� siden
        // Hver task svarer til en recipe part p� en ordre
        public List<EmployeeTask> Tasks { get; set; } = new List<EmployeeTask>();

        // Liste med alle warehouses, som jeg bruger i dropdownen til storage
        // Det er her vi kan v�lge om noget st�r i freezer, fridge eller dry storage
        public List<Warehouse> Warehouses { get; set; } = new List<Warehouse>();

        // true if any task has no registered storage location
        public bool HasUnassignedLocation { get; set; }

        private readonly OrderService _orderService;
        private readonly EmployeeService _employeeService;

        // Uses DI (services are registered in CompanyWebpages/Program.cs)
        public EmployeeDashboardModel(OrderService orderService, EmployeeService employeeService)
        {
            _orderService = orderService;
            _employeeService = employeeService;
        }

        private bool TryGetLoggedInEmployeeId(out int employeeId)
        {
            employeeId = 0;

            string userIdString = HttpContext.Session.GetString("userid");
            string isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");

            if (string.IsNullOrEmpty(userIdString) || isLoggedIn != "true")
            {
                return false;
            }

            employeeId = Convert.ToInt32(userIdString);
            return true;
        }

        // OnGet k�rer n�r siden bliver hentet f�rste gang
        // Her tjekker jeg om medarbejderen er logget ind og hvis ja s� hentes alle tasks til deres dashboard
        public IActionResult OnGet()
        {
            if (!TryGetLoggedInEmployeeId(out int employeeId))
            {
                return RedirectToPage("/EmployeeLogin");
            }

            Tasks = _employeeService.GetEmployeeTasks(employeeId) ?? new List<EmployeeTask>();
            HasUnassignedLocation = false;
            foreach (EmployeeTask task in Tasks)
            {
                if (task != null && task.Location == "Not registered")
                {
                    HasUnassignedLocation = true;
                    break;
                }
            }

            // henter alle warehouses til dropdownen 
            Warehouses = _orderService.GetAllWarehouses() ?? new List<Warehouse>();

            // returnerer siden med udfyldt 
            return Page();
        }

        // metoden k�rer, n�r medarbejderen trykker p� "Mark done" knappen
        // her opdateres status p� den valgte recipe part til "completed" i databasen
        public IActionResult OnPostMarkDone(int orderId, int recipePartId)
        {
            if (!TryGetLoggedInEmployeeId(out int employeeId))
            {
                return RedirectToPage("/EmployeeLogin");
            }

            _employeeService.MarkTaskDone(employeeId, orderId, recipePartId);

            // efter opdateringen loader jeg siden igen,
            // s� medarbejderen kan se, at status nu er �ndret til completed
            return RedirectToPage();
        }

        // Denne metode k�rer n�r medarbejderen v�lger et nyt warehouse
        // i dropdownen p� dashboardet og trykker "Update"
        // Her bruger jeg mit service lag til at opdatere lagerplaceringen p� recipe part
        public IActionResult OnPostUpdateLocation(int recipePartId, int selectedWarehouseId)
        {
            // tjekker om id er giver mening
            // Hvis ikke g�r jeg ingenting og loader bare siden igen
            if (recipePartId <= 0 || selectedWarehouseId <= 0)
            {
                return RedirectToPage();
            }

            // kalder OrderService, som opdaterer werehouseRecipePart tabellen
            // s� denne recipe part nu peger p� det valgte warehouse
            _orderService.UpdateRecipePartLocation(recipePartId, selectedWarehouseId);

            // loader dashboardet igen, s� medarbejderen kan se den nye location med det samme
            return RedirectToPage();
        }
    }
}
