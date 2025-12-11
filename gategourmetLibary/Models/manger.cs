using gategourmetLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gategourmetLibary.Models
{
    public class manger:Employee
    {
        public Position position { get; set; }

        public manger() : base()
        {
            position = new Position();
        }
    }
}
