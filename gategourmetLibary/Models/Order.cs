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
        public Dictionary<int,RecipePart> Recipe {  get; set; }

        // customer that has ordered this dish/ order
        public Customer CustomerOrder { get; set; }

        // current status of this order by using OrderStatus Enum
        public OrderStatus Status { get; set; }

        // when order was created 
        public DateTime OrderMade { get; set; }

        // when order must be done/finished 
        public DateTime OrderDoneBy { get; set; }

        public int ID { get; set; }
        public bool paystatus { get; set; }

        public Order( DateTime made,DateTime ready,int id,bool pstatus)
        {
            OrderMade = made;
            OrderDoneBy = ready;
            ID = id;
            paystatus = pstatus;
        }
        public Order(DateTime made, DateTime ready, Customer customer, int id, bool pstatus, Dictionary<int, RecipePart> recipe):this(made, ready, id, pstatus)
        {
            CustomerOrder = customer;
            Recipe = recipe;
        }
        public Order()
        {
            Recipe = new Dictionary<int, RecipePart>();
            Status = OrderStatus.Created;
        }
    }
}
