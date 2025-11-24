using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;

namespace gategourmetLibrary.Repo
{
    public interface IOrderRepo
    {
        //returns the list of all orders
        List<Order> GetAll();
        //add a new order to repo
        void Add(Order order);
        //delete an order by its ID
        void Delete(int orderID);
        //update an existing order by its ID
        void Update(int orderID, Order UpdatedOrder);
        //returns a specific order by its ID
        Order Get(int orderID);
        //returns a list of recipe parts for a specific order by orderID
        List<RecipePart> GetRecipeParts(int orderID);
        //filters orders made by a specific employee
        List<Order> FilterByEmployee(Employee employee);
        //filters orders placed today
        List<Order> FilterByToday(DateTime today);
        //filters orders for a specific customer/company
        List<Order> FilterByCompany(Customer customer);
        //filters orders by their status
        List<Order> FilterByStatus(OrderStatus status);
        //filters orders by a specific date
        List<Order> FilterByDate(DateTime date);
    }
}
