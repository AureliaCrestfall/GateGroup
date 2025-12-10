using gategourmetLibrary.Models;
using System;
using System.Collections.Generic;

namespace gategourmetLibrary.Repo
{
    // Interface for order repository.
    // This defines all methods that the OrderRepo class must implement.
    public interface IOrderRepo
    {
        // returns the list of all orders
        List<Order> GetAllOrders();

        // add a new order to repo
        void AddOrder(Order order);

        // delete an order by its ID
        void DeleteOrder(int orderID);

        // returns a specific order by its ID
        Order Get(int orderID);

        // update an existing order by its ID
        void UpdateOrder(int orderID, Order UpdatedOrder);

        // cancels an order
        void CancelOrder(int orderId);

        // returns a list of recipe parts for a specific order by orderID
        List<RecipePart> GetRecipeParts(int orderID);

        // filters orders made by a specific employee
        List<Order> FilterByEmployee(Employee employee);

        // filters orders placed today
        List<Order> FilterByToday(DateTime today);

        // filters orders for a specific customer/company
        List<Order> FilterByCompany(Customer customer);

        // filters orders by their status
        List<Order> FilterByStatus(OrderStatus status);

        // filters orders by a specific date
        List<Order> FilterByDate(DateTime date);

        // returns all ingredients
        Dictionary<int, Ingredient> GetAllIngredients();

        // returns all allergies
        Dictionary<int, string> GetAllAllergies();

        // returns the warehouse where a given recipe part is stored
        Warehouse GetRecipePartLocation(int recipePartId);

        // updates the warehouse location for a given recipe part
        void UpdateRecipePartLocation(int recipePartId, int warehouseId);

        // returns all warehouses (freezer, fridge, dry storage)
        List<Warehouse> GetAllWarehouses();
    }
}
