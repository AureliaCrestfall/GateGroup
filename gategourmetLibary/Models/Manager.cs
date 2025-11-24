using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// class represents a manager 
namespace gategourmetLibrary.Models
{
    public class Manager : Employee
    {
        // title or position of the manager 
        public string Position { get; set; }
    }
}
