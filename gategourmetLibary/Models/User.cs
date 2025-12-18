using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// base class for all the users (employee and customers)
namespace gategourmetLibrary.Models
{
    public abstract class User
    {
        // password used for login     
        [Required(ErrorMessage ="you must provide a valid password")]
        [RegularExpression(@"^[\w-\.]{5,17}", ErrorMessage = "Not a valid password")]
        public string Password { get; set; }
    }
}
