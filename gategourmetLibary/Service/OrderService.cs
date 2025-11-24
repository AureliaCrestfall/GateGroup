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
        // Method to get all orders
        public List<Order> GetAllOrders()
        {
            return _orderRepo.GetAll();
        }
        // Method to add a new order
        public void AddOrder(Order order)
        {
            _orderRepo.Add(order);
        }
        // Method to delete an order by ID
        public void DeleteOrder(int orderID)
        {
            _orderRepo.Delete(orderID);
        }
        // Method to update an existing order
        public void UpdateOrder(int orderID, Order updatedOrder)
        {
            _orderRepo.Update(orderID, updatedOrder);
        }
        // Method to get a specific order by ID
        public Order GetOrder(int orderID)
        {
            return _orderRepo.Get(orderID);
        }
        //// Method to get recipe parts for a specific order
        public List<RecipePart> GetOrderRecipeParts(int orderID)
        {
            return _orderRepo.GetRecipeParts(orderID);
        }
        // Method to filter orders by employee
        public List<Order> FilterOrdersByEmployee(Employee employee)
        {
            return _orderRepo.FilterByEmployee(employee);
        }
        // Method to filter orders placed today
        public List<Order> FilterOrdersByToday(DateTime today)
        {
            return _orderRepo.FilterByToday(today);
        }
        // Method to filter orders by company
        public List<Order> FilterOrdersByCompany(Customer customer)
        {
            return _orderRepo.FilterByCompany(customer);
        }
        // Method to filter orders by status
        public List<Order> FilterOrdersByStatus(OrderStatus status)
        {
            return _orderRepo.FilterByStatus(status);
        }
        // Method to filter orders by date
        public List<Order> FilterOrdersByDate(DateTime date)
        {
            return _orderRepo.FilterByDate(date);
        }
    }
}
