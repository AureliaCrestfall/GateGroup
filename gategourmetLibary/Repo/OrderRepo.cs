using gategourmetLibrary.Models;
using gategourmetLibrary.Secret;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace gategourmetLibrary.Repo
{
    // Repository class that handles all database operations related to orders.
    // This class is responsible for reading and writing order data and related data
    // (customers, recipe parts, ingredients, warehouses) to the sql database.
    public class OrderRepo : IOrderRepo
    {
        // Stores the current order ID
        public int orderId { get; set; }



        // connection string to the database
        private readonly string _connectionString;

        // constructor gets connection string from outside
        public OrderRepo(string connection)
        {
            _connectionString = connection;
        }

        // Gets all orders from the database as a dictionary.
        public Dictionary<int, Order> GetAll()
        {
            Dictionary<int, Order> ordersFromDatabase = new Dictionary<int, Order>();

            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
                "SELECT o.O_ID, o.O_Made, o.O_Ready, o.O_PaySatus, o.O_Status, " +
                "c.C_ID, c.C_Name " +
                "FROM OrderTable o " +
                "LEFT JOIN OrderTableCustomer oc ON o.O_ID = oc.O_ID " +
                "LEFT JOIN Customer c ON oc.C_ID = c.C_ID ",
                sqlConnection);

            try
            {
                sqlConnection.Open();
                SqlDataReader sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    int id = Convert.ToInt32(sqlReader["O_ID"]);
                    DateTime made = Convert.ToDateTime(sqlReader["O_Made"]);
                    DateTime ready = Convert.ToDateTime(sqlReader["O_Ready"]);
                    bool paystatus = Convert.ToBoolean(sqlReader["O_PaySatus"]);

                    // read status from database stored as a string
                    string statusString = sqlReader["O_Status"].ToString();

                    OrderStatus status;

                    // try to map the string to the enum
                    if (!Enum.TryParse<OrderStatus>(statusString, out status))
                    {
                        // fallback if value in DB is invalid or null
                        status = OrderStatus.Created;
                    }

                    // create order object
                    Order order = new Order(made, ready, id, paystatus);
                    order.Status = status;

                    // load customer information if available
                    if (sqlReader["C_ID"] != DBNull.Value)
                    {
                        order.CustomerOrder = new Customer
                        {
                            ID = Convert.ToInt32(sqlReader["C_ID"]),
                            Name = sqlReader["C_Name"].ToString()
                        };
                    }

                    ordersFromDatabase.Add(id, order);
                }

                sqlReader.Close();
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Database error in OrderRepository.GetAll(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return ordersFromDatabase;
        }

        // Returns all orders as a list 
        public List<Order> GetAllOrders()
        {
            Dictionary<int, Order> ordersFromDatabase = GetAll();

            if (ordersFromDatabase == null)
            {
                return new List<Order>();
            }

            return new List<Order>(ordersFromDatabase.Values);
        }

        // Adds a new order to the database and related tables
        public void AddOrder(Order newOrder)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
                "INSERT INTO OrderTable (O_Made, O_Ready, O_PaySatus, O_Status) " +
                "VALUES (@O_Made, @O_Ready, @O_PaySatus, @O_Status); " +
                "SELECT SCOPE_IDENTITY();",
                sqlConnection);

            sqlCommand.Parameters.AddWithValue("@O_Made", newOrder.OrderMade);
            sqlCommand.Parameters.AddWithValue("@O_Ready", newOrder.OrderDoneBy);
            sqlCommand.Parameters.AddWithValue("@O_PaySatus", newOrder.paystatus);

            // store status as string in the database  created, cancelled
            sqlCommand.Parameters.AddWithValue("@O_Status", newOrder.Status.ToString());

            int newOrderId = 0;

            try
            {
                sqlConnection.Open();
                newOrderId = Convert.ToInt32(sqlCommand.ExecuteScalar());
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Databasefejl i OrderRepository.AddOrder(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            // link order to customer if it exists
            if (newOrder.CustomerOrder != null)
            {
                AddOrderTableCustomert(newOrderId, newOrder.CustomerOrder.ID);
            }

            // add recipe parts and ingredients
            foreach (KeyValuePair<int, RecipePart> part in newOrder.Recipe)
            {
                AddRecipePart(part.Value, newOrderId, part.Value.Ingredients);
            }
        }

        // Method for cancelling an order and updates status to cancelled in the database
        public void CancelOrder(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    // Opdater ordre status
                    using (SqlCommand command = new SqlCommand(
                        "UPDATE dbo.OrderTable SET O_Status = @Status WHERE O_ID = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Status", OrderStatus.Cancelled.ToString());
                        command.Parameters.AddWithValue("@Id", orderId);
                        command.ExecuteNonQuery();
                    }

                    // Hent alle RecipePart IDs for ordren
                    List<int> recipePartIds = new List<int>();
                    using (SqlCommand sqlCommand = new SqlCommand(
                        "SELECT R_ID FROM orderTableRecipePart WHERE O_ID = @id", connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@id", orderId);
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                recipePartIds.Add((int)reader["R_ID"]);
                            }
                        }
                    }

                    // Opdater status på alle RecipeParts
                    foreach (int rId in recipePartIds)
                    {
                        using (SqlCommand commandRecipe = new SqlCommand(
                            "UPDATE dbo.RecipePart SET R_Status = @Status WHERE R_ID = @Id", connection))
                        {
                            commandRecipe.Parameters.AddWithValue("@Status", OrderStatus.Cancelled.ToString());
                            commandRecipe.Parameters.AddWithValue("@Id", rId);
                            commandRecipe.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Database fail in OrderRepository.CancelOrder(): " + ex.Message);
                }
            }
        }


        // Links order to customer in junction table OrderTableCustomer.
        public void AddOrderTableCustomert(int orderID, int customerID)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
                "INSERT INTO OrderTableCustomer (O_ID, C_ID) " +
                "VALUES (@O_ID, @C_ID)",
                sqlConnection);

            Debug.WriteLine("order id is " + orderID + " customer id is " + customerID);

            sqlCommand.Parameters.AddWithValue("@O_ID", orderID);
            sqlCommand.Parameters.AddWithValue("@C_ID", customerID);

            try
            {
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Databasefejl i OrderRepository.AddOrderTableCustomert(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        // adds one relation between a recipe and an ingredient
        public void AddRecipePartIngredient(int recipeID, int ingredientID)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
                "INSERT INTO IngrefientrecipePart (R_ID, I_ID) " +
                "VALUES (@R_ID, @I_ID)",
                sqlConnection);

            sqlCommand.Parameters.AddWithValue("@R_ID", recipeID);
            sqlCommand.Parameters.AddWithValue("@I_ID", ingredientID);

            try
            {
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Databasefejl i OrderRepository.AddRecipePartIngredient(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        // adds one relation between an order and a recipe part
        public void AddOrderRecipePart(int orderID, int recipePartID)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
                "INSERT INTO OrderTableRecipePart (R_ID, O_ID) " +
                "VALUES (@R_ID, @O_ID)",
                sqlConnection);

            sqlCommand.Parameters.AddWithValue("@O_ID", orderID);
            sqlCommand.Parameters.AddWithValue("@R_ID", recipePartID);

            try
            {
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Databasefejl i OrderRepository.AddOrderRecipePart(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        // adds a recipe part and its ingredients, then links it to an order
        public void AddRecipePart(RecipePart recipePart, int orderId, List<Ingredient> ingredients)
        {
            recipePart.status = "not begun";

            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
                "INSERT INTO RecipePart (R_HowToPrep, R_Name, R_Status) " +
                "VALUES (@R_HowToPrep, @R_Name, @R_Status); " +
                "SELECT SCOPE_IDENTITY();",
                sqlConnection);

            sqlCommand.Parameters.AddWithValue("@R_HowToPrep", recipePart.Assemble);
            sqlCommand.Parameters.AddWithValue("@R_Name", recipePart.partName);
            sqlCommand.Parameters.AddWithValue("@R_Status", recipePart.status);

            int newRecipePartId = 0;

            try
            {
                sqlConnection.Open();
                newRecipePartId = Convert.ToInt32(sqlCommand.ExecuteScalar());
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Databasefejl i OrderRepository.AddRecipePart(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            AddOrderRecipePart(orderId, newRecipePartId);

            foreach (Ingredient ingredient in ingredients)
            {
                AddRecipePartIngredient(newRecipePartId, ingredient.ID);
            }
        }

        public void Delete(int orderID)
        {
            // not implemented 
        }

        // Returns a specific order by its ID.
        public Order Get(int orderID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(
                    "SELECT OrderTable.O_ID as orderID, OrderTable.O_Made as made, OrderTable.O_Ready as ready," +
                    "  OrderTable.O_PaySatus as paysatus, OrderTable.O_Status as orderstatus, " +
                    "  RP.R_ID as rid, RP.R_Name as rname, RP.R_HowToPrep as howtoprep, RP.R_Status as rstatus, " +
                    "  i.I_ID as ingID, i.I_Name as ingeName" +
                    "   FROM OrderTable" +
                    "   left join orderTableRecipePart OTP on OTP.O_ID = OrderTable.o_ID " +
                    "  left JOIN RecipePart RP ON OTP.R_ID = RP.R_ID   " +
                    "  left join IngrefientrecipePart IR on IR.R_ID = RP.R_ID     " +
                    "  left join ingredient i on i.I_ID = IR.I_ID " +
                    " where ordertable.O_ID = @id",
                    sqlConnection);

                sqlCommand.Parameters.AddWithValue("@id", orderID);

                try
                {
                    sqlConnection.Open();

                    using (SqlDataReader sqlReader = sqlCommand.ExecuteReader())
                    {
                        Order order = null;

                        while (sqlReader.Read())
                        {
                            if (order == null)
                            {
                                int id = Convert.ToInt32(sqlReader["orderID"]);
                                DateTime made = Convert.ToDateTime(sqlReader["made"]);
                                DateTime ready = Convert.ToDateTime(sqlReader["ready"]);
                                bool paystatus = Convert.ToBoolean(sqlReader["paysatus"]);
                                string statusString = sqlReader["orderstatus"]?.ToString();

                                OrderStatus status;
                                if (!Enum.TryParse<OrderStatus>(statusString, out status))
                                {
                                    status = OrderStatus.Created;
                                }

                                order = new Order(made, ready, id, paystatus, status);
                            }

                            // NOTE: Some orders may have zero recipe parts. That's valid; just return an Order with an empty Recipe dictionary.
                            if (!DBNull.Value.Equals(sqlReader["rid"]))
                            {
                                int rID = Convert.ToInt32(sqlReader["rid"]);

                                if (!order.Recipe.ContainsKey(rID))
                                {
                                    order.Recipe.Add(rID, new RecipePart
                                    {
                                        ID = rID,
                                        partName = sqlReader["rname"]?.ToString(),
                                        Assemble = sqlReader["howtoprep"]?.ToString(),
                                        status = sqlReader["rstatus"]?.ToString(),
                                        Ingredients = new List<Ingredient>()
                                    });
                                }

                                // Ingredients are optional: recipe parts can exist without linked ingredients.
                                if (!DBNull.Value.Equals(sqlReader["ingID"]))
                                {
                                    int ingID = Convert.ToInt32(sqlReader["ingID"]);
                                    string ingeName = sqlReader["ingeName"]?.ToString();

                                    order.Recipe[rID].Ingredients.Add(new Ingredient
                                    {
                                        ID = ingID,
                                        Name = ingeName
                                    });
                                }
                            }
                        }

                        return order;
                    }
                }
                catch (SqlException sqlError)
                {
                    throw new Exception("Database error in OrderRepository.Get(): " + sqlError.Message);
                }
            }
        }

        public void Update(int orderID, Order updateOrder)
        {
            // not used in this project
        }

        public Order filterAfterWhoMade(Employee filterAfterWhoMade)
        {
            return null;
        }

        public Order filterAfterOrderToday(DateTime filterAfterOrderToday)
        {
            return null;
        }

        // Returns all ingredients from the database
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
                    int ingredientId = (int)reader["ingredientID"];

                    if (!ingredients.ContainsKey(ingredientId))
                    {
                        ingredients.Add(ingredientId, new Ingredient
                        {
                            ID = ingredientId,
                            Name = reader["ingredientName"].ToString(),
                            ExpireDate = Convert.ToDateTime(reader["ingredientExpireDate"]),
                            Quantity = (int)reader["quntityOfIngredient"]
                        });
                    }

                    ingredients[ingredientId].Allergies.Add(
                        (int)reader["allergyID"],
                        reader["allergyName"].ToString());
                }
            }

            return ingredients;
        }

        //delete an order by its ID 
        public void DeleteOrder(int ID)
        {


            // get constring from connect class
            string connectionString1 = new Connect().cstring;
            using (SqlConnection conn = new SqlConnection(connectionString1))
            {
                // open connection
                conn.Open();
                string sql2 = @" DELETE FROM orderTableRecipePart WHERE O_ID =@id";
                using (SqlCommand command = new SqlCommand(sql2, conn))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }
                string sql3 = @" DELETE FROM OrderTableCustomer WHERE O_ID =@id";
                using (SqlCommand command = new SqlCommand(sql3, conn))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }

                string sql4 = @" DELETE FROM EmployeeRecipePartOrderTable WHERE O_ID =@id";
                using (SqlCommand command = new SqlCommand(sql4, conn))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }


                string sql = @" DELETE FROM OrderTable WHERE O_ID =@id";

                //execute command
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }
            }





            // get constring from connect class
            string connectionString = new Connect().cstring;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // open connection
                conn.Open();
                string sql2 = @" DELETE FROM orderTableRecipePart WHERE O_ID =@id";
                using (SqlCommand command = new SqlCommand(sql2, conn))
                {
                    command.Parameters.AddWithValue("@id", ID);
                    command.ExecuteNonQuery();
                }
                string sql3 = @" DELETE FROM OrderTableCustomer WHERE O_ID =@id";
                using (SqlCommand command = new SqlCommand(sql3, conn))
                {
                    command.Parameters.AddWithValue("@id", ID);
                    command.ExecuteNonQuery();
                }

                string sql4 = @" DELETE FROM EmployeeRecipePartOrderTable WHERE O_ID =@id";
                using (SqlCommand command = new SqlCommand(sql4, conn))
                {
                    command.Parameters.AddWithValue("@id", ID);
                    command.ExecuteNonQuery();
                }


                string sql = @" DELETE FROM OrderTable WHERE O_ID =@id";

                //execute command
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@id", ID);
                    command.ExecuteNonQuery();
                }
            }
        }

        //update an existing order by its ID 
        public void UpdateOrder(int orderID, Order updatedOrder)
        {
            // not implemented in this project
        }

        //returns a list of recipe parts for a specific order by orderID
        public List<RecipePart> GetRecipeParts(int orderID)
        {
            return null;
        }

        //filters orders made by a specific employee
        public List<Order> FilterByEmployee(Employee employee)
        {
            return null;
        }

        //filters orders placed today
        public List<Order> FilterByToday(DateTime today)
        {
            List<Order> orders = new List<Order>();

            DateTime startDate = today.Date;
            DateTime endDate = today.Date.AddDays(1);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "SELECT O_ID, O_Made, O_Ready, O_PaySatus, O_Status " +
                    "FROM dbo.OrderTable " +
                    "WHERE O_Made >= @StartDate AND O_Made < @EndDate",
                    connection);

                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["O_ID"]);
                    DateTime made = Convert.ToDateTime(reader["O_Made"]);
                    DateTime ready = Convert.ToDateTime(reader["O_Ready"]);
                    bool paystatus = Convert.ToBoolean(reader["O_PaySatus"]);
                    string statusString = reader["O_Status"].ToString();

                    OrderStatus status;
                    if (!Enum.TryParse<OrderStatus>(statusString, out status))
                    {
                        status = OrderStatus.Created;
                    }

                    Order order = new Order(made, ready, id, paystatus);
                    order.Status = status;

                    orders.Add(order);
                }
            }

            return orders;
        }

        // filters orders by department
        public List<Order> FilterByDepartment(int departmentId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "SELECT DISTINCT o.O_ID, o.O_Made, o.O_Ready, o.O_PaySatus, o.O_Status " +
                    "FROM dbo.OrderTable o " +
                    "INNER JOIN dbo.EmployeeRecipePartOrderTable ero ON o.O_ID = ero.O_ID " +
                    "INNER JOIN dbo.EmployeeDepartment ed ON ero.E_ID = ed.E_ID " +
                    "WHERE ed.D_ID = @DepartmentId",
                    connection);

                command.Parameters.AddWithValue("@DepartmentId", departmentId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["O_ID"]);
                    DateTime made = Convert.ToDateTime(reader["O_Made"]);
                    DateTime ready = Convert.ToDateTime(reader["O_Ready"]);
                    bool paystatus = Convert.ToBoolean(reader["O_PaySatus"]);
                    string statusString = reader["O_Status"].ToString();

                    OrderStatus status;
                    if (!Enum.TryParse<OrderStatus>(statusString, out status))
                    {
                        status = OrderStatus.Created;
                    }

                    Order order = new Order(made, ready, id, paystatus);
                    order.Status = status;

                    orders.Add(order);
                }
            }

            return orders;
        }

        //filters orders for a specific customer/company
        public List<Order> FilterByCompany(Customer customer)
        {
            if (customer == null || customer.ID == 0)
            {
                return new List<Order>();
            }

            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // sql command to select orders for the customer
                SqlCommand command = new SqlCommand(
                    "SELECT o.O_ID, o.O_Made, o.O_Ready, o.O_PaySatus, o.O_Status " +
                    "FROM dbo.OrderTable o " +
                    "INNER JOIN dbo.OrderTableCustomer oc ON o.O_ID = oc.O_ID " +
                    "WHERE oc.C_ID = @CustomerId",
                    connection);

                command.Parameters.AddWithValue("@CustomerId", customer.ID);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["O_ID"]);
                    DateTime made = Convert.ToDateTime(reader["O_Made"]);
                    DateTime ready = Convert.ToDateTime(reader["O_Ready"]);
                    bool paystatus = Convert.ToBoolean(reader["O_PaySatus"]);
                    string statusString = reader["O_Status"].ToString();

                    OrderStatus status;
                    if (!Enum.TryParse<OrderStatus>(statusString, out status))
                    {
                        status = OrderStatus.Created;
                    }

                    Order order = new Order(made, ready, id, paystatus);
                    order.Status = status;
                    order.CustomerOrder = customer;

                    orders.Add(order);
                }
            }

            return orders;
        }

        //filters orders by their status
        public List<Order> FilterByStatus(OrderStatus status)
        {
            return null;
        }

        //filters orders by a specific date
        public List<Order> FilterByDate(DateTime date)
        {
            return null;
        }

        public void filterAfterCompany(Customer filterAfterCompany)
        {
        }

        public void filterAfterStatus(Enum filterAfterStatus)
        {
        }

        public void filterAfterDate(DateTime filterAfterDate)
        {
        }

        // Returns all allergies from the database.
        public Dictionary<int, string> GetAllAllergies()
        {
            Dictionary<int, string> allergiesFromDatabase = new Dictionary<int, string>();

            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
                "SELECT A_ID, A_Name FROM Allergie",
                sqlConnection);

            try
            {
                sqlConnection.Open();
                SqlDataReader sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    int id = Convert.ToInt32(sqlReader["A_ID"]);
                    string name = sqlReader["A_Name"].ToString();

                    allergiesFromDatabase.Add(id, name);
                }

                sqlReader.Close();
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Database error in OrderRepository.GetAllAllergies(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return allergiesFromDatabase;
        }

        // returns all warehouses (for example freezer, fridge, dry storage)
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
            }

            return warehouses;
        }

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
            }

            return warehouse;
        }

        public void UpdateRecipePartLocation(int recipePartId, int warehouseId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand update = new SqlCommand(
                    "UPDATE dbo.werehouseRecipePart " +
                    "SET W_ID = @W_ID " +
                    "WHERE R_ID = @R_ID",
                    connection);

                update.Parameters.AddWithValue("@W_ID", warehouseId);
                update.Parameters.AddWithValue("@R_ID", recipePartId);

                int rows = update.ExecuteNonQuery();

                if (rows == 0)
                {
                    SqlCommand insert = new SqlCommand(
                        "INSERT INTO dbo.werehouseRecipePart (W_ID, R_ID) " +
                        "VALUES (@W_ID, @R_ID)",
                        connection);

                    insert.Parameters.AddWithValue("@W_ID", warehouseId);
                    insert.Parameters.AddWithValue("@R_ID", recipePartId);

                    insert.ExecuteNonQuery();
                }
            }
        }


             public Dictionary<int, Order> GetAllFromID(int idcust)
        {
            Dictionary<int, Order> ordersFromDatabase = new Dictionary<int, Order>();

            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
                "SELECT o.O_ID, o.O_Made, o.O_Ready, o.O_PaySatus, o.O_Status, " +
                "c.C_ID, c.C_Name " +
                "FROM OrderTable o " +
                "LEFT JOIN OrderTableCustomer oc ON o.O_ID = oc.O_ID " +
                "LEFT JOIN Customer c ON oc.C_ID = c.C_ID " +
                " where c.C_ID = @id",
                sqlConnection);
            sqlCommand.Parameters.AddWithValue("@id", idcust);

            try
            {
                sqlConnection.Open();
                SqlDataReader sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    int id = Convert.ToInt32(sqlReader["O_ID"]);
                    DateTime made = Convert.ToDateTime(sqlReader["O_Made"]);
                    DateTime ready = Convert.ToDateTime(sqlReader["O_Ready"]);
                    bool paystatus = Convert.ToBoolean(sqlReader["O_PaySatus"]);

                    // read status from database stored as a string
                    string statusString = sqlReader["O_Status"].ToString();

                    OrderStatus status;

                    // try to map the string to the enum
                    if (!Enum.TryParse<OrderStatus>(statusString, out status))
                    {
                        // fallback if value in DB is invalid or null
                        status = OrderStatus.Created;
                    }

                    // create order object
                    Order order = new Order(made, ready, id, paystatus);
                    order.Status = status;

                    // load customer information if available
                    if (sqlReader["C_ID"] != DBNull.Value)
                    {
                        order.CustomerOrder = new Customer
                        {
                            ID = Convert.ToInt32(sqlReader["C_ID"]),
                            Name = sqlReader["C_Name"].ToString()
                        };
                    }

                    ordersFromDatabase.Add(id, order);
                }

                sqlReader.Close();
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Database error in OrderRepository.GetAll(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return ordersFromDatabase;
        }

        // Returns all orders as a list 
        public List<Order> GetAllOrdersFromid(int id)
        {
            Dictionary<int, Order> ordersFromDatabase = GetAllFromID(id);

            if (ordersFromDatabase == null)
            {
                return new List<Order>();
            }

            return new List<Order>(ordersFromDatabase.Values);
        }
    }
}
