using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibary.Models;
using gategourmetLibrary.Models;
using gategourmetLibrary.Repo;

namespace gategourmetLibrary.Service
{
    public class EmployeeService
    {
        private readonly IEmpolyeeRepo _iemployee;

        public EmployeeService(IEmpolyeeRepo repo)
        {
            _iemployee = repo;
        }

        // employee oprettes
        public void Add(Employee employee)
        {
            Dictionary<int,Employee> databaseEmployees = GetAll();
            if (employee != null)
            {
                if (!databaseEmployees.ContainsKey(employee.Id))
                {
                    _iemployee.Add(employee);
                }
                else
                {
                    throw new Exception("their already exists an employee with this ID");
                }
            }
            else
            {
                throw  new NullReferenceException();
            }
        }


        public void delete(int employeeId)
        {
            _iemployee.Delete(employeeId);
        }

        public void Update(Employee employee)
        {
            _iemployee.Update(employee);
        }

        // finde en medarbejder med det bestemte id
        public Employee Get(int employeeId)
        {
            return _iemployee.Get(employeeId);
        }


        public Dictionary<int,Employee> GetAll()
        {
            return _iemployee.GetAll();
        }


        public List<Employee> Filter(string employee)
        {
            return _iemployee.Filter(employee);
        }

        public Dictionary<int,string> GetAllPostions()
        {
            return _iemployee.GetAllPostions();
        }
        public Dictionary<int, Employee> GetEmployeeFromOrderID(int orderid)
        {
            return _iemployee.GetEmployeeFromOrderID(orderid);
        }
        public bool LoginAdmin(int employeeId,string password)
        {
            if(_iemployee.IsThisAnAdmin(employeeId) == true)
            {
                Admin loginAttempt = _iemployee.GetManger(employeeId);
                
                if(loginAttempt.Password == password)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }


        }

        public void AddNewAdmin(Admin admin)
        {
            _iemployee.AddNewAdmin(admin);
        }

        public Dictionary<int, string> GetEmployeesForFilter()
        {
            return _iemployee.GetEmployeesForFilter();
        }

        public List<int> GetOrderIdsByEmployeeId(int employeeId)
        {
            return _iemployee.GetOrderIdsByEmployeeId(employeeId);
        }

        public List<EmployeeTask> GetEmployeeTasks(int employeeId)
        {
            return _iemployee.GetEmployeeTasks(employeeId);
        }

        public void MarkTaskDone(int employeeId, int orderId, int recipePartId)
        {
            _iemployee.MarkTaskDone(employeeId, orderId, recipePartId);
        }



    }
}
