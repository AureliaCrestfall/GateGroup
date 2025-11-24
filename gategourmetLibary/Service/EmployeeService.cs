using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _iemployee.Add(employee);
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


        public List<Employee> GetAll()
        {
            return _iemployee.GetAll();
        }


        public List<Employee> Filter(string employee)
        {
            return _iemployee.Filter(employee);
        }


    }
}
