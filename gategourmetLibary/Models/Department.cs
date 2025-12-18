using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// class representing a department
namespace gategourmetLibrary.Models
{
    public class Department
    {
        // mail for the department 
        public string DepartmentEmail { get; set; }

        // name of the department 
        public string DepartmentName { get; set; }

        // physical location of the department 
        public string DepartmentLocation { get; set; }

        // id for department 
        public int DepartmentId { get; set; }
        
        // list for employees in this department 
        public List<Employee> DepartmentEmployees { get; set; }

        // managers responsible for this department 
        public List<Position> DepartmentPositions { get; set; }

        // warehouse linked to this department 
        public List<Warehouse> DepartmentWarehouse { get; set; }

        public Department() 
        {
            DepartmentEmployees = new List<Employee>();
            DepartmentPositions = new List<Position>();
            DepartmentWarehouse = new List<Warehouse>();
        }
    }
}
