using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// class for order
namespace gategourmetLibrary.Models
{
    public class Order
    {
        // parts from recipe that forms a complete dish 
        public List<RecipePart> Recipe {  get; set; }

        // customer that has ordered this dish/ order
        public Customer CustomerOrder { get; set; }

        // current status of this order by using OrderStatus Enum
        public OrderStatus Status { get; set; }

        // when order was created 
        public DateTime OrderMade { get; set; }

        // when order must be done/finished 
        public DateTime OrderDoneBy { get; set; }

        public Order()
        {
            Recipe = new List<RecipePart>();
        }
    }
}
