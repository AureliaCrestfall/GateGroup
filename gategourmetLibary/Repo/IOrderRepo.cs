using gategourmetLibrary.Models;
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
        List<Order> GetAllOrders();
        //add a new order to repo
        void AddOrder(Order order);
        //delete an order by its ID
        void DeleteOrder(int orderID);
        Order Get(int orderID);
        //update an existing order by its ID
        void UpdateOrder(int orderID, Order UpdatedOrder);

        // cancels an order
        void CancelOrder(int orderId);

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
        Dictionary<int, Ingredient> GetAllIngredients();
        Dictionary<int, string> GetAllAllergies();


        //void Add(Order order);
        //Order Get(int orderID);
        //void Delete(int orderPrimaryKey);
        //Dictionary<int, Order> GetAll();
        //void Update(int orderID, Order updateOrder);

    }
}
