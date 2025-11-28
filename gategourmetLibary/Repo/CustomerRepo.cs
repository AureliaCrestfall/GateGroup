using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.Data.SqlClient;

namespace gategourmetLibrary.Repo
{
    public class CustomerRepo : ICustomerRepo
    {
        // connection string to connect to the database
        private readonly string _connectionString;
        // constructor to initialize the connection string
        public CustomerRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        //returns all customers
        public List<Customer> GetAllCustomers()
        {
            //temporary list to hold customers
            List<Customer> customers = new List<Customer>();
            //using (using) to ensure the connection is closed after use
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //sql command to select all customers
                SqlCommand command = new SqlCommand("SELECT * FROM Customer", connection);
                    //open database connection
                    connection.Open();
                //execute command and read data
                SqlDataReader reader = command.ExecuteReader();
                //loop through each returned row
                while (reader.Read())
                {
                    customers.Add(new Customer
                    {
                        ID = (int)reader["C_ID"],
                        Name = reader["C_Name"].ToString(),
                        Password = reader["C_Password"].ToString()
                    });
                }
            }
            //return the list of customers
            return customers;
        }
        //adds a new customer
        public void AddCustomer(Customer customer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(@"
            INSERT INTO Customer (C_Name, C_Password, C_Email, C_CompanyName, C_CVR)
            VALUES (@Name, @Password, @Email, @CompanyName, @CVR)", connection);

                command.Parameters.AddWithValue("@Name", customer.Name);
                command.Parameters.AddWithValue("@Password", customer.Password);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@CompanyName", customer.CompanyName);
                command.Parameters.AddWithValue("@CVR", customer.CVR);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

       
        public void DeleteCustomer(int customerId)
        {
            //using (using) to ensure the connection is closed after use
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //first delete customer relationships in CustomerOrder table
                SqlCommand deleteRelations = new SqlCommand("DELETE FROM orderCustomer WHERE C_ID = @Id", connection);
                deleteRelations.Parameters.AddWithValue("@Id", customerId);
                //then delete the customer from Customer table
                SqlCommand deleteCustomer = new SqlCommand("DELETE FROM Customer WHERE C_ID = @Id", connection);
                deleteCustomer.Parameters.AddWithValue("@Id", customerId);
                //open database connection
                connection.Open();
                //execute both delete commands
                deleteRelations.ExecuteNonQuery();
                deleteCustomer.ExecuteNonQuery();
            }

        }
        //updates customer information by ID
        public void UpdateCustomer(int customerId, Customer updatedCustomer)
        {
            //using (using) to ensure the connection is closed after use
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //sql command to update customer information
                SqlCommand command = new SqlCommand("UPDATE Customer SET C_Name = @Name, C_Password = @Password WHERE C_ID = @Id", connection);
                //add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@Name", updatedCustomer.Name);
                command.Parameters.AddWithValue("@Password", updatedCustomer.Password);
                command.Parameters.AddWithValue("@Id", customerId);
                //open database connection
                connection.Open();
                //execute the update command
                command.ExecuteNonQuery();
            }

        }
        //returns a specific customer by ID
        public Customer GetCustomer(int customerId)
        {
            //using (using) to ensure the connection is closed after use
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //sql command to select a customer by ID
                SqlCommand command = new SqlCommand("SELECT * FROM Customer WHERE C_ID = @Id", connection);
                command.Parameters.AddWithValue("@Id", customerId);
                //open database connection
                connection.Open();
                //execute command and read data
                SqlDataReader reader = command.ExecuteReader();
                //if a row is returned, create and return the customer object
                if (reader.Read())
                {
                    return new Customer
                    {
                        ID = (int)reader["C_ID"],
                        Name = reader["C_Name"].ToString(),
                        Password = reader["C_Password"].ToString()
                    };
                }
            }
            //if no customer is found, return null
            return null;
        }
        //returns orders for a specific customer by ID
        public List<Order> GetCustomerOrders(int customerId)
        {
            //temporary list to hold orders
            List<Order> orders = new List<Order>();
            //using (using) to ensure the connection is closed after use
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //sql command to select orders for the customer
                SqlCommand command = new SqlCommand("SELECT o.* FROM orderTable o INNER JOIN OrderCustomer oc ON o.O_ID = oc.O_ID WHERE oc.C_ID = @Id", connection);
                command.Parameters.AddWithValue("@Id", customerId);
                //open database connection
                connection.Open();
                //execute command and read data
                SqlDataReader reader = command.ExecuteReader();
                //loop through each returned row
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        ID = (int)reader["O_ID"],
                        OrderMade = (DateTime)reader["O_Made"],
                        OrderDoneBy = (DateTime)reader["O_ready"],
                        paystatus = (bool)reader["O_PayStatus"],

                        //convert string column to Enum
                        Status = (OrderStatus)Enum.Parse<OrderStatus>(reader["O_Status"].ToString())
                    });
                }
            }
            //return the list of orders
            return orders;
        }
        //filters customers by name
        public List<Customer> FilterCustomersByName(string name)
        {
            //temporary list to hold filtered customers
            List<Customer> customers = new List<Customer>();
            //using (using) to ensure the connection is closed after use
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //sql command to select customers with names matching the filter (using LIKE is better for search/filter)
                SqlCommand command = new SqlCommand("SELECT * FROM Customer WHERE C_Name LIKE @Name", connection);
                //using wildcard % for partial matches
                command.Parameters.AddWithValue("@Name", "%" + name + "%");
                //open database connection
                connection.Open();
                //execute command and read data
                SqlDataReader reader = command.ExecuteReader();
                //loop through each returned row
                while (reader.Read())
                {
                    customers.Add(new Customer
                    {
                        ID = (int)reader["C_ID"],
                        Name = reader["C_Name"].ToString(),
                        Password = reader["C_Password"].ToString()
                    });
                }
            }
            //return the list of filtered customers
            return customers;
        }
    }
}
