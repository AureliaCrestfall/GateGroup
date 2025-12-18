using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;

namespace gategourmetLibrary.Repo
{
    public interface ICustomerRepo
    {
        //returns all customers
        List<Customer> GetAllCustomers();
        //adds a new customer
        void AddCustomer(Customer customer);
        //deletes a customer by ID
        void DeleteCustomer(int customerId);
        //updates customer info by ID
        void UpdateCustomer(int customerId, Customer updatedCustomer);
        //returns a specific customer by ID
        Customer GetCustomer(int customerId);
        //returns orders for a specific customer by ID
        List<Order> GetCustomerOrders(int customerId);
        //filters customers by name
        List<Customer> FilterCustomersByName(string name);
        Customer GetCustomerByOrder(int orderid);

        //void Add(int customer);
        //int Get();
        //void Delete(int customer);
        //void GetAll();
        //void MyOrder(int customer);
        //void Update(int customer);
        //void Filter(string customer);
    }
}
