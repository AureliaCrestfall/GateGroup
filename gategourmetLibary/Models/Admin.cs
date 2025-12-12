using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibrary.Models;

namespace gategourmetLibary.Models
{
    public class Admin : Employee
    {
        public Position MyPosition {  get; set; }

        public Admin()
        {
            MyPosition = new Position();
        }






    }
}
