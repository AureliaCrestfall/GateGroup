using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gategourmetLibrary.Repo
{
    public class OrderRepo : IOrderRepo
    {
        //returns the list of all orders
        public List<Order> GetAll()
        {
            return null;
        }
        //add a new order to repo
        public void Add(Order order)
        {

        }
        //delete an order by its ID
        public void Delete(int orderID)
        {

        }
        //update an existing order by its ID
        public void Update(int orderID, Order UpdatedOrder)
        {

        }
        //returns a specific order by its ID
        public Order Get(int orderID)
        {
            return null;
        }
        //returns a list of recipe parts for a specific order by orderID
        public List<RecipePart> GetRecipeParts(int orderID)
        {
            return null;
        }
        //filters orders made by a specific employee
        public List<Order> FilterByEmployee(Employee employee)
        {
             return null;
        }
        //filters orders placed today
        public List<Order> FilterByToday(DateTime today)
        { return null;
        }
        //filters orders for a specific customer/company
        public List<Order> FilterByCompany(Customer customer)
        {  
          return null;
        }
        //filters orders by their status
        public List<Order> FilterByStatus(OrderStatus status)
        {
          return null;
        }
        //filters orders by a specific date
        public List<Order> FilterByDate(DateTime date)
        {
            return null;
        }

        //public void GetAll()
        //{
        //    List<Order>
        //  }

        //public void Add(Order newOrder)
        //{

        //}
        //public void Delete(int orderID)
        //{

        //}
        //public void Update(int orderID, Order updateOrder)
        //{

        //}

        //public void Get(int orderID)
        //{

        //}

        //public void GetRecipeParts(int orderID)
        //{
        //    List<RecipePart>
        //}

        //public void Filter(employee filterAfterWhoMade)
        //{
        //    List<order>
        //}

        //public void Filter(datetime filterAfterOrderToday)
        //{
        //    List<order>
        //}

        //public void Filter(customer filterAfterCompany)
        //{
        //    List<order>
        //}

        //public void Filter (enum filterAfterStatus)
        //{
        //    List<order> 
        //}

        //public void Filter(datetime filterAfterDate)
        //{ 

        //}
    }
}
