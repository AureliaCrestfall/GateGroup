using System;
using System.Collections.Generic;
using gategourmetLibrary.Models;

namespace gategourmetLibrary.Repo
{
    // Interface for order data access and raw sql 
  
    public interface IOrderRepo
    {
        // Returns all orders in the system
        List<Order> GetAllOrders();

        // Returns all orders for a specific customer
        List<Order> GetOrdersByCustomerId(int customerId);

        // Adds a new order to the database
        void AddOrder(Order order);

        // Deletes an order by its ID
        void DeleteOrder(int orderId);

        // Updates an existing order
        void UpdateOrder(int orderId, Order updatedOrder);

        // Returns a single order by ID
        Order Get(int orderId);

        // Returns all recipe parts belonging to an order
        List<RecipePart> GetRecipeParts(int orderId);

        // Returns orders assigned to a specific employee
        List<Order> FilterByEmployee(Employee employee);

        // Returns orders created today
        List<Order> FilterByToday(DateTime today);

        // Returns orders for a specific company/customer
        List<Order> FilterByCompany(Customer customer);

        // Returns orders with a specific status
        List<Order> FilterByStatus(OrderStatus status);

        // Returns orders from a specific date
        List<Order> FilterByDate(DateTime date);

        // Returns all ingredients (ID -> Ingredient)
        Dictionary<int, Ingredient> GetAllIngredients();

        // Returns all allergies (ID -> allergy name)
        Dictionary<int, string> GetAllAllergies();

        // Returns all warehouse locations
        List<Warehouse> GetAllWarehouses();

        // Returns the current warehouse location of a recipe part
        Warehouse GetRecipePartLocation(int recipePartId);

        // Updates the warehouse location of a recipe part
        void UpdateRecipePartLocation(int recipePartId, int warehouseId);

        // Cancels an order (status update)
        void CancelOrder(int orderId);
    }
}
