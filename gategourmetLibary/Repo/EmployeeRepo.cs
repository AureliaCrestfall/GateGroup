using gategourmetLibary.Models;
using gategourmetLibrary.Repo;
using gategourmetLibrary.Secret;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace gategourmetLibrary.Models
{
    public class EmployeeRepo : IEmpolyeeRepo
    {
        private readonly string _connectionString;
        private readonly List<Employee> _employee;
        public EmployeeRepo(string connectionString)
        {
            _connectionString = connectionString;
            _employee = new List<Employee>();
        }

        public void Add(Employee employee)
        {
            // opretter forbindelse til databasen 
            SqlConnection connection = new SqlConnection(_connectionString);

            // sql kommando: der indsætter ny medarbejder i Employee tabellen
            SqlCommand command = new SqlCommand(
                "INSERT INTO Employee (E_ID,E_Name, E_Email, E_Password) " +
                "VALUES (@id,@name, @email, @password)",
                connection);

            // indsætter værdierne i paramenterne
            command.Parameters.AddWithValue("@id", employee.Id);
            command.Parameters.AddWithValue("@name", employee.Name);
            command.Parameters.AddWithValue("@email", employee.Email);
            command.Parameters.AddWithValue("@password", employee.Password);

            // åben forbindelse til sql
            connection.Open();

            // udføre sql kommando (indsætter data)
            command.ExecuteNonQuery();

            //lukker forbindelsen igen 
            connection.Close();

            AddPhonenumber(employee.PersonalPhoneNumber,employee.Id);
            AddPhonenumber(employee.WorkPhoneNumber,employee.Id);
        }

        void AddPhonenumber(string phone,int employeeID)
        {
            // opretter forbindelse til databasen 
            SqlConnection connection = new SqlConnection(_connectionString);

            // sql kommando: der indsætter ny medarbejder i Employee tabellen
            SqlCommand command = new SqlCommand(
                "insert into Phone(P_number)" +
                "values(@number)" +
                "select scope_identity()",
                connection);

            // indsætter værdierne i paramenterne 
            command.Parameters.AddWithValue("@number", phone);
            int phoneID = 0;

            try
            {            
                // åben forbindelse til sql
                connection.Open();
                // udføre sql kommando (indsætter data)
                command.ExecuteNonQuery();

                phoneID = Convert.ToInt32(command.ExecuteScalar());
                AddEmployeePhoneLink(phoneID, employeeID);

            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                //lukker forbindelsen igen 
                connection.Close();
            }
        }

        void AddEmployeePhoneLink(int phoneID,int employeeID)
        {
            // opretter forbindelse til databasen 
            SqlConnection connection = new SqlConnection(_connectionString);

            // sql kommando: der indsætter ny medarbejder i Employee tabellen
            SqlCommand command = new SqlCommand(
                "insert into employeePhone(E_ID,P_ID)" +
                "values(@e_ID,@p_ID)",
                connection);

            // indsætter værdierne i paramenterne 
            command.Parameters.AddWithValue("@p_ID", phoneID);
            command.Parameters.AddWithValue("@e_ID", employeeID);


            try
            {
                // åben forbindelse til sql
                connection.Open();
                // udføre sql kommando (indsætter data)
                command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //lukker forbindelsen igen 
                connection.Close();
            }
        }


        public void Delete(int employeeId)
        {
            // opretter fornidelse til sql 
            SqlConnection connection = new SqlConnection(_connectionString);

            // sql kommando der fjerner en medarbejder 
            SqlCommand command = new SqlCommand(
                " delete from EmployeeRecipePartOrderTable where E_ID=@id;" +
                " delete from EmployeePostion where E_ID=@id;" +
                " delete from EmployeeDepartment where E_ID=@id;" +
                " delete from employeePhone where E_ID=@id;" +
                " DELETE FROM Employye WHERE E_Id = @id",
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
            command.Parameters.AddWithValue("@phone", employee.WorkPhoneNumber);
            command.Parameters.AddWithValue("@password", employee.Password);
            command.Parameters.AddWithValue("@id", employee.Id);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();


        }
        public Dictionary<int,Employee> GetAll()
        {
            //oprette en tom liste til at gemme alle medarbejdere
            Dictionary<int,Employee> employees = new Dictionary<int, Employee>();

            //opret forbindelse
            SqlConnection connection = new SqlConnection(_connectionString);

            //henter alle medarbejder fra sql tabllen 
            SqlCommand command = new SqlCommand("select Employee.E_Name as employeeName, Employee.E_ID as employeeId,Employee.E_Email as employeeEmail,Employee.E_Password as employeePassword" +
                ",Phone.P_number as phoneNumber,Phone.P_ID as phoneid from Employee" +
                " join employeePhone on Employee.E_ID = employeePhone.E_ID" +
                " join Phone on employeePhone.P_ID = Phone.P_ID", connection);

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
                        Id = Convert.ToInt32(reader["employeeId"]),
                        Name = reader["employeeName"].ToString(),
                        Email = reader["employeeEmail"].ToString(),
                        Password = reader["employeePassword"].ToString()
                    };
                if (!employees.ContainsKey(employee.Id))
                {
                    //tilføje medarbejder til listen 
                    employees.Add(employee.Id, employee);
                }
               


                int phone = (int)reader["phoneid"];
                if (phone % 2 == 0)
                {
                    employees[(int)reader["employeeId"]].WorkPhoneNumber = reader["phoneNumber"].ToString();
                }
                else if (phone % 2 != 0)
                {
                    employees[(int)reader["employeeId"]].PersonalPhoneNumber = reader["phoneNumber"].ToString();
                }

            }
            //luk reader  
            reader.Close();

            //luk forbindelse 
            connection.Close();

            //returnere liste med alle medarbejder 
            return employees;
        }

        public Employee Get(int employee)
        {

            SqlConnection connection = new SqlConnection(_connectionString);

            // SQL kommando: Find medarbejder med dette ID
            SqlCommand command = new SqlCommand(
                "SELECT * FROM Employee WHERE E_ID = @id", connection);

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
                    Id = (int)reader["E_ID"],
                    Name = reader["E_Name"].ToString(),
                    Email = reader["E_Email"].ToString(),
                    Password = reader["E_Password"].ToString()
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
        }

        public List<Employee> Filter(string empolyee)
        {
            return null;
        }


        public Dictionary<int, string> GetAllPostions()
        {
            Dictionary<int, string> databasePostions = new Dictionary<int, string>();
            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand("select Pos_ID,Pos_Name from Postion", sqlConnection);

            try
            {

            
            sqlConnection.Open();
            SqlDataReader sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                string postionName = Convert.ToString(sqlReader["Pos_Name"]);
                int postionID = Convert.ToInt32(sqlReader["Pos_ID"]);

                databasePostions.Add(postionID, postionName);

            }

            sqlReader.Close();
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Database error in EmployeeRepo.GetAllPostions(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return databasePostions;
        }

        public Dictionary<int,Employee> GetEmployeeFromOrderID(int orderid)
        {
            Dictionary<int, Employee> databaseemployees = new Dictionary<int, Employee>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("select Employee.E_Name as employeeName, " +
                 "Employee.E_ID as employeeId, Employee.E_Email as employeeEmail, Employee.E_Password as employeePassword" +
                 ", Phone.P_number as phoneNumber, Phone.P_ID as phoneid, " +
                 "EmployeeRecipePartOrderTable.R_ID as rid" +
                 " from Employee " +
                 " join employeePhone on Employee.E_ID = employeePhone.E_ID" +
                 " join Phone on employeePhone.P_ID = Phone.P_ID " +
                 " join EmployeeRecipePartOrderTable on EmployeeRecipePartOrderTable.E_ID = Employee.E_ID " +
                 "where EmployeeRecipePartOrderTable.O_ID = @id", connection);


                command.Parameters.AddWithValue("@id", orderid);
                //åben forbindelse
                connection.Open();
                // udføre kommando (henter alle medarbejdere) 
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                   


                    // loop sikre at vi læser hver eneste medarbejder i databasen.
                    while (reader.Read())
                    {
                        int recipeID = Convert.ToInt32(reader["rid"]);
                        //opret et nyt objekt for hvert medarbejder
                        Employee employee = new Employee()
                        {
                            // henter data fra databasen og sætter det på Employee objektet 
                            Id = Convert.ToInt32(reader["employeeId"]),
                            Name = reader["employeeName"].ToString(),
                            Email = reader["employeeEmail"].ToString(),
                            Password = reader["employeePassword"].ToString()
                        };
                        if (!databaseemployees.ContainsKey(recipeID))
                        {
                            //tilføje medarbejder til listen 
                            databaseemployees.Add(recipeID, employee);
                        }



                        int phone = (int)reader["phoneid"];
                        if (phone % 2 == 0)
                        {
                            databaseemployees[recipeID].WorkPhoneNumber = reader["phoneNumber"].ToString();
                        }
                        else if (phone % 2 != 0)
                        {
                            databaseemployees[recipeID].PersonalPhoneNumber = reader["phoneNumber"].ToString();
                        }

                    }
                }
                catch (SqlException sqlError)
                {
                    throw new Exception("Database error in EployeeeRepository.GetEmployeeFromOrderID(int orderid): " + sqlError.Message);

                }
                finally
                {
                    //luk reader  
                    reader.Close();

                    //luk forbindelse 
                    connection.Close();

                   
                }
                //returnere liste med alle medarbejder 
                return databaseemployees;

            }
            

        }
        public bool IsThisAnAdmin(int employeeID)
        {
            Position positionCheck = null;
            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand("select Pos_ID from EmployeePostion" +
                " where EmployeePostion.E_ID =@id"
                , sqlConnection);

            sqlCommand.Parameters.AddWithValue("@id", employeeID);


            try
            {


                sqlConnection.Open();
                SqlDataReader sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    positionCheck = new Position
                    {
                        Id = Convert.ToInt32(sqlReader["Pos_ID"])
                    };
                }

                sqlReader.Close();
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Database error in EmployeeRepo.IsThisAnAdmin(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            if(positionCheck != null)
            {
                return true;

            }
            else
            {
                return false;           
            }
        }
        public manger GetManger(int id)
        {

            SqlConnection connection = new SqlConnection(_connectionString);

            // SQL kommando: Find medarbejder med dette ID
               SqlCommand command = new SqlCommand("select Employee.E_Name as employeeName, Employee.E_ID as employeeId,Employee.E_Email as employeeEmail," +
                " Employee.E_Password as employeePassword" +
                ",Postion.Pos_ID as postionid, Postion.Pos_Name as posname from Employee" +
                " join EmployeePostion on EmployeePostion.E_ID = Employee.E_ID" +
                "  join Postion on Postion.Pos_ID = EmployeePostion.Pos_ID " +
                " where  Employee.E_ID = @id", connection);
            // her sætte vi ID parameter
            command.Parameters.AddWithValue("@id", id);

            // vi åbner forbindelsen til databasen 
            connection.Open();

            //sql commando til finde resultat
            SqlDataReader reader = command.ExecuteReader();

            //hvis der findes en medarbejder med dette ID
            if (reader.Read())
            {
                // hvis medarbejdern blev fundet, bliver der oprettet en ny objekt med data fra databasen
                manger admin = new manger()
                {
                    Id = (int)reader["employeeId"],
                    Name = reader["employeeName"].ToString(),
                    Email = reader["employeeEmail"].ToString(),
                    Password = reader["employeePassword"].ToString()
                };
                admin.position.Id = (int)reader["postionid"];
                admin.position.Name = reader["posname"].ToString();


                // lukker for reader og returner medarbejderen 
                reader.Close();
                return admin;


            }
            else // hvis der ikke findes en medarbejder med det ID
            {
                // luk for reader og returner null
                reader.Close();
                return null;
            }
        }
    }
}

