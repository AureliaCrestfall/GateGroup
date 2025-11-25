using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibrary.Secret;
using Microsoft.Data.SqlClient;

namespace gategourmetLibrary.Repo
{
    public class OrderRepo : IOrderRepo
    {
        private readonly string _connectionString;
        public OrderRepo( string connectionString)
        {
            _connectionString = connectionString;
        }
        public Dictionary<int, Order> GetAll()
        {

            Dictionary<int, Order> ordersFromDatabase = new Dictionary<int, Order>();

            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
                "SELECT O_ID,O_Made,O_ready,O_paystatus,O_status FROM Ordertable",
                sqlConnection);
            /*join orderTable on OrderRecipe.O_ID = ordertable.O_ID  join recipePart on OrderRecipe.R_ID = RecipePart.R_ID",*/
            try
            {
                sqlConnection.Open();
                SqlDataReader sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    int id = Convert.ToInt32(sqlReader["O_ID"]);
                    DateTime made = Convert.ToDateTime(sqlReader["O_Made"]);
                    DateTime ready = Convert.ToDateTime(sqlReader["O_ready"]);
                    bool paystatus = Convert.ToBoolean(sqlReader["O_paystatus"]);
                    string status =  sqlReader["O_status"].ToString();
                    //int rID = Convert.ToInt32(sqlReader["R_ID"]);
                    //string howToPrep = sqlReader["R_HowToPrep"].ToString();
                    //string name = sqlReader["R_Name"].ToString();
                    //string rStatus = sqlReader["R_Status"].ToString();


                    Order order = new Order(made,ready,id,paystatus);

                    ordersFromDatabase.Add(id, order);
                }

                sqlReader.Close();
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Database error in OrderRepository.AddOrder(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return ordersFromDatabase;
        }
        //add a new order to repo

        public void AddOrder(Order newOrder)
        {

            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
                "INSERT INTO ordertable (O_ID, O_Made, O_ready, o_paystatus, O_status) " +
                "VALUES (@O_ID, @O_Made, @O_ready, @O_paystatus, @O_status)",
                sqlConnection);

           

            AddOrderTableCustomert(newOrder.ID, newOrder.CustomerOrder.ID);

            sqlCommand.Parameters.AddWithValue("@O_made", newOrder.OrderMade);
            sqlCommand.Parameters.AddWithValue("@O_status", newOrder.Status);
            sqlCommand.Parameters.AddWithValue("@O_ready", newOrder.OrderDoneBy);
            sqlCommand.Parameters.AddWithValue("@O_ID", newOrder.ID);
            sqlCommand.Parameters.AddWithValue("@O_paystatus", newOrder.paystatus);

            try
            {
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Databasefejl i OrderRepository.AddOrder(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            foreach (KeyValuePair<int,RecipePart> part in newOrder.Recipe)
            {
                AddRecipePart(part.Value);
                AddOrderRecipePart(newOrder.ID, part.Value.ID);
              
              
            }
            foreach (KeyValuePair<int, RecipePart> part in newOrder.Recipe)
            {
                foreach(Ingredient i in part.Value.Ingredients)
                {
                    AddRecipePartIngredient(part.Value.ID, i.ID);
                }
            }
            

        }


        public void AddOrderTableCustomert(int orderID,int customerID)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
            "INSERT INTO IngrefientrecipePart (O_ID, C_ID) " +
            "VALUES (@O_ID, @C_ID)",
            sqlConnection);


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
        public void AddRecipePartIngredient(int recipeID,int ingredientID)
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

        public void AddOrderRecipePart(int orderID,int recipePartID)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
             "INSERT INTO recipePart (R_ID, O_ID) " +
             "VALUES (@R_ID, @O_ID)",
             sqlConnection);

            sqlCommand.Parameters.AddWithValue("@O_ID", orderID);
            sqlCommand.Parameters.AddWithValue("@R_ID", recipePartID);
            try
            {
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
        public void AddRecipePart(RecipePart rp)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            SqlCommand sqlCommand = new SqlCommand(
                   "INSERT INTO recipePart (R_ID, R_howToPrep, R_name, R_status) " +
                   "VALUES (@R_ID, @R_howToPrep, @R_name, @R_status)",
                   sqlConnection);

            sqlCommand.Parameters.AddWithValue("@R_ID", rp.ID);
            sqlCommand.Parameters.AddWithValue("@R_howToprep", rp.Assemble);
            sqlCommand.Parameters.AddWithValue("@r_name", rp.partName);
            sqlCommand.Parameters.AddWithValue("@R_status", rp.status);


            try
            {
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException sqlError)
            {
                throw new Exception("Databasefejl i OrderRepository.AddRecipePart(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        





        public void Delete(int orderID)
        {
        }

        //returns a specific order by its ID
        public Order Get(int orderID)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            Order order = new Order();
            SqlCommand sqlCommand = new SqlCommand(
                "SELECT O_ID,O_Made,O_ready,O_paystatus,O_status FROM Ordertable where O_ID = @O_ID ",
                sqlConnection);
            sqlCommand.Parameters.AddWithValue("@O_ID", orderID);

            try
            {
                sqlConnection.Open();
                SqlDataReader sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    int id = Convert.ToInt32(sqlReader["O_ID"]);
                    DateTime made = Convert.ToDateTime(sqlReader["O_Made"]);
                    DateTime ready = Convert.ToDateTime(sqlReader["O_ready"]);
                    bool paystatus = Convert.ToBoolean(sqlReader["O_paystatus"]);
                    string status = sqlReader["O_status"].ToString();



                    order = new Order(made, ready, id, paystatus);

                }

            }
            catch (SqlException sqlError)
            {
                throw new Exception("Database error in OrderRepository.AddOrder(): " + sqlError.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return order;
            }
        

        public void Update(int orderID, Order updateOrder)
        {
            
        }   
       

        public Order filterAfterWhoMade(Employee filterAfterWhoMade)
        {
            return
                 null;
        }

        public Order filterAfterOrderToday(DateTime filterAfterOrderToday)
        {
            return
                 null;
        }
    
        //returns the list of all orders
        public List<Order> GetAllOrders()
        {
            return null;
        }
       
        //delete an order by its ID
        public void DeleteOrder(int orderID)
        {

        }
        //update an existing order by its ID
        public void UpdateOrder(int orderID, Order UpdatedOrder)
        {

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
        { return null;
        }
        //filters orders for a specific customer/company
        public List<Order> FilterByCompany(Customer customer)
        {  
          return null;
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
       
    }
}
