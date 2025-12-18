using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// class for a chef 
namespace gategourmetLibrary.Models
{
    public class Chef : Employee
    {

        // tasks for this specific chef, each order is mapped to one recipe part
        public Dictionary<Order,RecipePart> MyTasks { get; set; }

        public Chef() 
        {
            MyTasks = new Dictionary<Order, RecipePart>();
        }
    }
}
