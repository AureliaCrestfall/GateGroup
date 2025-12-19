using System;
using System.Collections;
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

        public Dictionary<int, Order> GetAll()
        {
            return _orderRepo.GetAll();
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
            if (order != null)
            {
                if ((order.OrderDoneBy - order.OrderMade).TotalDays >= 6)
                {
                    List<int> invalidRecipeParts = new List<int>();
                    foreach (KeyValuePair<int, RecipePart> rp in order.Recipe)
                    {
                        if (rp.Value.partName == string.Empty || rp.Value.partName == null)
                        {
                            invalidRecipeParts.Add(rp.Key);
                        }
                    }

                    foreach (int i in invalidRecipeParts)
                    {
                        order.Recipe.Remove(i);
                    }

                    if (order.Recipe.Count > 0)
                    {
                        _orderRepo.AddOrder(order);
                    }
                    else
                    {
                        throw new Exception("order dosn't conatin any vailed recipeparts");
                    }
                }
                else
                {
                    throw new Exception("Order Ready by is the close to the time of when the order is made is need to be at least 7 days after");
                }
            }
        }

        // deletes an order by ID
        public void DeleteOrder(int ID)
        {
            _orderRepo.DeleteOrder(ID);
        }

        // updates an existing order
        public void UpdateOrder(int orderID, Order updatedOrder)
        {
            _orderRepo.UpdateOrder(orderID, updatedOrder);
        }

        // gets a specific order by ID
        public Order GetOrder(int orderID)
        {
            Order order = _orderRepo.Get(orderID);
            Dictionary<int, Ingredient> allIngredients = _orderRepo.GetAllIngredients();

            foreach (RecipePart recipePart in order.Recipe.Values)
            {
                foreach (Ingredient ing in recipePart.Ingredients)
                {
                    if (allIngredients.ContainsKey(ing.ID))
                    {
                        ing.Allergies = allIngredients[ing.ID].Allergies;
                    }
                }
            }

            return order;
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

        // to filter orders placed today with a specific status 
        public List<Order> FilterOrdersByTodayAndStatus(DateTime today, OrderStatus status)
        {
            return _orderRepo.FilterByTodayAndStatus(today, status);
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

        // to filter orders by department
        public List<Order> FilterOrdersByDepartment(int departmentId)
        {
            return _orderRepo.FilterByDepartment(departmentId);
        }

        // Filters the provided orders list to only include orders whose IDs are in orderIds.
        // This is used by UI flows that already loaded Orders and then want to apply an "allowed IDs" filter.
        public List<Order> FilterOrdersByOrderIds(List<Order> orders, ICollection<int> orderIds)
        {
            if (orders == null || orders.Count == 0)
            {
                return new List<Order>();
            }

            if (orderIds == null || orderIds.Count == 0)
            {
                return new List<Order>();
            }

            HashSet<int> allowed = orderIds as HashSet<int> ?? new HashSet<int>(orderIds);

            List<Order> filtered = new List<Order>();
            foreach (Order order in orders)
            {
                if (order != null && allowed.Contains(order.ID))
                {
                    filtered.Add(order);
                }
            }

            return filtered;
        }

        // Filters orders by created date range (OrderMade) if from/to values are provided.
        public List<Order> FilterOrdersByCreatedDateRange(List<Order> orders, DateTime? fromDate, DateTime? toDate)
        {
            if (orders == null || orders.Count == 0)
            {
                return new List<Order>();
            }

            DateTime? from = fromDate?.Date;
            DateTime? to = toDate?.Date;

            List<Order> filtered = new List<Order>();
            foreach (Order order in orders)
            {
                if (order == null)
                {
                    continue;
                }

                DateTime made = order.OrderMade.Date;

                if (from.HasValue && made < from.Value)
                {
                    continue;
                }

                if (to.HasValue && made > to.Value)
                {
                    continue;
                }

                filtered.Add(order);
            }

            return filtered;
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

        public List<Order> GetAllOrdersFromid(int id)
        {
            return _orderRepo.GetAllOrdersFromid(id);
        }

        public void MarkorderDone(int orderId)
        {
            _orderRepo.MarkorderDone(orderId);
        }
    }
}
