using gategourmetLibary.Models;
using gategourmetLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        Dictionary<int, string> GetAllPostions();
        Dictionary<int, Employee> GetEmployeeFromOrderID(int orderid);
        bool IsThisAnAdmin(int employeeID);
        manger GetManger(int id);
    }
}
