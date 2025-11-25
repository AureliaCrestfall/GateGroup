using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.Data.SqlClient;

namespace gategourmetLibrary.Repo
{
    public class DepartmentRepo : IDepartmentRepo
    {
        // connection string bruges til at kommuniker med database 
        private readonly string _connectionString;

        // constructor modtager connection string fra service 
        public DepartmentRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        // returns all departments
        public List<Department> GetAllDepartments()
        {
            List<Department> departments = new List<Department>();
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(
                "SELECT D_ID, D_Name, D_Location, D_Email FROM Department",
                connection);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            // gennemgår hver department fra hver table 
            while (reader.Read())
            {
                // der laves et objekt for hver række 
                Department department = new Department();
                department.DepartmentId = (int)reader["D_ID"];
                department.DepartmentName = reader["D_Name"].ToString();
                department.DepartmentLocation = reader["D_Location"].ToString();
                department.DepartmentEmail = reader["email"].ToString();

                departments.Add(department);
            }

            reader.Close();
            connection.Close();

            return departments;
        }
        // adds a new department to database 
        public void AddDepartment(Department newDepartment)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(
                "INSERT INTO Department (D_Name, D_Location, D_Email)" +
                "VALUES (@name, @location, @mail)",
                connection);

            command.Parameters.AddWithValue("@name", newDepartment.DepartmentName);
            command.Parameters.AddWithValue("@location", newDepartment.DepartmentLocation);
            command.Parameters.AddWithValue("@mail", newDepartment.DepartmentEmail);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

        }
        // deletes a department with matching ID
        public void DeleteDepartment(int departmentId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            SqlCommand command = new SqlCommand(
               "DELETE FROM Departments WHERE D_ID = @id",
               connection);

            command.Parameters.AddWithValue("@id", departmentId);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

        }
        // updates department info by ID
        public void UpdateDepartment(int departmentId, Department updatedDepartment)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            SqlCommand command = new SqlCommand(
                "UPDATE Department SET D_Name =@name,D_Location=@location,D_Email=@mail" +
                "Where D_ID = @id",
                connection);

            command.Parameters.AddWithValue("@name", updatedDepartment.DepartmentName);
            command.Parameters.AddWithValue("@location", updatedDepartment.DepartmentLocation);
            command.Parameters.AddWithValue("@mail",updatedDepartment.DepartmentEmail);
            command.Parameters.AddWithValue("@id", updatedDepartment.DepartmentId);

            connection.Open ();
            command.ExecuteNonQuery();
            connection.Close ();
        }

        // returns a specific department by ID
        public Department GetDepartment(int departmentId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(
                "SELECT D_ID, D_Name, D_Location, D_Email FROM Department WHERE D_ID =@id",
                connection);

            command.Parameters.AddWithValue("@id",departmentId);

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
         

        // assigns a new warehouse to a department
        public void NewWarehouse(Warehouse newWarehouse)
        {
            SqlConnection connection = new SqlConnection (_connectionString);

            connection.Open ();

            // indsætter warehouse og retunerer generet W_id
            SqlCommand insertWarehouse = new SqlCommand (
                "INSERT INTO Warehouse (W_Name, W_Type, W_Location)" +
                "VALUES (@name, @type,@location); SELECT SCOPE_IDENTITY();",
                connection);

            insertWarehouse.Parameters.AddWithValue("@name", newWarehouse.Name);
            insertWarehouse.Parameters.AddWithValue("@type", newWarehouse.Type);
            insertWarehouse.Parameters.AddWithValue("@location", newWarehouse.Location);

            object result = insertWarehouse.ExecuteScalar();
            int warehouseId = Convert.ToInt32(result);

            // linker warehouse til department 
            SqlCommand link = new SqlCommand (
                "INSERT INTO werehouseDepartment (D_ID, W_ID) VALUES (@d, @w)",
                connection);

            link.Parameters.AddWithValue("@d", newWarehouse.DepartmentId);
            link.Parameters.AddWithValue("@w", warehouseId);
            link.ExecuteNonQuery ();

            connection.Close ();

        }
        // stocks an ingredient in the department's warehouse
        public void StockIngredient(Ingredient stockIngredient)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

        }
        // gets the stock of a specific warehouse
        public List<Ingredient> GetWarehouseStock(int warehouseId)
        {
            return null;
        }
        // gets the managers of a specific department
        public List<Manager> GetDepartmentManagers(int departmentId)
        {
            return null;
        }
        // gets the employees of a specific department
        public List<Employee> GetDepartmentEmployees(int departmentId)
        {
            return null;
        }
        // adds a new manager to a department
        public void AddNewDepartmentManager(int departmentId, Manager newManager)
        {

        }
        // adds a new employee to a department
        public void AddNewDepartmentEmployee(int departmentId, Employee newEmployee)
        {

        }
        // removes stock from a department's warehouse
        public void RemoveStock(Ingredient ingredient, int amount, int departmentID, int warehouseID)
        {

        }



        //public void Add(int department)
        //{

        //}

        //public void Delete(int department)
        //{

        //}

        //public void Update(int DepartmentID, Department UpdateDepartment  )
        //{

        //}

        //public void GetAll()
        //{

        //}

        //public void NewWarehouse(WereHouse newWarehouse)
        //{

        //}

        //public void StokIngredient(Ingredient stockIngredient)
        //{

        //}
        //public void GetWarehouseStock(int warehouse)
        //{

        //}

        //public void GetDepartmentManagers(int department)
        //{

        //}

        //public void GetDepartmentEmployees(int department)
        //{

        //}

        //public int Get( int DepartmentID )
        //{
        //    int i = 1;
        //    return i;
        //}

        //public void AddNewDepartmentManager(int DepartmentID,manager newManager) 
        //{

        //}


        //public void  AddNewDepartmentEmpolyee(int DepartmentID, Employee newEmployee)
        //{

        //}

        //public void RemoveStock(Ingredient ingredient, int amount, Department departmentID, Warehouse warehouseID)
        //{

        //}

    }
}
