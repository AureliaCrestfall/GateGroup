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

        public void CancelOrder(int orderId)
        {
            _orderRepo.CancelOrder(orderId);
        }

        // adds a new order
        public void AddOrder(Order order)
        {
            if(order != null)
            {
                if ((order.OrderDoneBy - order.OrderMade).TotalDays >= 6)
                {
                    List<int> invalidRecipeParts = new List<int>();
                    foreach(KeyValuePair<int,RecipePart> rp in order.Recipe)
                    {
                        if (rp.Value.partName == string.Empty || rp.Value.partName == null )
                        {
                            invalidRecipeParts.Add(rp.Key);
                        }
                        
                    }
                    foreach(int i in invalidRecipeParts)
                    {
                        order.Recipe.Remove(i);
                    }
                    if (order.Recipe.Count >0)
                    {
                        _orderRepo.AddOrder(order);

                    }
                    else
                    {
                        throw new Exception("order dosn't conatin any vailed recipeParts");
                    }

                }
                else
                {
                    throw new Exception("Order Ready by is the close to the time of when the order is made is need to be at least 7 days after");
                }
            }
            


            
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

        public Dictionary<int, Ingredient> GetAllIngredients()
        {
            return _orderRepo.GetAllIngredients();
        }
        public Dictionary<int, string> GetAllAllergies()
        {
            return _orderRepo.GetAllAllergies();
        }

        // returns all warehouses fks example freezer, fridge, dry storage + used to show choices in dropdown for employees
        public List<Warehouse> GetAllWarehouses()
        {
            return _orderRepo.GetAllWarehouses();
        }

        // returns the current warehouse location for a specific recipe part
        public Warehouse GetRecipePartLocation(int recipePartId)
        {
            return _orderRepo.GetRecipePartLocation(recipePartId);
        }

        // updates the warehouse location for a specific recipe part
        
        public void UpdateRecipePartLocation(int recipePartId, int warehouseId)
        {
            _orderRepo.UpdateRecipePartLocation(recipePartId, warehouseId);
        }
        

    }
}
