using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibrary.Models;
using gategourmetLibrary.Repo;

namespace gategourmetLibrary.Service
{
    public class CustomerService
    {
        private readonly ICustomerRepo _customerRepo;
        // Constructor to inject the order repository
        public CustomerService(ICustomerRepo customerRepo)
        {
            _customerRepo = customerRepo;
        }
        // returns all customers
        public List<Customer> GetAllCustomers()
        {
            return _customerRepo.GetAllCustomers();
        }
        // adds a new customer
        public void AddCustomer(Customer customer)
        {
            Debug.WriteLine(customer);
            _customerRepo.AddCustomer(customer);
        }
        // deletes a customer by ID
        public void DeleteCustomer(int customerId)
        {
            _customerRepo.DeleteCustomer(customerId);
        }
        // updates customer information by ID
        public void UpdateCustomer(int customerId, Customer updatedCustomer)
        {
            _customerRepo.UpdateCustomer(customerId, updatedCustomer);
        }
        // returns a specific customer by ID
        public Customer GetCustomer(int customerId)
        {
            return _customerRepo.GetCustomer(customerId);
        }
        // returns orders for a specific customer by ID
        public List<Order> GetCustomerOrders(int customerId)
        {
            return _customerRepo.GetCustomerOrders(customerId);
        }
        //filters customers by name
        public List<Customer> FilterCustomersByName(string name)
        {
            return _customerRepo.FilterCustomersByName(name);
        }
    }
}
