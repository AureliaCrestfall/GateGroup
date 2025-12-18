using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// class for one part of a full recipe order fx. main dish 
namespace gategourmetLibrary.Models
{
    public class RecipePart
    {
        // where the recipe part was stored before assembly 
        public string StoredLocation {  get; set; }

        // ingredient used for this part of the dish
        public List<Ingredient> Ingredients { get; set; }

        // status of dish(prepared etc)
        public string status { get; set; }

        // assebling instruction 
        public string Assemble { get; set; }

        public string partName { get; set; }
        public int ID { get; set;}

        public RecipePart() 
        {
            Ingredients = new List<Ingredient>();
        }
    }
}
