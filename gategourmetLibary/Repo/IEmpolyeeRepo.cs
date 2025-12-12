using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibary.Models;
using gategourmetLibrary.Models;

namespace gategourmetLibrary.Repo
{
    public interface IEmpolyeeRepo
    {
        void Add(Employee empolyee);
        Employee Get(int employee);
        void Delete(int empolyee);
        Dictionary<int,Employee> GetAll();
        void Update(Employee empolyee);
        List<Employee> Filter(string empolyee);
        public void AddNewAdmin(Admin admin);

        Dictionary<int, string> GetAllPostions();
    }
}
