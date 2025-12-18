using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Linq.Expressions;

namespace GateGroupWebpages.Pages
{
    public class NewOrderModel : PageModel
    {
        readonly CustomerService _cs;
        readonly OrderService _os;
        [BindProperty]
        public Order newOrder { get; set; }
        [BindProperty]

     

        public RecipePart recipePart1 { get; set; }
        [BindProperty]
        public RecipePart recipePart2 { get; set; }
        [BindProperty]

        public RecipePart recipePart3 { get; set; }
        [BindProperty]

        public RecipePart recipePart4 { get; set; }
        [BindProperty]

        public RecipePart recipePart5 { get; set; }
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
        [BindProperty]
        public int allerie1 { get; set; }
        [BindProperty]
        public int allerie2 { get; set; }
        [BindProperty]

        public int allerie3 { get; set; }
        [BindProperty]

        public int allerie4 { get; set; }
        [BindProperty]

        public int allerie5 { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string SuccessMessage { get; set; }
     


        [BindProperty]
        public List<SelectListItem> IngredientOptions { get; set; }

        [BindProperty]
        public List<SelectListItem> AllerieOptions { get; set; }


        [BindProperty]
        Dictionary<int, Ingredient> Ingredients { get; set; }


        public NewOrderModel(OrderService os,CustomerService cs)
        {
            _os = os;
            _cs = cs;
            newOrder = new Order();
            recipePart1 = new RecipePart();
            recipePart2 = new RecipePart();
            recipePart3 = new RecipePart();
            recipePart4 = new RecipePart();
            recipePart5 = new RecipePart();
            Load();





        }


        public IActionResult OnGet()
        {
            // Tjek om brugeren er logget ind før den giver adgang til siden 
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                // Hvis IKKE logget ind - send til login siden
                return RedirectToPage("/Login");
            }

            // Hvis logget ind - vis siden som normalt
            return Page();

        }


        public IActionResult OnPost()
        {
            List<int> slectedAllerieId = new List<int>();
            if(Ingredient1 != 0)
            {
                recipePart1.Ingredients.Add(Ingredients[Ingredient1]);
                newOrder.Recipe.Add(1, recipePart1);
                slectedAllerieId.Add(allerie1);
            }
            if (Ingredient2 != 0)
            {
                recipePart2.Ingredients.Add(Ingredients[Ingredient2]);
                newOrder.Recipe.Add(2, recipePart2);
                slectedAllerieId.Add(allerie2);

            }
            if (Ingredient3 != 0)
            {
                recipePart3.Ingredients.Add(Ingredients[Ingredient3]);
                newOrder.Recipe.Add(3, recipePart3);
                slectedAllerieId.Add(allerie3);

            }
            if (Ingredient4 != 0)
            {
                recipePart4.Ingredients.Add(Ingredients[Ingredient4]);
                newOrder.Recipe.Add(4, recipePart4);
                slectedAllerieId.Add(allerie4);
            }
            if (Ingredient5 != 0)
            {
                recipePart5.Ingredients.Add(Ingredients[Ingredient5]);
                newOrder.Recipe.Add(5, recipePart5);
                slectedAllerieId.Add(allerie5);

            }


            try
            {
                foreach(KeyValuePair<int,RecipePart> kv in newOrder.Recipe)
                {
                    foreach(Ingredient i in kv.Value.Ingredients)
                    {
                        foreach(KeyValuePair<int,string> a in i.Allergies)
                        {
                            foreach (int allerieID in slectedAllerieId)
                            {
                                if (a.Key == allerieID)
                                {
                                    ErrorMessage = $"{i.Name} has one of your {a.Value} allerie";
                                    Load();
                                    return Page();
                                }
                            }
                        }
                        
                       
                    }

                }

                _os.AddOrder(newOrder);
                SuccessMessage = "order has been added";
                Load();
                return Page();


            }
            catch (Exception ex)
            {
                newOrder.OrderMade = DateTime.Now;
                                    newOrder.OrderDoneBy = DateTime.Now.AddDays(7).Date;
                                    Ingredients = _os.GetAllIngredients();
                                    Dictionary<int, string> Alleries = _os.GetAllAllergies();

                                    IngredientOptions = new List<SelectListItem>();
                                    foreach (KeyValuePair<int, Ingredient> ikv in Ingredients)
                                    {
                                        IngredientOptions.Add(new SelectListItem(ikv.Value.Name, ikv.Value.ID.ToString()));
                                    }
                                    AllerieOptions = new List<SelectListItem>();
                                    foreach (KeyValuePair<int, string> akv in Alleries)
                                    {
                                        AllerieOptions.Add(new SelectListItem(akv.Value, akv.Key.ToString()));
                                    }
                ErrorMessage = $"{ex.Message}";
                return Page();

            }


            return RedirectToPage("/NewOrder");
        }
        public void Load()
        {
            newOrder.OrderMade = DateTime.Now;
            newOrder.OrderDoneBy = DateTime.Now.AddDays(7).Date;
            Ingredients = _os.GetAllIngredients();
            Dictionary<int, string> Alleries = _os.GetAllAllergies();

            IngredientOptions = new List<SelectListItem>();
            foreach (KeyValuePair<int, Ingredient> ikv in Ingredients)
            {
                IngredientOptions.Add(new SelectListItem(ikv.Value.Name, ikv.Value.ID.ToString()));
            }
            AllerieOptions = new List<SelectListItem>();
            foreach (KeyValuePair<int, string> akv in Alleries)
            {
                AllerieOptions.Add(new SelectListItem(akv.Value, akv.Key.ToString()));
            }
        }

    }
}
