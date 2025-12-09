using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

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


        public List<Ingredient> Ingredients { get; set; }
       



       
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
            newOrder.OrderMade = DateTime.Now;
            newOrder.OrderDoneBy = DateTime.Now.AddDays(7).Date;
            Ingredients = _os.GetAllIngredients();
            
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
            
            recipePart1.Ingredients.Add(new Ingredient(Ingredient1));
            recipePart2.Ingredients.Add(new Ingredient(Ingredient2));
            recipePart3.Ingredients.Add(new Ingredient(Ingredient3));
            recipePart4.Ingredients.Add(new Ingredient(Ingredient4));
            recipePart5.Ingredients.Add(new Ingredient(Ingredient5));
           
            newOrder.Recipe.Add(1,recipePart1);
            newOrder.Recipe.Add(2, recipePart2);
            newOrder.Recipe.Add(3, recipePart3);
            newOrder.Recipe.Add(4, recipePart4);
            newOrder.Recipe.Add(5, recipePart5);
            _os.AddOrder(newOrder);
            return Page();
        }

    }
}
