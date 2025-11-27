using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibrary.Repo;
using gategourmetLibrary.Models;

namespace gategourmetLibrary.Service
{
    public class DepartmentService
    {
        private readonly IDepartmentRepo _departmentRepo;
        // Constructor to inject the department repository
        public DepartmentService(IDepartmentRepo departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }
        // gets all departments
        public List<Department> GetAllDepartments()
        {
            return _departmentRepo.GetAllDepartments();
        }
        // adds a new department
        public void AddDepartment(Department newDepartment)
        {
            _departmentRepo.AddDepartment(newDepartment);
        }
        // deletes a department by ID
        public void DeleteDepartment(int departmentId)
        {
            _departmentRepo.DeleteDepartment(departmentId);
        }
        // updates department info by ID
        public void UpdateDepartment(int departmentId, Department updatedDepartment)
        {
            _departmentRepo.UpdateDepartment(departmentId, updatedDepartment);
        }
        // gets a specific department by ID
        public Department GetDepartment(int departmentId)
        {
            return _departmentRepo.GetDepartment(departmentId);
        }
        // assigns a new warehouse to a department
        public void NewWarehouse(Warehouse newWarehouse)
        {
            _departmentRepo.NewWarehouse(newWarehouse);
        }
        // to stock an ingredient in the department's warehouse
        public void StockIngredient(Ingredient stockIngredient)
        {
            _departmentRepo.StockIngredient(stockIngredient);
        }
        // gets the stock of a specific warehouse
        public List<Ingredient> GetWarehouseStock(int warehouseId)
        {
            return _departmentRepo.GetWarehouseStock(warehouseId);
        }
        // gets the managers of a specific department
        public List<Position> GetDepartmentPositions(int departmentId)
        {
            return _departmentRepo.GetDepartmentPositions(departmentId);
        }
        // gets the employees of a specific department
        public List<Employee> GetDepartmentEmployees(int departmentId)
        {
            return _departmentRepo.GetDepartmentEmployees(departmentId);
        }
       
        // adds a new employee to a department
        public void AddNewDepartmentEmployee(int departmentId, Employee newEmployee)
        {
            _departmentRepo.AddNewDepartmentEmployee(departmentId, newEmployee);
        }

        // adds a new position (role) to an employee in a department
        public void AddNewDepartmentPosition(int departmentId, Employee employee, Position newPosition)
        {
            _departmentRepo.AddNewDepartmentPosition(departmentId, employee, newPosition);
        }

        // removes stock from a departments warehouse
        public void RemoveStock(Ingredient ingredient, int amount, int departmentID, int warehouseID)
        {
            _departmentRepo.RemoveStock(ingredient, amount, departmentID, warehouseID);
        }
    }
}
