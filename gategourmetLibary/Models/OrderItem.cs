using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gategourmetLibrary.Models
{
    
}
namespace gategourmetLibrary.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        // Link to the order
        public int OrderId { get; set; }

        // The ingredient that is needed
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        // How much of the ingredient is needed
        public int Quantity { get; set; }

        // Where the ingredient is stored
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
