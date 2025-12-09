using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// class represents an employee working for the company 
namespace gategourmetLibrary.Models
{
    public class Employee : User 
    {
        // Unique id
        public int Id { get; set; }

        // name of employee
        public string Name { get; set; }

        // mail of employee
        public string Email { get; set; }

        // number of employee
        public  string WorkPhoneNumber {  get; set; }

        public string PersonalPhoneNumber { get; set; }

        public Employee()
        {

        }
    }
}
