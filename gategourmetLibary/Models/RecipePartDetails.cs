using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gategourmetLibary.Models
{
    public class RecipePartDetails
    {
        public int R_ID { get; set; }
        public string Name { get; set; }
        public string HowToPrep { get; set; }
        public List<string> Ingredients { get; set; } = new List<string>(); //Initialize to avoid null reference and it means (= new List<string>(); )
    }
}
