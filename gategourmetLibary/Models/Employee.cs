using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

// class represents an employee working for the company 
namespace gategourmetLibrary.Models
{
    public class Employee : User 
    {
        // Unique id
        [Required(ErrorMessage = "you must provide a valid ID")]
        [RegularExpression(@"[0-9]+", ErrorMessage = "Not a valid ID")]
        public int Id { get; set; }

        // name of employee
        [Required(ErrorMessage = "you must provide a valid name")]
        [RegularExpression(@"^[A-Z].+", ErrorMessage = "Not a valid name")]
        public string Name { get; set; }

        // mail of employee
        [Required(ErrorMessage = "you must provide a valid EmailAddress")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Not a valid EmailAddress")]
        public string Email { get; set; }

        // number of employee
        [Required(ErrorMessage = "you must provide a valid phonenumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"[0-9]{8}", ErrorMessage = "Not a valid phone number")]
        public  string WorkPhoneNumber {  get; set; }

        [Required(ErrorMessage = "you must provide a valid phonenumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"[0-9]{8}", ErrorMessage = "Not a valid phone number")]

        public string PersonalPhoneNumber { get; set; }

        public Employee()
        {

        }
    }
}
