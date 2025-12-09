using gategourmetLibrary.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace gategourmetLibrary.Repo
{
    public class DepartmentRepo : IDepartmentRepo
    {
        // bruges til at oprette forbindelse til databasen
        private readonly string _connectionString;

        // modtager connectionstring fra service laget
        public DepartmentRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        // henter alle afdelinger fra databasen
        public List<Department> GetAllDepartments()
        {
            List<Department> departments = new List<Department>();
            SqlConnection connection = new SqlConnection(_connectionString);

            SqlCommand command = new SqlCommand(
                "SELECT D_ID, D_Name, D_Location, D_Email FROM Department",
                connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            // gennemgår alle rækker og laver department objekter
            while (reader.Read())
            {
                Department department = new Department();
                department.DepartmentId = (int)reader["D_ID"];
                department.DepartmentName = reader["D_Name"].ToString();
                department.DepartmentLocation = reader["D_Location"].ToString();
                department.DepartmentEmail = reader["D_Email"].ToString();

                departments.Add(department);
            }

            reader.Close();
            connection.Close();
            return departments;
        }

        public List<OrderItem> GetOrderStockLocations(int orderId)
        {
            List<OrderItem> items = new List<OrderItem>();

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("Connection string er null eller tom i DepartmentRepo");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection == null)
                    {
                        throw new Exception("SqlConnection kunne ikke oprettes");
                    }

                    connection.Open();

                    using (SqlCommand command = new SqlCommand(
                     "select orderTableRecipePart.O_ID, ingredient.I_Quntity, ingredient.I_ID, ingredient.I_Name, ingredient.I_ExpireDate," +
                     "warehouse.W_ID, warehouse.W_Name, warehouse.W_Location" +
                     "from orderTableRecipePart" +
                     "join RecipePart on RecipePart.R_ID = orderTableRecipePart.R_ID" +
                     "join IngrefientrecipePart on RecipePart.R_ID = IngrefientrecipePart.R_ID" +
                     "join ingredient on ingredient.I_ID = IngrefientrecipePart.I_ID" +
                     "join warehouseIngredient on warehouseIngredient.I_ID = ingredient.I_ID" +
                     "join warehouse on warehouse.W_ID = warehouseIngredient.I_ID" +
                      "where O_ID = @id" ,
                        connection))
                    {
                        command.Parameters.AddWithValue("@id", orderId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderItem item = new OrderItem();

                                item.OrderItemId = (int)reader["orderTableRecipePart.O_ID"];
                                item.OrderId = orderId;
                                item.IngredientId = (int)reader["ingredient.I_ID"];
                                item.Quantity = (int)reader["ingredient.I_quntity"];
                                item.WarehouseId = (int)reader["warehouse.W_ID"];

                                item.Ingredient = new Ingredient
                                {
                                    ID = (int)reader["ingredient.I_ID"],
                                    Name = reader["ingredient.I_Name"].ToString(),
                                    ExpireDate = (DateTime)reader["ingredient.I_expireDate"],
                                    Quantity = (int)reader["ingredient.I_quntity"]
                                };

                                item.Warehouse = new Warehouse
                                {
                                    ID = (int)reader["warehouse.W_ID"],
                                    Name = reader["warehouse.W_Name"].ToString(),
                                    Location = reader["warehouse.W_Location"].ToString()
                                };

                                items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlError)
            {
                throw new Exception($"Database fejl i GetOrderStockLocations(): {sqlError.Message}", sqlError);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fejl i GetOrderStockLocations(): {ex.Message}", ex);
            }

            return items;
        }



        // opretter en ny afdeling
        public void AddDepartment(Department newDepartment)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(
                "INSERT INTO Department (D_Name, D_Location, D_Email) VALUES (@name, @location, @mail)",
                connection);

            command.Parameters.AddWithValue("@name", newDepartment.DepartmentName);
            command.Parameters.AddWithValue("@location", newDepartment.DepartmentLocation);
            command.Parameters.AddWithValue("@mail", newDepartment.DepartmentEmail);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        // sletter en afdeling ud fra id
        public void DeleteDepartment(int departmentId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(
               "DELETE FROM Department WHERE D_ID = @id",
               connection);

            command.Parameters.AddWithValue("@id", departmentId);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        // ændrer information om en afdeling
        public void UpdateDepartment(int departmentId, Department updatedDepartment)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(
                "UPDATE Department SET D_Name = @name, D_Location = @location, D_Email = @mail WHERE D_ID = @id",
                connection);

            command.Parameters.AddWithValue("@name", updatedDepartment.DepartmentName);
            command.Parameters.AddWithValue("@location", updatedDepartment.DepartmentLocation);
            command.Parameters.AddWithValue("@mail", updatedDepartment.DepartmentEmail);
            command.Parameters.AddWithValue("@id", departmentId);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        // finder en afdeling ud fra id
        public Department GetDepartment(int departmentId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(
                "SELECT D_ID, D_Name, D_Location, D_Email FROM Department WHERE D_ID = @id",
                connection);

            command.Parameters.AddWithValue("@id", departmentId);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Department department = new Department();
                department.DepartmentId = (int)reader["D_ID"];
                department.DepartmentName = reader["D_Name"].ToString();
                department.DepartmentLocation = reader["D_Location"].ToString();
                department.DepartmentEmail = reader["D_Email"].ToString();

                reader.Close();
                connection.Close();
                return department;
            }

            reader.Close();
            connection.Close();
            return null;
        }

        // opretter et nyt lager (warehouse) og knytter det til en afdeling
        public void NewWarehouse(Warehouse newWarehouse)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand insertWarehouse = new SqlCommand(
                "INSERT INTO Warehouse (W_Name, W_Type, W_Location) VALUES (@name, @type, @location); SELECT SCOPE_IDENTITY();",
                connection);

            insertWarehouse.Parameters.AddWithValue("@name", newWarehouse.Name);
            insertWarehouse.Parameters.AddWithValue("@type", newWarehouse.Type);
            insertWarehouse.Parameters.AddWithValue("@location", newWarehouse.Location);

            object result = insertWarehouse.ExecuteScalar();
            int warehouseId = Convert.ToInt32(result);

            SqlCommand link = new SqlCommand(
                "INSERT INTO werehouseDepartment (D_ID, W_ID) VALUES (@d, @w)",
                connection);

            link.Parameters.AddWithValue("@d", newWarehouse.DepartmentId);
            link.Parameters.AddWithValue("@w", warehouseId);
            link.ExecuteNonQuery();

            connection.Close();
        }

        // lægger en ingrediens på lager og sikrer en kobling til warehouse
        public void StockIngredient(Ingredient stockIngredient)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand updateQuantity = new SqlCommand(
                "UPDATE Ingredient SET I_quntity = I_quntity + @amount WHERE I_ID = @id",
                connection);

            updateQuantity.Parameters.AddWithValue("@amount", stockIngredient.Quantity);
            updateQuantity.Parameters.AddWithValue("@id", stockIngredient.ID);
            updateQuantity.ExecuteNonQuery();

            SqlCommand linkCheck = new SqlCommand(
                "SELECT COUNT(*) FROM werehouseIngredient WHERE W_ID = @w AND I_ID = @i",
                connection);

            linkCheck.Parameters.AddWithValue("@w", stockIngredient.WarehouseId);
            linkCheck.Parameters.AddWithValue("@i", stockIngredient.ID);

            int exists = Convert.ToInt32(linkCheck.ExecuteScalar());

            if (exists == 0)
            {
                SqlCommand createLink = new SqlCommand(
                    "INSERT INTO werehouseIngredient (W_ID, I_ID) VALUES (@w, @i)",
                    connection);

                createLink.Parameters.AddWithValue("@w", stockIngredient.WarehouseId);
                createLink.Parameters.AddWithValue("@i", stockIngredient.ID);
                createLink.ExecuteNonQuery();
            }

            connection.Close();
        }

        // henter alle ingredienser på lageret i en bestemt warehouse
        public List<Ingredient> GetWarehouseStock(int warehouseId)
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            SqlConnection connection = new SqlConnection(_connectionString);

            SqlCommand command = new SqlCommand(
                "SELECT i.I_ID, i.I_Name, i.I_expireDate, i.I_quntity " +
                "FROM werehouseIngredient wi " +
                "JOIN Ingredient i ON wi.I_ID = i.I_ID " +
                "WHERE wi.W_ID = @id",
                connection);

            command.Parameters.AddWithValue("@id", warehouseId);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            // laver Ingredient objekter fra sql
            while (reader.Read())
            {
                Ingredient ingredient = new Ingredient();
                ingredient.ID = (int)reader["I_ID"];
                ingredient.Name = reader["I_Name"].ToString();
                ingredient.ExpireDate = (DateTime)reader["I_expireDate"];
                ingredient.Quantity = (int)reader["I_quntity"];
                ingredient.WarehouseId = warehouseId;

                ingredients.Add(ingredient);
            }

            reader.Close();
            connection.Close();
            return ingredients;
        }

        // henter alle ansatte i en afdeling
        public List<Employee> GetDepartmentEmployees(int departmentId)
        {
            List<Employee> employees = new List<Employee>();
            SqlConnection connection = new SqlConnection(_connectionString);

            SqlCommand command = new SqlCommand(
                "SELECT e.Employee_ID, e.E_Name, e.E_Email, e.E_PhoneNumber " +
                "FROM EmployeeDepartment ed " +
                "JOIN Employee e ON ed.E_ID = e.Employee_ID " +
                "WHERE ed.D_ID = @id",
                connection);

            command.Parameters.AddWithValue("@id", departmentId);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Employee employee = new Employee();
                employee.Id = (int)reader["Employee_ID"];
                employee.Name = reader["E_Name"].ToString();
                employee.Email = reader["E_Email"].ToString();
                employee.PhoneNumber = reader["E_PhoneNumber"].ToString();

                employees.Add(employee);
            }

            reader.Close();
            connection.Close();
            return employees;
        }

        // henter alle positioner (roller) knyttet til ansatte i en afdeling
        public List<Position> GetDepartmentPositions(int departmentId)
        {
            List<Position> positions = new List<Position>();
            SqlConnection connection = new SqlConnection(_connectionString);

            SqlCommand command = new SqlCommand(
                "SELECT DISTINCT p.P_ID, p.P_Name " +
                "FROM EmployeeDepartment ed " +
                "JOIN EmployeePostion ep ON ed.E_ID = ep.E_ID " +
                "JOIN Postion p ON ep.P_ID = p.P_ID " +
                "WHERE ed.D_ID = @id",
                connection);

            command.Parameters.AddWithValue("@id", departmentId);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Position position = new Position();
                position.Id = (int)reader["P_ID"];
                position.Name = reader["P_Name"].ToString();
                positions.Add(position);
            }

            reader.Close();
            connection.Close();
            return positions;
        }

        // knytter en position (rolle) til en employee i en bestemt afdeling
        public void AddNewDepartmentPosition(int departmentId, Employee employee, Position newPosition)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            SqlCommand command = new SqlCommand(
                "INSERT INTO EmployeePostion (E_ID, P_ID) VALUES (@e, @p)",
                connection);

            // bruger employee Id fra employee objektet
            command.Parameters.AddWithValue("@e", employee.Id);
            command.Parameters.AddWithValue("@p", newPosition.Id);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }


        // knytter en employee til en afdeling
        public void AddNewDepartmentEmployee(int departmentId, Employee newEmployee)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            SqlCommand command = new SqlCommand(
                "INSERT INTO EmployeeDepartment (E_ID, D_ID) VALUES (@e, @d)",
                 connection);

            command.Parameters.AddWithValue("@e", newEmployee.Id);
            command.Parameters.AddWithValue("@d", departmentId);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        // fjerner ingredienser fra lager og sletter kobling når mængden er 0
        public void RemoveStock(Ingredient ingredient, int amount, int departmentID, int warehouseID)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand update = new SqlCommand(
                "UPDATE Ingredient SET I_quntity = I_quntity - @amount WHERE I_ID = @id",
                connection);

            update.Parameters.AddWithValue("@amount", amount);
            update.Parameters.AddWithValue("@id", ingredient.ID);
            update.ExecuteNonQuery();

            SqlCommand getQuantity = new SqlCommand(
                "SELECT I_quntity FROM Ingredient WHERE I_ID = @id",
                connection);

            getQuantity.Parameters.AddWithValue("@id", ingredient.ID);
            object quantityObj = getQuantity.ExecuteScalar();

            if (quantityObj == null || quantityObj == DBNull.Value)
            {
                connection.Close();
                return;
            }

            int quantity = Convert.ToInt32(quantityObj);

            if (quantity <= 0)
            {
                SqlCommand removeLink = new SqlCommand(
                    "DELETE FROM werehouseIngredient WHERE W_ID = @w AND I_ID = @i",
                    connection);

                removeLink.Parameters.AddWithValue("@w", warehouseID);
                removeLink.Parameters.AddWithValue("@i", ingredient.ID);
                removeLink.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}

