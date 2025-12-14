using System;
using System.Collections.Generic;
using gategourmetLibrary.Models;
using gategourmetLibrary.Repo;

namespace gategourmetLibrary.Service
{
    // Service responsible for order business rules and validation before repository calls.
    public class OrderService
    {
        // Repository used for data access (raw SQL lives in repo).
        private readonly IOrderRepo _orderRepo;

        // Constructor - injects the order repository.
        public OrderService(IOrderRepo orderRepo)
        {
            _orderRepo = orderRepo;
        }

        // Returns all orders.
        public List<Order> GetAllOrders()
        {
            return _orderRepo.GetAllOrders();
        }

        // Returns all orders for a specific customer.
        public List<Order> GetOrdersByCustomerId(int customerId)
        {
            return _orderRepo.GetOrdersByCustomerId(customerId);
        }

        // Cancels an order by id.
        public void CancelOrder(int orderId)
        {
            _orderRepo.CancelOrder(orderId);
        }

        // Adds a new order (validates dates + recipe parts before saving).
        public void AddOrder(Order order)
        {
            // 1) Null check (fail fast).
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order cannot be null.");
            }

            // 2) Ensure recipe dictionary exists.
            if (order.Recipe == null)
            {
                throw new Exception("Order recipe is missing (Recipe dictionary is null).");
            }

            // 3) Validate ready-by date (date only to avoid time edge cases).
            DateTime madeDate = order.OrderMade.Date;
            DateTime readyDate = order.OrderDoneBy.Date;

            double daysBetween = (readyDate - madeDate).TotalDays;

            // Requirement: ready-by must be at least 7 days after made date.
            if (daysBetween < 7)
            {
                throw new Exception("Ready by date must be at least 7 days after the order is made.");
            }

            // 4) Validate recipe parts.
            // We remove parts that are unusable. If part name is missing but ingredients exist, we set a default name.
            List<int> recipePartKeysToRemove = new List<int>();

            foreach (KeyValuePair<int, RecipePart> recipeEntry in order.Recipe)
            {
                int recipePartNumber = recipeEntry.Key;
                RecipePart recipePart = recipeEntry.Value;

                // If the recipe part object is null, it cannot be used.
                if (recipePart == null)
                {
                    recipePartKeysToRemove.Add(recipePartNumber);
                }
                else
                {
                    // Check if part has ingredients.
                    bool hasIngredients = recipePart.Ingredients != null && recipePart.Ingredients.Count > 0;

                    // If name is missing but ingredients exist, create a default name.
                    if (string.IsNullOrWhiteSpace(recipePart.partName))
                    {
                        if (hasIngredients)
                        {
                            recipePart.partName = "Recipe part " + recipePartNumber;
                        }
                        else
                        {
                            // No name and no ingredients -> not valid.
                            recipePartKeysToRemove.Add(recipePartNumber);
                        }
                    }

                    // If it has no ingredients, it is not useful to store.
                    if (!hasIngredients)
                    {
                        recipePartKeysToRemove.Add(recipePartNumber);
                    }
                }
            }

            // Remove invalid parts.
            foreach (int key in recipePartKeysToRemove)
            {
                if (order.Recipe.ContainsKey(key))
                {
                    order.Recipe.Remove(key);
                }
            }

            // 5) Ensure we still have at least one valid recipe part.
            if (order.Recipe.Count == 0)
            {
                throw new Exception("Order does not contain any valid recipe parts. Please select ingredients (and optionally names).");
            }

            // 6) Save order.
            _orderRepo.AddOrder(order);
        }

        // Deletes an order by id.
        public void DeleteOrder(int orderId)
        {
            _orderRepo.DeleteOrder(orderId);
        }

        // Updates an existing order by id.
        public void UpdateOrder(int orderId, Order updatedOrder)
        {
            _orderRepo.UpdateOrder(orderId, updatedOrder);
        }

        // Returns a specific order by id.
        public Order GetOrder(int orderId)
        {
            return _orderRepo.Get(orderId);
        }

        // Returns recipe parts for a specific order.
        public List<RecipePart> GetOrderRecipeParts(int orderId)
        {
            return _orderRepo.GetRecipeParts(orderId);
        }

        // Filters orders by employee.
        public List<Order> FilterOrdersByEmployee(Employee employee)
        {
            return _orderRepo.FilterByEmployee(employee);
        }

        // Filters orders placed today.
        public List<Order> FilterOrdersByToday(DateTime today)
        {
            return _orderRepo.FilterByToday(today);
        }

        // Filters orders by customer/company.
        public List<Order> FilterOrdersByCompany(Customer customer)
        {
            return _orderRepo.FilterByCompany(customer);
        }

        // Filters orders by status.
        public List<Order> FilterOrdersByStatus(OrderStatus status)
        {
            return _orderRepo.FilterByStatus(status);
        }

        // Filters orders by date.
        public List<Order> FilterOrdersByDate(DateTime date)
        {
            return _orderRepo.FilterByDate(date);
        }

        // Returns all ingredients (id -> ingredient).
        public Dictionary<int, Ingredient> GetAllIngredients()
        {
            return _orderRepo.GetAllIngredients();
        }

        // Returns all allergies (id -> allergy name).
        public Dictionary<int, string> GetAllAllergies()
        {
            return _orderRepo.GetAllAllergies();
        }

        // Returns all warehouses (freezer, fridge, dry storage etc).
        public List<Warehouse> GetAllWarehouses()
        {
            return _orderRepo.GetAllWarehouses();
        }

        // Returns the current warehouse location for a recipe part.
        public Warehouse GetRecipePartLocation(int recipePartId)
        {
            return _orderRepo.GetRecipePartLocation(recipePartId);
        }

        // Updates the warehouse location for a recipe part.
        public void UpdateRecipePartLocation(int recipePartId, int warehouseId)
        {
            _orderRepo.UpdateRecipePartLocation(recipePartId, warehouseId);
        }
    }
}
