using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// base class for all the users (employee and customers)
namespace gategourmetLibrary.Models
{
    public abstract class User
    {
        // password used for login     
        public string Password { get; set; }
    }
}
