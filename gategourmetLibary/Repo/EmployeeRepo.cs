using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace gategourmetLibrary.Models
{
    public class EmployeeRepo
    {
        private readonly string _connectionString;
        private readonly List<Employee> _employee;

        public void Add(Employee employee)
        {
            
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(
                "INSERT INTO Employee (Name, Email, PhoneNumber) " +
                "VALUES (@name, @email, @phone)",
                connection);

            command.Parameters.AddWithValue("@name", employee.Name);
            command.Parameters.AddWithValue("@email", employee.Email);
            command.Parameters.AddWithValue("@phone", employee.PhoneNumber);


            connection.Open();
            command.ExecuteNonQuery();
        }
        public void Delete(int employeeID)
        {
        }
        public void Update(Employee employee)
        {
        }
        public List<Employee> GetAll()
        {

        }

        public Employee Get( int employee)
        {
            int i = 1;
            return i;
        }
    }
}
