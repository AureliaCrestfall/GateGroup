using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// class representing a customer 
namespace gategourmetLibrary.Models
{
    public class Customer : User
    {
        // list for orders for this customer 
        public List<Order> MyOrders { get; set; }

        //Company cvr
        public string CVR {  get; set; }

        // id for customer 
        public int ID { get; set; }

        // Name of customer
        public string Name { get; set; }

        // name of company 
        public string CompanyName { get; set; }

        // contact mail
        public string Email { get; set; }

        public Customer()
        {
            MyOrders = new List<Order>();
           
        }
    }
}
