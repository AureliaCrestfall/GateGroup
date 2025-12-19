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
        public void AddNewAdmin(Admin admin);
        void AddPhonenumber(string phone, int employeeID);
        void AddEmployeePhoneLink(int phoneID, int employeeID);
        Dictionary<int, string> GetAllPostions();
        Dictionary<int, Employee> GetEmployeeFromOrderID(int orderid);
        bool IsThisAnAdmin(int employeeID);
        Admin GetAdmin(int id);
        Dictionary<int, string> GetEmployeesForFilter();
        List<int> GetOrderIdsByEmployeeId(int employeeId);
        List<EmployeeTask> GetEmployeeTasks(int employeeId);
        void MarkTaskDone(int employeeId, int orderId, int recipePartId);

        void AsignTask(int employeeid, int orderid, int recpiepartid);
    }
}
