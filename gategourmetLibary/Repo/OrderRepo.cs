using System;
using System.Collections.Generic;
using System.Diagnostics;
using gategourmetLibrary.Models;
using Microsoft.Data.SqlClient;

namespace gategourmetLibrary.Repo
{
    // Repository for all Order SQL operations.
    // Raw SQL lives here (not in Service / Razor Pages).
    public class OrderRepo : IOrderRepo
    {
        // Connection string for the database
        private readonly string _connectionString;

        // Constructor gets connection string from outside
        public OrderRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Gets all orders (including customer if linked)
        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "SELECT o.O_ID, o.O_Made, o.O_Ready, o.O_PaySatus, o.O_Status, " +
                    "c.C_ID, c.C_Name " +
                    "FROM OrderTable o " +
                    "LEFT JOIN OrderTableCustomer oc ON o.O_ID = oc.O_ID " +
                    "LEFT JOIN Customer c ON oc.C_ID = c.C_ID",
                    connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["O_ID"]);
                    DateTime made = Convert.ToDateTime(reader["O_Made"]);
                    DateTime ready = Convert.ToDateTime(reader["O_Ready"]);
                    bool paystatus = Convert.ToBoolean(reader["O_PaySatus"]);

                    string statusText = reader["O_Status"].ToString();
                    OrderStatus status = ParseOrderStatus(statusText);

                    Order order = new Order(made, ready, id, paystatus);
                    order.Status = status;

                    // Customer is optional (LEFT JOIN)
                    if (reader["C_ID"] != DBNull.Value)
                    {
                        order.CustomerOrder = new Customer();
                        order.CustomerOrder.ID = Convert.ToInt32(reader["C_ID"]);
                        order.CustomerOrder.Name = reader["C_Name"].ToString();
                    }

                    orders.Add(order);
                }

                reader.Close();
            }

            return orders;
        }

        // Gets orders for one customer (used by customer dashboard)
        public List<Order> GetOrdersByCustomerId(int customerId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "SELECT o.O_ID, o.O_Made, o.O_Ready, o.O_PaySatus, o.O_Status " +
                    "FROM dbo.OrderTable o " +
                    "INNER JOIN dbo.OrderTableCustomer oc ON o.O_ID = oc.O_ID " +
                    "WHERE oc.C_ID = @CustomerId",
                    connection);

                command.Parameters.AddWithValue("@CustomerId", customerId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["O_ID"]);
                    DateTime made = Convert.ToDateTime(reader["O_Made"]);
                    DateTime ready = Convert.ToDateTime(reader["O_Ready"]);
                    bool paystatus = Convert.ToBoolean(reader["O_PaySatus"]);

                    string statusText = reader["O_Status"].ToString();
                    OrderStatus status = ParseOrderStatus(statusText);

                    Order order = new Order(made, ready, id, paystatus);
                    order.Status = status;

                    orders.Add(order);
                }

                reader.Close();
            }

            return orders;
        }

        // Adds a new order + links customer + creates recipe parts + ingredient links
        public void AddOrder(Order newOrder)
        {
            if (newOrder == null)
            {
                throw new ArgumentNullException(nameof(newOrder));
            }

            int newOrderId;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "INSERT INTO OrderTable (O_Made, O_Ready, O_PaySatus, O_Status) " +
                    "VALUES (@O_Made, @O_Ready, @O_PaySatus, @O_Status); " +
                    "SELECT SCOPE_IDENTITY();",
                    connection);

                command.Parameters.AddWithValue("@O_Made", newOrder.OrderMade);
                command.Parameters.AddWithValue("@O_Ready", newOrder.OrderDoneBy);
                command.Parameters.AddWithValue("@O_PaySatus", newOrder.paystatus);
                command.Parameters.AddWithValue("@O_Status", newOrder.Status.ToString());

                connection.Open();
                newOrderId = Convert.ToInt32(command.ExecuteScalar());
            }

            // Link order to customer (if given)
            if (newOrder.CustomerOrder != null && newOrder.CustomerOrder.ID != 0)
            {
                AddOrderTableCustomer(newOrderId, newOrder.CustomerOrder.ID);
            }

            // Add recipe parts + ingredient relations
            if (newOrder.Recipe != null)
            {
                foreach (KeyValuePair<int, RecipePart> partEntry in newOrder.Recipe)
                {
                    RecipePart recipePart = partEntry.Value;
                    if (recipePart != null)
                    {
                        if (recipePart.Ingredients == null)
                        {
                            recipePart.Ingredients = new List<Ingredient>();
                        }

                        AddRecipePart(recipePart, newOrderId, recipePart.Ingredients);
                    }
                }
            }
        }

        // Deletes an order and all dependent relations (junction tables first)
        public void DeleteOrder(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Delete junction table rows first (avoid FK conflicts)
                SqlCommand cmd1 = new SqlCommand("DELETE FROM orderTableRecipePart WHERE O_ID = @id", connection);
                cmd1.Parameters.AddWithValue("@id", orderId);
                cmd1.ExecuteNonQuery();

                SqlCommand cmd2 = new SqlCommand("DELETE FROM OrderTableCustomer WHERE O_ID = @id", connection);
                cmd2.Parameters.AddWithValue("@id", orderId);
                cmd2.ExecuteNonQuery();

                SqlCommand cmd3 = new SqlCommand("DELETE FROM EmployeeRecipePartOrderTable WHERE O_ID = @id", connection);
                cmd3.Parameters.AddWithValue("@id", orderId);
                cmd3.ExecuteNonQuery();

                // Finally delete the order
                SqlCommand cmd4 = new SqlCommand("DELETE FROM OrderTable WHERE O_ID = @id", connection);
                cmd4.Parameters.AddWithValue("@id", orderId);
                cmd4.ExecuteNonQuery();
            }
        }

        // Updates an order (not used yet, but must exist for interface)
        public void UpdateOrder(int orderId, Order updatedOrder)
        {
            // Not used right now
            throw new NotImplementedException();
        }

        // Gets one order (and recipe parts) by id
        public Order Get(int orderId)
        {
            Order order = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "SELECT o.O_ID, o.O_Made, o.O_Ready, o.O_PaySatus, o.O_Status " +
                    "FROM OrderTable o " +
                    "WHERE o.O_ID = @id",
                    connection);

                command.Parameters.AddWithValue("@id", orderId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int id = Convert.ToInt32(reader["O_ID"]);
                    DateTime made = Convert.ToDateTime(reader["O_Made"]);
                    DateTime ready = Convert.ToDateTime(reader["O_Ready"]);
                    bool paystatus = Convert.ToBoolean(reader["O_PaySatus"]);

                    string statusText = reader["O_Status"].ToString();
                    OrderStatus status = ParseOrderStatus(statusText);

                    order = new Order(made, ready, id, paystatus);
                    order.Status = status;

                    // Prepare recipe dictionary (so it never becomes null)
                    order.Recipe = new Dictionary<int, RecipePart>();
                }

                reader.Close();
            }

            // Load recipe parts separately (keep Get() simple and safe)
            if (order != null)
            {
                List<RecipePart> parts = GetRecipeParts(orderId);

                for (int i = 0; i < parts.Count; i++)
                {
                    RecipePart part = parts[i];
                    if (part != null)
                    {
                        // Use recipe part id as key
                        order.Recipe.Add(part.ID, part);
                    }
                }
            }

            return order;
        }

        // Gets recipe parts for an order (basic version)
        public List<RecipePart> GetRecipeParts(int orderId)
        {
            List<RecipePart> parts = new List<RecipePart>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "SELECT rp.R_ID, rp.R_Name, rp.R_HowToPrep, rp.R_Status " +
                    "FROM OrderTableRecipePart otp " +
                    "INNER JOIN RecipePart rp ON otp.R_ID = rp.R_ID " +
                    "WHERE otp.O_ID = @id",
                    connection);

                command.Parameters.AddWithValue("@id", orderId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RecipePart part = new RecipePart();
                    part.ID = Convert.ToInt32(reader["R_ID"]);
                    part.partName = reader["R_Name"].ToString();
                    part.Assemble = reader["R_HowToPrep"].ToString();
                    part.status = reader["R_Status"].ToString();

                    // Ingredients can be loaded later if needed
                    part.Ingredients = new List<Ingredient>();

                    parts.Add(part);
                }

                reader.Close();
            }

            return parts;
        }

        // Filters by employee (not used yet)
        public List<Order> FilterByEmployee(Employee employee)
        {
            // Not used right now
            return new List<Order>();
        }

        // Filters by today (not used yet)
        public List<Order> FilterByToday(DateTime today)
        {
            // Not used right now
            return new List<Order>();
        }

        // Filters by customer/company (kept for interface)
        public List<Order> FilterByCompany(Customer customer)
        {
            if (customer == null || customer.ID == 0)
            {
                return new List<Order>();
            }

            return GetOrdersByCustomerId(customer.ID);
        }

        // Filters by status (not used yet)
        public List<Order> FilterByStatus(OrderStatus status)
        {
            // Not used right now
            return new List<Order>();
        }

        // Filters by date (not used yet)
        public List<Order> FilterByDate(DateTime date)
        {
            // Not used right now
            return new List<Order>();
        }

        // Returns all ingredients with allergies (dropdown usage)
        public Dictionary<int, Ingredient> GetAllIngredients()
        {
            Dictionary<int, Ingredient> ingredients = new Dictionary<int, Ingredient>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "SELECT ingredient.I_ID as ingredientID, ingredient.I_Name as ingredientName, " +
                    "ingredient.I_Quntity as quntityOfIngredient, ingredient.I_ExpireDate as ingredientExpireDate, " +
                    "A.A_ID as allergyID, A.A_Name as allergyName " +
                    "FROM ingredient " +
                    "JOIN IngredientAllergie AS IA ON IA.I_ID = ingredient.I_ID " +
                    "JOIN Allergie AS A ON A.A_ID = IA.A_ID",
                    connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int ingredientId = Convert.ToInt32(reader["ingredientID"]);

                    if (!ingredients.ContainsKey(ingredientId))
                    {
                        Ingredient ingredient = new Ingredient();
                        ingredient.ID = ingredientId;
                        ingredient.Name = reader["ingredientName"].ToString();
                        ingredient.ExpireDate = Convert.ToDateTime(reader["ingredientExpireDate"]);
                        ingredient.Quantity = Convert.ToInt32(reader["quntityOfIngredient"]);

                        ingredients.Add(ingredientId, ingredient);
                    }

                    // Add allergy to ingredient
                    int allergyId = Convert.ToInt32(reader["allergyID"]);
                    string allergyName = reader["allergyName"].ToString();

                    ingredients[ingredientId].Allergies.Add(allergyId, allergyName);
                }

                reader.Close();
            }

            return ingredients;
        }

        // Returns all allergies (dropdown usage)
        public Dictionary<int, string> GetAllAllergies()
        {
            Dictionary<int, string> allergies = new Dictionary<int, string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT A_ID, A_Name FROM Allergie", connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["A_ID"]);
                    string name = reader["A_Name"].ToString();

                    allergies.Add(id, name);
                }

                reader.Close();
            }

            return allergies;
        }

        // Returns all warehouses
        public List<Warehouse> GetAllWarehouses()
        {
            List<Warehouse> warehouses = new List<Warehouse>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "SELECT W_ID, W_Name, W_Type, W_Location FROM dbo.warehouse",
                    connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Warehouse warehouse = new Warehouse();
                    warehouse.ID = Convert.ToInt32(reader["W_ID"]);
                    warehouse.Name = reader["W_Name"].ToString();
                    warehouse.Location = reader["W_Location"].ToString();

                    WarehouseType type;
                    if (Enum.TryParse<WarehouseType>(reader["W_Type"].ToString(), true, out type))
                    {
                        warehouse.Type = type;
                    }

                    warehouses.Add(warehouse);
                }

                reader.Close();
            }

            return warehouses;
        }

        // Gets current warehouse location for a recipe part
        public Warehouse GetRecipePartLocation(int recipePartId)
        {
            Warehouse warehouse = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "SELECT w.W_ID, w.W_Name, w.W_Type, w.W_Location " +
                    "FROM dbo.werehouseRecipePart wrp " +
                    "JOIN dbo.warehouse w ON wrp.W_ID = w.W_ID " +
                    "WHERE wrp.R_ID = @R_ID",
                    connection);

                command.Parameters.AddWithValue("@R_ID", recipePartId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    warehouse = new Warehouse();
                    warehouse.ID = Convert.ToInt32(reader["W_ID"]);
                    warehouse.Name = reader["W_Name"].ToString();
                    warehouse.Location = reader["W_Location"].ToString();

                    WarehouseType type;
                    if (Enum.TryParse<WarehouseType>(reader["W_Type"].ToString(), true, out type))
                    {
                        warehouse.Type = type;
                    }
                }

                reader.Close();
            }

            return warehouse;
        }

        // Updates (or inserts) warehouse location for a recipe part
        public void UpdateRecipePartLocation(int recipePartId, int warehouseId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand update = new SqlCommand(
                    "UPDATE dbo.werehouseRecipePart SET W_ID = @W_ID WHERE R_ID = @R_ID",
                    connection);

                update.Parameters.AddWithValue("@W_ID", warehouseId);
                update.Parameters.AddWithValue("@R_ID", recipePartId);

                int rows = update.ExecuteNonQuery();

                // If no row exists -> insert
                if (rows == 0)
                {
                    SqlCommand insert = new SqlCommand(
                        "INSERT INTO dbo.werehouseRecipePart (W_ID, R_ID) VALUES (@W_ID, @R_ID)",
                        connection);

                    insert.Parameters.AddWithValue("@W_ID", warehouseId);
                    insert.Parameters.AddWithValue("@R_ID", recipePartId);

                    insert.ExecuteNonQuery();
                }
            }
        }

        // Cancels an order by setting status to Cancelled
        public void CancelOrder(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "UPDATE dbo.OrderTable SET O_Status = @Status WHERE O_ID = @Id",
                    connection);

                command.Parameters.AddWithValue("@Status", OrderStatus.Cancelled.ToString());
                command.Parameters.AddWithValue("@Id", orderId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Adds link between order and customer
        private void AddOrderTableCustomer(int orderId, int customerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "INSERT INTO OrderTableCustomer (O_ID, C_ID) VALUES (@O_ID, @C_ID)",
                    connection);

                command.Parameters.AddWithValue("@O_ID", orderId);
                command.Parameters.AddWithValue("@C_ID", customerId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Adds link between recipe part and ingredient
        private void AddRecipePartIngredient(int recipePartId, int ingredientId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "INSERT INTO IngrefientrecipePart (R_ID, I_ID) VALUES (@R_ID, @I_ID)",
                    connection);

                command.Parameters.AddWithValue("@R_ID", recipePartId);
                command.Parameters.AddWithValue("@I_ID", ingredientId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Adds link between order and recipe part
        private void AddOrderRecipePart(int orderId, int recipePartId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "INSERT INTO OrderTableRecipePart (R_ID, O_ID) VALUES (@R_ID, @O_ID)",
                    connection);

                command.Parameters.AddWithValue("@O_ID", orderId);
                command.Parameters.AddWithValue("@R_ID", recipePartId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Adds a recipe part and its ingredient relations, then links to order
        private void AddRecipePart(RecipePart recipePart, int orderId, List<Ingredient> ingredients)
        {
            // Default status for new recipe parts
            recipePart.status = "not begun";

            int newRecipePartId;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "INSERT INTO RecipePart (R_HowToPrep, R_Name, R_Status) " +
                    "VALUES (@R_HowToPrep, @R_Name, @R_Status); " +
                    "SELECT SCOPE_IDENTITY();",
                    connection);

                command.Parameters.AddWithValue("@R_HowToPrep", recipePart.Assemble);
                command.Parameters.AddWithValue("@R_Name", recipePart.partName);
                command.Parameters.AddWithValue("@R_Status", recipePart.status);

                connection.Open();
                newRecipePartId = Convert.ToInt32(command.ExecuteScalar());
            }

            AddOrderRecipePart(orderId, newRecipePartId);

            for (int i = 0; i < ingredients.Count; i++)
            {
                AddRecipePartIngredient(newRecipePartId, ingredients[i].ID);
            }
        }

        // Converts DB string to enum safely
        private OrderStatus ParseOrderStatus(string statusText)
        {
            OrderStatus status;
            if (!Enum.TryParse<OrderStatus>(statusText, out status))
            {
                status = OrderStatus.Created;
            }
            return status;
        }
    }
}
