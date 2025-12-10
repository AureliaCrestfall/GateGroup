using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using gategourmetLibrary.Repo;
using gategourmetLibrary.Secret;
using System.Collections.Generic;

namespace CompanyWebpages.Pages
{
    // Page where an employee can update the warehouse location, for a specific recipe part of their task
    public class EmployeeTaskLocationModel : PageModel
    {
        // all possible warehouse choices freezer, fridge, dry storage
        public List<Warehouse> Warehouses { get; set; }

        // current location of this recipe part
        public Warehouse CurrentLocation { get; set; }

        // id of the recipe part task 
        [BindProperty]
        public int RecipePartId { get; set; }

        // selected warehouse from dropdown
        [BindProperty]
        public int SelectedWarehouseId { get; set; }

        private readonly OrderService _orderService;

        public EmployeeTaskLocationModel()
        {
            string connectionString = new Connect().cstring;
            IOrderRepo orderRepo = new OrderRepo(connectionString);
            _orderService = new OrderService(orderRepo);
        }

        // loads page with current data
        public void OnGet(int recipePartId)
        {
            RecipePartId = recipePartId;

            // list of all warehouses to show in dropdown
            Warehouses = _orderService.GetAllWarehouses();

            // current location of this recipe part
            CurrentLocation = _orderService.GetRecipePartLocation(recipePartId);
        }

        // handles the post method when employee updates location
        public IActionResult OnPostUpdateLocation()
        {
            _orderService.UpdateRecipePartLocation(RecipePartId, SelectedWarehouseId);

            // after update, reload same page so employee can see new location
            return RedirectToPage(new { recipePartId = RecipePartId });
        }
    }
}
