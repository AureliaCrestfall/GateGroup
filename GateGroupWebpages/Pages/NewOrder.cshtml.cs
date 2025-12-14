using System;
using System.Collections.Generic;
using System.Diagnostics;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GateGroupWebpages.Pages
{
    // PageModel for creating a new order with up to 5 recipe parts.
    // Loads ingredients + allergies for dropdowns and validates allergy selections before saving.
    public class NewOrderModel : PageModel
    {
        // Services used to access business logic
        private readonly CustomerService _customerService;
        private readonly OrderService _orderService;

        // The order that will be created
        [BindProperty]
        public Order NewOrder { get; set; }

        // Recipe parts (up to 5)
        [BindProperty]
        public RecipePart RecipePart1 { get; set; }

        [BindProperty]
        public RecipePart RecipePart2 { get; set; }

        [BindProperty]
        public RecipePart RecipePart3 { get; set; }

        [BindProperty]
        public RecipePart RecipePart4 { get; set; }

        [BindProperty]
        public RecipePart RecipePart5 { get; set; }

        // Selected ingredient ids (from dropdowns)
        [BindProperty]
        public int Ingredient1 { get; set; }

        [BindProperty]
        public int Ingredient2 { get; set; }

        [BindProperty]
        public int Ingredient3 { get; set; }

        [BindProperty]
        public int Ingredient4 { get; set; }

        [BindProperty]
        public int Ingredient5 { get; set; }

        // Selected allergy ids (from dropdowns)
        [BindProperty]
        public int Allergy1 { get; set; }

        [BindProperty]
        public int Allergy2 { get; set; }

        [BindProperty]
        public int Allergy3 { get; set; }

        [BindProperty]
        public int Allergy4 { get; set; }

        [BindProperty]
        public int Allergy5 { get; set; }

        // Error message shown in the UI
        [BindProperty]
        public string ErrorMessage { get; set; }

        // Dropdown options (do NOT bind from the form)
        public List<SelectListItem> IngredientOptions { get; set; }
        public List<SelectListItem> AllergyOptions { get; set; }

        // Cached ingredients (id -> ingredient) (do NOT bind from the form)
        public Dictionary<int, Ingredient> Ingredients { get; set; }

        // Constructor - initializes objects and loads dropdown data
        public NewOrderModel(OrderService orderService, CustomerService customerService)
        {
            _orderService = orderService;
            _customerService = customerService;

            // Create order object and ensure nested objects exist to avoid null references
            NewOrder = new Order();
            NewOrder.CustomerOrder = new Customer();
            NewOrder.Recipe = new Dictionary<int, RecipePart>();

            // Create recipe parts to bind inputs safely
            RecipePart1 = new RecipePart();
            RecipePart2 = new RecipePart();
            RecipePart3 = new RecipePart();
            RecipePart4 = new RecipePart();
            RecipePart5 = new RecipePart();

            // Default dates
            NewOrder.OrderMade = DateTime.Now;
            NewOrder.OrderDoneBy = DateTime.Now.AddDays(7).Date;

            // Load dropdown lists
            LoadDropdownData();
        }

        // Runs when the page is loaded (GET)
        public IActionResult OnGet()
        {
            // Check if the user is logged in
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToPage("/Login");
            }

            // Ensure dropdowns exist on first load
            LoadDropdownData();

            return Page();
        }

        // Runs when the form is submitted (POST)
        public IActionResult OnPost()
        {
            // Check login first
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToPage("/Login");
            }

            // Always reload dropdown data on postback (so the page can re-render if we return Page())
            LoadDropdownData();

            // Validate session userid (avoid FormatException)
            string userIdFromSession = HttpContext.Session.GetString("userid");

            int customerId;
            bool customerIdIsOk = int.TryParse(userIdFromSession, out customerId);

            if (!customerIdIsOk)
            {
                ErrorMessage = "Could not read your user id. Please log in again.";
                return Page();
            }

            // Always set server-side values (do NOT trust hidden fields)
            NewOrder.OrderMade = DateTime.Now;

            if (NewOrder.CustomerOrder == null)
            {
                NewOrder.CustomerOrder = new Customer();
            }

            NewOrder.CustomerOrder.ID = customerId;

            // Make sure the Recipe dictionary exists
            if (NewOrder.Recipe == null)
            {
                NewOrder.Recipe = new Dictionary<int, RecipePart>();
            }

            // Collect allergy selections (only if a recipe part is actually used)
            List<int> selectedAllergyIds = new List<int>();

            // Build order recipe dictionary based on selected ingredients
            AddRecipePartIfSelected(1, RecipePart1, Ingredient1, Allergy1, selectedAllergyIds);
            AddRecipePartIfSelected(2, RecipePart2, Ingredient2, Allergy2, selectedAllergyIds);
            AddRecipePartIfSelected(3, RecipePart3, Ingredient3, Allergy3, selectedAllergyIds);
            AddRecipePartIfSelected(4, RecipePart4, Ingredient4, Allergy4, selectedAllergyIds);
            AddRecipePartIfSelected(5, RecipePart5, Ingredient5, Allergy5, selectedAllergyIds);

            try
            {
                // Validate allergies: if selected allergy matches ingredient allergens -> block save
                foreach (KeyValuePair<int, RecipePart> recipeEntry in NewOrder.Recipe)
                {
                    RecipePart part = recipeEntry.Value;

                    if (part.Ingredients == null)
                    {
                        continue;
                    }

                    foreach (Ingredient ingredient in part.Ingredients)
                    {
                        foreach (KeyValuePair<int, string> allergen in ingredient.Allergies)
                        {
                            int ingredientAllergyId = allergen.Key;
                            string ingredientAllergyName = allergen.Value;

                            for (int i = 0; i < selectedAllergyIds.Count; i++)
                            {
                                int selectedAllergyId = selectedAllergyIds[i];

                                if (ingredientAllergyId == selectedAllergyId)
                                {
                                    Debug.WriteLine("Order triggers selected allergy.");

                                    ErrorMessage = ingredient.Name + " contains your allergen: " + ingredientAllergyName;
                                    return Page();
                                }
                            }
                        }
                    }
                }

                // Save order
                _orderService.AddOrder(NewOrder);

                // After success, go back to dashboard (more user-friendly than staying on NewOrder)
                return RedirectToPage("/Dashboard");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        // Adds a recipe part to the order if the ingredient is selected (ingredientId != 0)
        private void AddRecipePartIfSelected(
            int recipePartNumber,
            RecipePart recipePart,
            int ingredientId,
            int allergyId,
            List<int> selectedAllergyIds)
        {
            // If no ingredient chosen, skip this recipe part
            if (ingredientId == 0)
            {
                return;
            }

            // Ensure recipePart exists
            if (recipePart == null)
            {
                recipePart = new RecipePart();
            }

            // Ensure recipePart has its ingredient list
            if (recipePart.Ingredients == null)
            {
                recipePart.Ingredients = new List<Ingredient>();
            }

            // Add ingredient to recipe part
            if (Ingredients != null && Ingredients.ContainsKey(ingredientId))
            {
                recipePart.Ingredients.Add(Ingredients[ingredientId]);
            }

            // Add recipe part to order dictionary
            if (!NewOrder.Recipe.ContainsKey(recipePartNumber))
            {
                NewOrder.Recipe.Add(recipePartNumber, recipePart);
            }

            // Add selected allergy id (if chosen)
            if (allergyId != 0)
            {
                selectedAllergyIds.Add(allergyId);
            }
        }

        // Loads ingredient and allergy dropdown data from OrderService
        private void LoadDropdownData()
        {
            Ingredients = _orderService.GetAllIngredients();
            Dictionary<int, string> allergies = _orderService.GetAllAllergies();

            IngredientOptions = new List<SelectListItem>();
            foreach (KeyValuePair<int, Ingredient> ingredientEntry in Ingredients)
            {
                Ingredient ingredient = ingredientEntry.Value;
                IngredientOptions.Add(new SelectListItem(ingredient.Name, ingredient.ID.ToString()));
            }

            AllergyOptions = new List<SelectListItem>();
            foreach (KeyValuePair<int, string> allergyEntry in allergies)
            {
                AllergyOptions.Add(new SelectListItem(allergyEntry.Value, allergyEntry.Key.ToString()));
            }
        }
    }
}
