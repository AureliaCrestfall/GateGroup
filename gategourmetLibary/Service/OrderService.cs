using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibrary.Repo;
using gategourmetLibrary.Models;

namespace gategourmetLibrary.Service
{
    public class OrderService
    {
        private readonly IOrderRepo _orderRepo;

        // Constructor to inject the order repository
        public OrderService(IOrderRepo orderRepo)
        {
            _orderRepo = orderRepo;
        }
        // gets all orders
        public List<Order> GetAllOrders()
        {
            return _orderRepo.GetAllOrders();
        }
        // adds a new order
        public void AddOrder(Order order)
        {
            _orderRepo.AddOrder(order);
        }
        // deletes an order by ID
        public void DeleteOrder(int orderID)
        {
            _orderRepo.DeleteOrder(orderID);
        }
        // updates an existing order
        public void UpdateOrder(int orderID, Order updatedOrder)
        {
            _orderRepo.UpdateOrder(orderID, updatedOrder);
        }
        // gets a specific order by ID
        public Order GetOrder(int orderID)
        {
            return _orderRepo.Get(orderID);
        }
        // gets recipe parts for a specific order
        public List<RecipePart> GetOrderRecipeParts(int orderID)
        {
            return _orderRepo.GetRecipeParts(orderID);
        }
        // to filter orders by employee
        public List<Order> FilterOrdersByEmployee(Employee employee)
        {
            return _orderRepo.FilterByEmployee(employee);
        }
        // to filter orders placed today
        public List<Order> FilterOrdersByToday(DateTime today)
        {
            return _orderRepo.FilterByToday(today);
        }
        // to filter orders by company
        public List<Order> FilterOrdersByCompany(Customer customer)
        {
            return _orderRepo.FilterByCompany(customer);
        }
        // to filter orders by status
        public List<Order> FilterOrdersByStatus(OrderStatus status)
        {
            return _orderRepo.FilterByStatus(status);
        }
        // to filter orders by date
        public List<Order> FilterOrdersByDate(DateTime date)
        {
            return _orderRepo.FilterByDate(date);
        }

        public List<Ingredient> GetAllIngredients()
        {
            return _orderRepo.GetAllIngredients();
        }

       

    }
}
