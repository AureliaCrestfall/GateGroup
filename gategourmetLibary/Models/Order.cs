using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
// class for order
namespace gategourmetLibrary.Models
{
    public class Order
    {
        public Dictionary<int, RecipePart> Recipe { get; set; }
        public Customer CustomerOrder { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderMade { get; set; }
        public DateTime OrderDoneBy { get; set; }
        public int ID { get; set; }
        public bool paystatus { get; set; }

        // NEW: Track individual items
        public List<OrderItem> Items { get; set; }

        public Order(DateTime made, DateTime ready, int id, bool pstatus):this()
        {
            OrderMade = made;
            OrderDoneBy = ready;
            ID = id;
            paystatus = pstatus;
        }

        public Order(DateTime made, DateTime ready, Customer customer, int id, bool pstatus, Dictionary<int, RecipePart> recipe): this(made, ready, id, pstatus)
        {
            CustomerOrder = customer;
            Recipe = recipe;
        }
        public Order(DateTime made, DateTime ready, int id, bool pstatus,OrderStatus status):this( made,  ready,  id,  pstatus)
        {
            Status = status;
        }

        public Order()
        {
            Recipe = new Dictionary<int, RecipePart>();
            Items = new List<OrderItem>();
            Status = OrderStatus.Created;
        }

        
    }



}

