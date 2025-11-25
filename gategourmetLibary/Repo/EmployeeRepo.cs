using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Reflection;
using gategourmetLibrary.Secret;
using gategourmetLibrary.Repo;

namespace gategourmetLibrary.Models
{
    public class EmployeeRepo:IEmpolyeeRepo
    {
        private readonly string _connectionString;
        private readonly List<Employee> _employee;
        private readonly Connect connect = new Connect();
        public EmployeeRepo()
        {
            _connectionString = connect.cstring;
        }

        public void Add(Employee employee)
        {
            // opretter forbindelse til databasen 
            SqlConnection connection = new SqlConnection(_connectionString);

            // sql kommando: der indsætter ny medarbejder i Employee tabellen
            SqlCommand command = new SqlCommand(
                "INSERT INTO Employee (E_Name, E_Email, E_PhoneNumber, E_Password) " +
                "VALUES (@name, @email, @phonenumber, @password)",
                connection);
            
            // indsætter værdierne i paramenterne 
            command.Parameters.AddWithValue("@name", employee.Name);
            command.Parameters.AddWithValue("@email", employee.Email);
            command.Parameters.AddWithValue("@phonenumber", employee.PhoneNumber);
            command.Parameters.AddWithValue("@password", employee.Password);

            // åben forbindelse til sql
            connection.Open();

            // udføre sql kommando (indsætter data)
            command.ExecuteNonQuery();

            //lukker forbndelsen igen 
            connection.Close();
        }
        public void Delete(int employeeId)
        {
            // opretter fornidelse til sql 
            SqlConnection connection = new SqlConnection(_connectionString);

            // sql kommando der fjerner en medarbejder 
            SqlCommand command = new SqlCommand(
                "DELETE FROM Employee WHERE employee_Id = @id", 
                connection);
           
            // indsætter værdier i parameter
            command.Parameters.AddWithValue("@id", employeeId);

      
            // åben forbindelse
            connection.Open();

            //udføre sql kommando ( slet data)
            command.ExecuteNonQuery();

            //luk forbindelse igen
            connection.Close();


        }
        public void Update(Employee employee)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(
                "UPDATE Employee SET E_Name = @name, E_Email = @email, E_PhoneNumber = @phone, E_Password = @password WHERE Employee_ID = @id", connection);

            command.Parameters.AddWithValue("@name", employee.Name);
            command.Parameters.AddWithValue("@email", employee.Email);
            command.Parameters.AddWithValue("@phone", employee.PhoneNumber);
            command.Parameters.AddWithValue("@password", employee.Password);
            command.Parameters.AddWithValue("@id", employee.Id);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();


        }
        public List<Employee> GetAll()
        {
            //oprette en tom liste til at gemme alle medarbejdere
            List<Employee> employees = new List<Employee>();

            //opret forbindelse
            SqlConnection connection = new SqlConnection(_connectionString);

            //henter alle medarbejder fra sql tabllen 
            SqlCommand command = new SqlCommand("SELECT * FROM Employee", connection);

            //åben forbindelse
            connection.Open();

            // udføre kommando (henter alle medarbejdere) 
            SqlDataReader reader = command.ExecuteReader();

            // loop sikre at vi læser hver eneste medarbejder i databasen.
            while (reader.Read()) 
            {
                //opret et nyt objekt for hvert medarbejder
                Employee employee = new Employee()  
                {
                    // henter data fra databasen og sætter det på Employee objektet 
                    Id = (int)reader["Employee_ID"],  
                    Name = reader["E_Name"].ToString(),
                    Email = reader["E_Email"].ToString(),
                    PhoneNumber = reader["E_PhoneNumber"].ToString()
                };
                //tilføje medarbejder til listen 
                employees.Add(employee);  
            }
            //luk reader  
            reader.Close();

            //luk forbindelse 
            connection.Close();

            //returnere liste med alle medarbejder 
            return employees;
        }

        public Employee Get( int employee)
        {

            SqlConnection connection = new SqlConnection(_connectionString);

            // SQL kommando: Find medarbejder med dette ID
            SqlCommand command = new SqlCommand(
                "SELECT * FROM Employee WHERE Employee_ID = @id", connection);

            // her sætte vi ID parameter
            command.Parameters.AddWithValue("@id", employee);
            
            // vi åbner forbindelsen til databasen 
            connection.Open();
           
            //sql commando til finde resultat
            SqlDataReader reader = command.ExecuteReader();

            //hvis der findes en medarbejder med dette ID
            if (reader.Read())
            {
                // hvis medarbejdern blev fundet, bliver der oprettet en ny objekt med data fra databasen
                Employee employees = new Employee()
                {
                    Id = (int)reader["Employee_ID"],  
                    Name = reader["E_Name"].ToString(),
                    Email = reader["E_Email"].ToString(),
                    PhoneNumber = reader["E_PhoneNumber"].ToString()
                };

                // lukker for reader og returner medarbejderen 
                reader.Close();
                return employees;


            }
            else // hvis der ikke findes en medarbejder med det ID
            {
                // luk for reader og returner null
                reader.Close();
                return null;
            }

            List<Employee> Filter(string empolyee)
            {
                return null;
            }



        }
    }
}
