using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Class represents a single ingredient that can be used in recipes
namespace gategourmetLibrary.Models
{
    public class Ingredient
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public DateTime ExpireDate { get; set; }

        public int Quantity { get; set; }

        public int WarehouseId { get; set; }

        //List of allergy tags connected to the ingredients (f.ks.lactose )
        public List<string> Allergies { get; set; }


        public Ingredient() 
        {
            Allergies = new List<string>();
        }

    }
}
